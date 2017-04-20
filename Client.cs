using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerApplication
{
    class Client
    {
        private string name{get;set;}

        public Client(TcpClient client, string name)
        {
            new ClientHandler(client);
            this.name = name;

        }

    }
}
