using Server.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        Dictionary<string, IServiceModule> services = new Dictionary<string, IServiceModule>();
        List<IListener> listeners = new List<IListener>();
        List<ICommunicator> communicators = new List<ICommunicator>();

        public Server() { }
        void AddServiceModule(string name, IServiceModule service)
        {
            services.Add(name, service);
        }
        //uruchomia komunikator
        void AddCommunicator(ICommunicator communicator)
        {
            communicators.Add(communicator);
        }

        void AddListener(IListener listener)
        {
           listeners.Add(listener);
           listener.Start(new CommunicatorD(AddCommunicator));
        }

        void RemoveServiceModule(string name)
        {
            services.Remove(name);
        }

        void RemoveCommunicator(ICommunicator communicator)
        {
            communicators.Remove(communicator);
        }

        void RemoveListener(IListener listener)
        {
            listeners.Remove(listener);
        }

        void Start()
        {

        }

        void Stop()
        {

        }
        static void Main()
        {
            Server server = new Server();
            server.AddServiceModule("ping", new PingService());
            server.AddListener(new protocols.TCPL(IPAddress.Any, 12345));
        }
    }
}
