using Server.utils;
using System;
using System.Collections.Generic;
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
        int port;

        public TCPL(IPAddress localAddress, int port)
        {
            this.localAddress = localAddress;
            this.port = port;

        }
        public void Start(CommunicatorD onConnect)
        {
            server = new TcpListener(localAddress, port);
            server.Start();
            byte[] bytes = new byte[256];
            while (true)
            {
                Console.WriteLine("Trwa łączenie....");
                TcpClient client = server.AcceptTcpClient();

                Console.WriteLine("Udało się nazwiązać połączenie!");
                //od 34 lini to komunikator nie listener
                int len;
                string data = null;
                NetworkStream stream = client.GetStream();
                while ((len = stream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    data += Encoding.ASCII.GetString(bytes, 0, len);
                    ICommunicator communicator = new TCPC(client);
                    //listener ma tylko nasłuchiwać i tworzy komunikatory
                    CommandD command = new CommandD(Ping.Pong);
                    //on Connect metoda serwera która uruchomi komunikator
                    communicator.Start(command, onConnect);
                    data = null;
                }
                client.Close();
            }
        }

        public void Stop()
        {
            server.Stop();
        }
    }

    public class TCPC : ICommunicator
    {

        private TcpClient client;
        public TCPC(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        //dostosowywać do rozmiaru
        //dostaje pytanie zwraca odpowiedź
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            NetworkStream stream = client.GetStream();
            string message = onCommand("pong 1024");
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Console.WriteLine("Wysłano: {0}", message);
        }

        public void Stop()
        {
            client.Close();
        }
    }
}
