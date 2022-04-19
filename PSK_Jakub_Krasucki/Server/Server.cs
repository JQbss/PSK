﻿using Server.enums;
using Server.services;
using System;
using System.Collections.Generic;
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
            return "Brak możliwości połaczenia z serwerem.\n";
        }
        void Start()
        {
            for(int i = 0; i < listeners.Count; i++)
            {
                listeners[i].Start(new CommunicatorD(AddCommunicator));
            }
            serverStatus = ServerStatus.Running;
        }
        void WaitForStop()
        {
            while (communicators.Count != 0)
            {
                Thread.Sleep(1000);
            }
            while (services.Count != 0)
            {
                Thread.Sleep(1000);
            }
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
            server.Start();
            server.AddListener(new protocols.TCPL(IPAddress.Any, 12345));
            server.WaitForStop();
            server.Stop();
        }
    }
}
