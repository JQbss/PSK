﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.protocols
{
    public class UDPL : IListener
    {
        private UdpClient client;
        private IPAddress localAddress;
        private int port;

        public UDPL(IPAddress localAddress, int port)
        {
            this.localAddress = localAddress;
            this.port = port;
        }

        public UDPL(string config)
        {
            if (config != "")
            {
                string[] splitted = config.Split(' ');
                if (splitted.Length > 1)
                {
                    this.localAddress = IPAddress.Parse(splitted[0]);
                    this.port = int.Parse(splitted[1]);
                }
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            IPEndPoint iPEndPoint = new IPEndPoint(localAddress, port);
            client = new UdpClient(iPEndPoint);
            onConnect(new UDPC(client, iPEndPoint));
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }

    public class UDPC : ICommunicator
    {
        private UdpClient client;
        private IPEndPoint iPEndPoint;
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        public UDPC(UdpClient client)
        {
            this.client = client;
        }

        public UDPC(UdpClient client, IPEndPoint iPEndPoint)
        {
            this.iPEndPoint = iPEndPoint;
            this.client = client;
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            client.BeginReceive(new AsyncCallback(TaskHandler),client);
        }

        private void TaskHandler(IAsyncResult asyncResult)
        {
            try
            {
                byte[] reciveBytes = client.EndReceive(asyncResult, ref iPEndPoint);
                string reciveString = Encoding.ASCII.GetString(reciveBytes);
                string recive = onCommand(reciveString);
                byte[] sendBytes = Encoding.ASCII.GetBytes(recive);

                Console.WriteLine(recive);

                client.Send(sendBytes,sendBytes.Length,iPEndPoint);
                client.BeginReceive(new AsyncCallback(TaskHandler), client);
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                onDisconnect(this);
            }
           
        }

        public void Stop()
        {
            onDisconnect(this);
            client.Close();
            Console.WriteLine("Koniec komunikacji");
        }
    }
}
