using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace SelfHostServer
{
    public class MainClass
    {
        public static async Task Main()
        {
            using (WebApp.Start<StartUp>("http://localhost:9000"))
            {
                await Task.Factory.StartNew(InputQuit).ConfigureAwait(false);
            }
        }

        private static void InputQuit()
        {
            do
            {
                Console.WriteLine("if quit server press Q or q");
            }
            while (Console.ReadLine().ToUpper() != "Q");
        }

    }
}

