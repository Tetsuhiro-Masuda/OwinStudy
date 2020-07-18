using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace SelfHostServer
{
    public class MainClass
    {
        public static void Main()
        {
            using (WebApp.Start<StartUp>("http://localhost:9000"))
            {
                new TaskFactory().StartNew(InputQuit).Wait();
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
