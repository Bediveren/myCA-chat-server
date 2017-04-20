using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(2000);
            server.Start();

            while (server.Listen()) { }


            server.Stop();
            Console.ReadKey();

        }
    }
}
