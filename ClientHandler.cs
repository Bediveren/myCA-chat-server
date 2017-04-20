using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace ServerApplication
{
    public class ServerCommandEventArgs : EventArgs
    {
        public string Message { get; set; }
    }


    public class ClientHandler
    {

        public static List<ClientHandler> clientList = new List<ClientHandler>();
        public event EventHandler<ServerCommandEventArgs> EventServerSendMessage;
        public event EventHandler<ServerCommandEventArgs> EventClientConnect;
        public event EventHandler<ServerCommandEventArgs> EventClientDisconnect;
        public event EventHandler<ServerCommandEventArgs> EventServerRecieveMessage;
        public TcpClient client;                                                                           
        public NetworkStream netStream;                                    
        public BinaryReader binaryReader;
        public BinaryWriter binaryWriter;                                                                                                  
        //public EventSystem eventSystem;
        public CommandHandler commandHandler;
        public bool clientConnected = false;
        public string clientName = null;
        // Event system with all events defined



        public ClientHandler(TcpClient client)                                                                                         // Establishing connection, listening
        {
            // Add to list
            // Assing subscribers
            if (client != null)
            {
                try
                {
                    SetupListening(client);
                }
                catch (Exception ex)
                { }

                //Call Listen in a thread
                Console.WriteLine("Client sucesfully initiated, ready to be listened");
               // OnEventClientConnect(new ServerCommandEventArgs() { Message = binaryReader.ReadString() });
                new Thread(Listen).Start();
            }
        }

        private void Listen()                                                                                   // Listening for clinet input
        {

            if(client != null)
            {
                
                clientConnected = true;
                #region Listening
                while (clientConnected)
                {
                     try
                    {
                        String messageRecieved = binaryReader.ReadString();
                        OnEventServerRecieveMessage(new ServerCommandEventArgs() { Message = messageRecieved });

                    }   
                    catch(IOException ex)
                    {
                        // No input recieved
                    }
                    
                    // Reducing workload
                    Thread.Sleep(10);

                }
                #endregion


            }

        }                                                                                                             

        private void SetupListening(TcpClient client)
        {
           
            this.client = client;
            try
            {
                netStream = client.GetStream();
                binaryReader = new BinaryReader(netStream, Encoding.UTF8);
                binaryWriter = new BinaryWriter(netStream, Encoding.UTF8);
            }
            catch (IOException ex)
            {
                ex.Data.Add("Description", "Error initializing Binary IO");
                throw ex;
            }
            catch (Exception ex)
            {
                ex.Data.Add("Description", "Unknown error setting up listening for client");
                throw ex;
            }
           // eventSystem = new EventSystem();
            commandHandler = new CommandHandler(this);
            clientList.Add(this);
        }

        
        public virtual void CloseConnection()
        {
            //throw away from list
            clientConnected = false;
            binaryWriter.Close();
            binaryReader.Close();
            netStream.Close();
            client.Close();
        }
        public virtual void OnEventServerSendMessage(ServerCommandEventArgs args)
        {
            if (EventServerSendMessage != null)
            {
                EventServerSendMessage(this, args);
            }
        }

        public virtual void OnEventClientConnect(ServerCommandEventArgs args)
        {
            if (EventClientConnect != null)
            {
                EventClientConnect(this, args);
            }
        }

        public virtual void OnEventClientDisconnect(ServerCommandEventArgs args)
        {
            if (EventClientDisconnect != null)
            {
                EventClientDisconnect(this, args);
            }
        }
        public virtual void OnEventServerRecieveMessage(ServerCommandEventArgs args)
        {
            if (EventServerRecieveMessage != null)
            {
                EventServerRecieveMessage(this, args);
            }

        }


    }
}
