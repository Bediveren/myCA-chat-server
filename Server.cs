using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerApplication
{
    class Server
    {
        private readonly IPAddress serverIP;
        private readonly int serverPort;
        TcpListener server;


        public Server(int port = 1986)
        {
            serverIP = IPAddress.Any;                           // Assigning IP
            serverPort = port;                                  // Giving default port   
            server = new TcpListener(serverIP, serverPort);                
        }

        public void Start()
        {
            try
            {
                server.Start();
            }
            catch( SocketException ex)
            {
                ex.Data.Add("Description", "Error initializing server");
                throw ex;
            }

        }
        public void Stop()
        {
            try
            {
                server.Stop();
            }
            catch (SocketException ex)
            {
                ex.Data.Add("Description", "Error finalizing server");
                throw ex;
            }
        } 
        public bool Listen()
        {
            TcpClient client = server.AcceptTcpClient();
            new ClientHandler(client);
            return true;
               
        }

    }
}
