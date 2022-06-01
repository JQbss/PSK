using Server.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.protocols
{
    public class TCPL : IListener
    {
        TcpListener server;
        IPAddress localAddress; 
        CommunicatorD onConnect;
        int port;

        public TCPL(IPAddress localAddress, int port)
        {
            this.localAddress = localAddress;
            this.port = port;

        }

        public TCPL(string config)
        {
            if(config != "")
            {
                string[] splitted = config.Split(' ');
                if(splitted.Length > 1)
                {
                    this.localAddress = IPAddress.Parse(splitted[0]);
                    this.port = int.Parse(splitted[1]);
                }
            }
        }

        private void AcceptHandler(IAsyncResult result)
        {
            TcpClient client = server.EndAcceptTcpClient(result);
            server.BeginAcceptTcpClient(AcceptHandler, server);
            ICommunicator communicator = new TCPC(client);
            onConnect(communicator);
            Console.WriteLine("Udalo się nazwiazac polaczenie z " + client.Client.RemoteEndPoint);
        }
        public void Start(CommunicatorD onConnect)
        {
            this.onConnect = onConnect; 
            server = new TcpListener(localAddress, port);
            server.Start();
            server.BeginAcceptTcpClient(AcceptHandler, server);
        }

        public void Stop()
        {
            server.Stop();
        }
    }

    public class TCPC : ICommunicator
    {

        private TcpClient client;
        Task task;
        CommandD onCommand;
        CommunicatorD onDisconnect;

        public TCPC(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            task = new Task(() => TaskHandler());
            task.Start();
        }

        private void TaskHandler()
        {
            Console.WriteLine("Rozpoczecie komunikacji");
            NetworkStream stream = client.GetStream();
            byte[] bytes = new byte[256];

            string data = string.Empty;

            while (client.Connected)
            {
                try
                {
                    if (stream.DataAvailable)
                    {
                        int len = stream.Read(bytes, 0, bytes.Length);
                        data += Encoding.ASCII.GetString(bytes, 0, len);
                    }
                    else if(data != string.Empty)
                    {
                        string message = onCommand(data);
                        bytes = Encoding.ASCII.GetBytes(message);
                        stream.Write(bytes, 0, bytes.Length);
                        Console.WriteLine("Wysłano: {0}", message);
                        data = string.Empty;
                    }
                }
                catch
                {
                    Stop();
                }
            }
            stream.Close();
        }

        public void Stop()
        {
            onDisconnect(this);
            client.Close();
            Console.WriteLine("Koniec komunikacji");
        }
    }
}
