using System;
using Shivan.Web;

namespace Shivan.Test
{
    class Program
    {
        static int Main(string[] args)
        {
            var server = new HttpServer();
            server.Start();
            Console.WriteLine("server.Start() called");
            return 0;
        }
    }
}
