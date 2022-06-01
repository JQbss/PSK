using Server.enums;
using Server.services;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        ServerStatus serverStatus = ServerStatus.New;
        Dictionary<string, IServiceModule> services = new Dictionary<string, IServiceModule>();
        List<IListener> listeners = new List<IListener>();
        List<ICommunicator> communicators = new List<ICommunicator>();
        public bool IsServerStarted { get; set; }
        private object _lockCommunicator = new object();
        private object _lockListener = new object();

        public Server() { }
        void AddServiceModule(string name, IServiceModule service)
        {
            services.Add(name, service);
        }
        void AddCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                communicators.Add(communicator);
                if (serverStatus == ServerStatus.Running)
                {
                    communicator.Start(new CommandD(GetServiceName), RemoveCommunicator);
                }
            }
        }

        void AddListener(IListener listener)
        {
            lock(_lockListener)
            {
                listeners.Add(listener);
                if(serverStatus == ServerStatus.Running)
                {
                    listener.Start(new CommunicatorD(AddCommunicator));
                }
            }
           
        }

        void RemoveServiceModule(string name)
        {
            services.Remove(name);
        }

        void RemoveCommunicator(ICommunicator communicator)
        {
            lock (_lockCommunicator)
            {
                communicators.Remove(communicator);
            }
        }

        void RemoveListener(IListener listener)
        {
            lock (_lockListener)
            {
                listeners.Remove(listener);
            }
        }
        private string GetServiceName(string command)
        {
            string serviceName = command.Split()[0];
            if (services.ContainsKey(serviceName))
            {
                return services[serviceName].AnswerCommand(command);
            }
            return "Brak mozliwosci polaczenia z serwerem.\n";
        }
        void Start()
        {
            AddServiceModule("config", new ConfigurationService(
                    new ConfigurationService.AddListenerD(AddListener),
                    new ConfigurationService.RemoveListenerD(RemoveListener),
                    new ConfigurationService.AddServiceD(AddServiceModule),
                    new ConfigurationService.RemoveServiceModuleD(RemoveServiceModule)));
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].Start(new CommunicatorD(AddCommunicator));
            }
            serverStatus = ServerStatus.Running;
        }
        void Stop()
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                listeners[i].Stop();
            }
            for(int i = 0; i < communicators.Count; i++)
            {
                communicators[i].Stop();
            }
            communicators.Clear();
            services.Clear();
        }
        static void Main()
        {
            Server server = new Server();
            server.AddServiceModule("ping", new PingService());
            server.AddServiceModule("chat", new ChatService());
            server.AddServiceModule("ftp", new FtpService());
            server.Start();
            server.AddListener(new protocols.TCPL(IPAddress.Any, 12345));
            server.AddListener(new protocols.UDPL(IPAddress.Any, 12346));
            server.AddListener(new protocols.NetRemotingL("30000"));
            server.AddListener(new protocols.RS232L(new SerialPort("COM2", 9600, Parity.None, 8, StopBits.One)));
            server.AddListener(new protocols.FilesManagerL(@"../../../FileMedium"));
            while (true);
            
        }
    }
}
