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
                case "addservice":
                case "removeservice":
                default:
                    return "Nieprawidłowa komenda";
            }
        }
        //c addlistener name listenertype address ip
        private string AddListener(string[] splitted)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 3; i < splitted.Length; i++) sb.Append(splitted[i] + " ");
            Type t = typeof(AddListenerD);
            object o = Activator.CreateInstance(t, sb);
            addListenerD((IListener)o);
            return "Dodano nowy listener";
        }
    }
}
