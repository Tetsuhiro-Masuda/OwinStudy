using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace SelfHostServer
{
    internal class StartUp
    {
        public StartUp()
        {
            //need reflection
        }

        //use https://docs.microsoft.com/ja-jp/aspnet/aspnet/overview/owin-and-katana/owin-startup-class-detection
        public void Configuration(IAppBuilder app)
        {
            // details https://docs.microsoft.com/ja-jp/aspnet/core/fundamentals/static-files?view=aspnetcore-3.1
            var fileServerOption = new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "webroot")),
                RequestPath = new PathString("/Contents1/Contents2"),
                EnableDirectoryBrowsing = true,
            };
            fileServerOption.DefaultFilesOptions.DefaultFileNames = new List<string> { Path.Combine("Contents1", "Contents2", "testPicture2.png") };
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
    }
}