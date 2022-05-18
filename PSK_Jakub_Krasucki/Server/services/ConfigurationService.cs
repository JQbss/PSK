using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.services
{
    public class ConfigurationService : IServiceModule
    {
        public delegate void AddListenerD(IListener listener);
        public delegate void RemoveListenerD(IListener listener);
        public delegate void AddServiceD(string name, IServiceModule service);
        public delegate void RemoveServiceModuleD(string name);

        private AddListenerD addListenerD;
        private RemoveListenerD removeListenerD;
        private AddServiceD addServiceD;
        private RemoveServiceModuleD removeServiceModuleD;

        public ConfigurationService(AddListenerD addListenerD, RemoveListenerD removeListenerD, 
            AddServiceD addServiceD, RemoveServiceModuleD removeServiceModuleD)
        {
            this.addListenerD = addListenerD;
            this.removeListenerD = removeListenerD;
            this.addServiceD = addServiceD;
            this.removeServiceModuleD = removeServiceModuleD;
        }

        public string AnswerCommand(string command)
        {
            string[] splitted = command.Trim().Split();
            switch (splitted[1])
            {
                case "addlistener":
                    return AddListener(splitted);
                case "removelistener":
                    return RemoveListener(splitted);
                case "addservice":
                    return AddService(splitted);
                case "removeservice":
                default:
                    return "Nieprawidłowa komenda";
            }
        }
        private string AddListener(string[] splitted)
        {
            String s = "";
            for (int i = 3; i < splitted.Length; i++)
            {
                s+=splitted[i] + " ";
            }
            s=s.Trim();
            Type t = Type.GetType("Server.protocols." + splitted[2], false,true);

            object o = Activator.CreateInstance(t, s);
            addListenerD((IListener)o);
            return "Dodano nowy listener";
        }

        private string RemoveListener(string[] splitted)
        {
            String s = "";
            for (int i = 3; i < splitted.Length; i++)
            {
                s += splitted[i] + " ";
            }
            s = s.Trim();
            Type t = Type.GetType("Server.protocols." + splitted[2], false, true);
            object o = Activator.CreateInstance(t, s);
            removeListenerD((IListener)o);
            return "Usunieto listener";
        }

        private string AddService(string[] splitted)
        {
            Type t = Type.GetType("Server.services." + splitted[2], false, true);
            object o = Activator.CreateInstance(t);
            addServiceD(splitted[3],(IServiceModule)o);
            return "Dodano serwis";
        }
    }
}
