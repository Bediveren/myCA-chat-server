using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApplication
{
    public struct CommandHandler
    {

        public const string CommandDisconnect = "/disconnect";
        public const string CommandSetName = "/set name";
        public const string CommandClientList = "/set client list";


        public ClientHandler clientConnection;
        public CommandHandler(ClientHandler clientConnection)
        {
            this.clientConnection = clientConnection;

           

            clientConnection.EventClientConnect += OnEventClientConnect;
            clientConnection.EventClientDisconnect += OnEventClientDisconnect;
            clientConnection.EventServerSendMessage += OnEventServerSendMessage;
            clientConnection.EventServerRecieveMessage += OnEventServerRecieveMessage;
           

        }
        public void SendMessageToClient(ClientHandler ch, string message = "")
        {
            try
            {
                if (message != "")
                {
                    ch.binaryWriter.Write(message);
                    ch.binaryWriter.Flush();
                }
            }
            catch (Exception)
            { }
        }

        public void OnEventServerSendMessage(object source, ServerCommandEventArgs args)
        {
            Console.WriteLine("Send Message method called");
            // send to a list on clients***
            
            Console.WriteLine("Message recieved:" + args.Message);
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                SendMessageToClient(client, clientConnection.clientName + ": " + args.Message);
            }
        }

        public string ConstructClientListString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(CommandClientList);
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                stringBuilder.Append(client.clientName).Append("\n");
            }
            return stringBuilder.ToString();

        }

        public bool IsNameTaken(string name)
        {
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                if (name == client.clientName)
                {
                    return true;
                }
            }
            return false;
        }

        public void SendUserListToClients()
        {
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                SendMessageToClient(client, ConstructClientListString());
            }
        }

        public void OnEventClientConnect(object source, ServerCommandEventArgs args)
        {
            //send to a list of clients***
            //args.Message = "A client is connecting to server...";
            //clientConnection.clientName = args.Message;
            SendUserListToClients();
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                SendMessageToClient(client, clientConnection.clientName + " connected to server!");
            }
        }

        public void OnEventClientDisconnect(object source, ServerCommandEventArgs args)
        {
            // args.Message = "A client is disconnecting from server...";
            //notify other users that he left
            ClientHandler.clientList.Remove(clientConnection);
            SendUserListToClients();
            foreach (ClientHandler client in ClientHandler.clientList)
            {
                SendMessageToClient(client, clientConnection.clientName + " disconnected from server :(");
            }
           
            clientConnection.CloseConnection();
        }

        public void OnEventServerRecieveMessage(object source, ServerCommandEventArgs args)
        {
            //verify the message
           if(args.Message.StartsWith(CommandSetName))
            {
                if (clientConnection.clientName == null)
                {
                    clientConnection.clientName = args.Message.Substring(CommandSetName.Length).Trim();
                    clientConnection.OnEventClientConnect(new ServerCommandEventArgs() { Message = clientConnection.clientName }); 
                }
                else
                {
                    if (args.Message.Substring(CommandSetName.Length).Trim() != "")
                    {
                        if (!IsNameTaken(args.Message.Substring(CommandSetName.Length).Trim()))
                        {
                            clientConnection.clientName = args.Message.Substring(CommandSetName.Length).Trim();
                        }
                    }
                }
                SendUserListToClients();

               // clientConnection.OnEventClientConnect(new ServerCommandEventArgs() {Message = clientConnection.clientName});
            }
           else if (args.Message.StartsWith(CommandDisconnect))
            {
                clientConnection.OnEventClientDisconnect(new ServerCommandEventArgs() {Message = clientConnection.clientName});
            }
           else
            {
                clientConnection.OnEventServerSendMessage(args);
            }


        }
        
    }
}
