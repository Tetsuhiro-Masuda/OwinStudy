using System;
using System.IO;
using Owin;

namespace SelfHostServer
{
    internal class StartUp
    {
        //use https://docs.microsoft.com/ja-jp/aspnet/aspnet/overview/owin-and-katana/owin-startup-class-detection
        public void Configuration(IAppBuilder app)
        {
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