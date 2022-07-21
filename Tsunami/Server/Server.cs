using Server.enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        public string path { get; set; } = string.Empty;
        public uint PackSize { get; set; } = 256;
        Dictionary<int, string> files = new Dictionary<int, string>();

        ServerStatus serverStatus = ServerStatus.New;

        List<IListener> TCPListeners = new List<IListener>();
        List<ICommunicator> TCPcommunicators = new List<ICommunicator>();

        List<IListener> UDPListeners = new List<IListener>();
        List<ICommunicator> UDPcommunicators = new List<ICommunicator>();

        private object _lockCommunicator = new object();
        private object _lockListener = new object();
        private int packetNumber = 0;
        static int port = 12345;
        void AddTCPListener(IListener listener)
        {
            lock (_lockListener)
            {
                TCPListeners.Add(listener);
                if (serverStatus == ServerStatus.Running)
                {
                    listener.Start(new CommunicatorD(AddTCPCommunicator));
                }
            }

        }

        void AddTCPCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                TCPcommunicators.Add(communicator);
                if (serverStatus == ServerStatus.Running)
                {
                    communicator.Start(new CommandD(GetTCPAnswer), RemoveTCPCommunicator);
                }
            }
        }
        private string GetTCPAnswer(string command)
        {
            if(path != string.Empty)
            {
                string[] splitted = command.Split();
                if(splitted.Length > 1)
                {
                    switch (splitted[0])
                    {
                        case "getfile":
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            if(!File.Exists(path + "/" + splitted[1]))
                            {
                                return "Podany plik nie istnieje";
                            }
                            AddUDPListener(new protocols.UDPL(IPAddress.Any, port));
                            string file = Utils.Base64Converter.FileToBase64(path + "/" + splitted[1]);
                            files.Add(packetNumber, file);
                            return "OK " + file.Length + " " + PackSize + " " + packetNumber++ + " " + port;
                        case "exit":
                            files.Remove(int.Parse(splitted[1]));
                            return "EXIT";
                        default:
                            return "Nieprawidlowa komenda";
                    }
                }
                return "Cos poszlo nie tak";
            }
            return "Cos poszlo nie tak";
        }
        void RemoveTCPCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                TCPcommunicators.Remove(communicator);
            }
        }

       public void AddUDPListener(IListener listener)
        {
            lock (_lockListener)
            {
                UDPListeners.Add(listener);
                if (serverStatus == ServerStatus.Running)
                {
                    listener.Start(new CommunicatorD(AddUDPCommunicator));
                }
            }
        }

        void AddUDPCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                UDPcommunicators.Add(communicator);
                if (serverStatus == ServerStatus.Running)
                {
                    communicator.Start(new CommandD(GetUDPAnswer), RemoveUDPCommunicator);
                }
            }
        }

        private string GetUDPAnswer(string command)
        {
            if (path != null)
            {
                string[] splitted = command.Split();
                if (splitted.Length > 1)
                {
                    switch (splitted[0])
                    {
                        case "get":
                            if (files[int.Parse(splitted[1])] != null)
                            {
                                double packLength = files[int.Parse(splitted[1])].Length / PackSize;
                                var sb = new StringBuilder();
                                for (int i = 0; i < packLength; i++)
                                {
                                    sb.Append(i + 1 + " " + files[int.Parse(splitted[1])].Substring((int)(i * PackSize), (int)PackSize) + Environment.NewLine);
                                }
                                Console.WriteLine("Ilość paczek: " + packLength);
                                return sb.ToString();
                            }
                            return "OK";
                        case "getpacket":
                            if(files[int.Parse(splitted[1])] != null)
                            {
                                int packetNumber = int.Parse(splitted[2]);
                                return packetNumber + " " + files[int.Parse(splitted[1])].Substring((int)(packetNumber * PackSize), (int)PackSize) + Environment.NewLine;
                            }
                            return "EXIT";
                        default:
                            return "Nieprawidlowa komenda";
                    }
                }
                return "Cos poszlo nie tak";
            }
            return "Cos poszlo nie tak";
        }
        void RemoveUDPCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                UDPcommunicators.Remove(communicator);
            }
        }
        void Start()
        {
            for (int i = 0; i < TCPListeners.Count; i++)
            {
                TCPListeners[i].Start(new CommunicatorD(AddTCPCommunicator));
            }
            for (int i = 0; i < UDPListeners.Count; i++)
            {
                UDPListeners[i].Start(new CommunicatorD(AddUDPCommunicator));
            }
            serverStatus = ServerStatus.Running;
        }
        static void Main(string[] args)
        {
            Server server = new Server();
            server.path = "test";
            server.Start();
            server.AddTCPListener(new protocols.TCPL(IPAddress.Any, port));
            while (true);
        }
    }
}
