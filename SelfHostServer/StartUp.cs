using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace SelfHostServer
{
    internal class StartUp
    {
        private readonly PathString _applicationPath;

        public StartUp()
        {
            _applicationPath = new PathString("/Contents1");
            //_applicationPath = new PathString("/Contents1/Contents2");
        }

        //use https://docs.microsoft.com/ja-jp/aspnet/aspnet/overview/owin-and-katana/owin-startup-class-detection
        public void Configuration(IAppBuilder app)
        {
            //applicationPath 以下の呼び出しに読み替える
            // http://localhost:9000/testPicture.png → http://localhost:9000/Contents1/testPicture.png
            app.Use<RewriteMiddleWere>(_applicationPath);

            // details https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1
            var fileServerOption = new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webroot")),
                EnableDirectoryBrowsing = true,
            };
            fileServerOption.StaticFileOptions.OnPrepareResponse = ctx =>
            {
                //キャッシュを削除しておかないと誤った応答が返り続けることがある
                ctx.OwinContext.Response.Headers["Cache-Control"] = "no-cache, no-store";
            };

            fileServerOption.DefaultFilesOptions.DefaultFileNames = new List<string> { Path.Combine("testPicture.png") };
            app.UseFileServer(fileServerOption);

            //fileServer = {staticFile,defaultFile,DirectoryBrowser}　下３つの複合
            //staticFile = provide file contents ファイルだけ提供したければこれで十分
            //defaultFile = rewrite {default.htm,default.html,indexhtm,index.html} or my entry point
            // webroot内のフォルダ要求に対し既定or指定のファイルを返すようにする
            //directoryBrowser = allow directory referrence ブラウザからディレクトリ構成が見れるようになる

            app.Use((context, next) =>
            {
                var output = context.Get<TextWriter>("host.TraceOutput");
                return next().ContinueWith(result =>
                {
                    output.WriteLine("Scheme {0} : Method {1} : Path {2} : MS {3}",
                    context.Request.Scheme, context.Request.Method, context.Request.Path, getTime());
                });
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(getTime() + " My First OWIN App");
            });
        }

        string getTime()
        {
            return DateTime.Now.Millisecond.ToString();
        }

        public class RewriteMiddleWere : OwinMiddleware
        {
            private readonly PathString _applicationPath;

            public RewriteMiddleWere(OwinMiddleware next, PathString applicationPath) : base(next)
            {
                _applicationPath = applicationPath;
            }

            public override Task Invoke(IOwinContext context)
            {
                context.Request.Path = _applicationPath.Add(context.Request.Path);
                return Next.Invoke(context);
            }
        }
    }
}