using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class NetRemotingUtil : MarshalByRefObject
    {
        public delegate string CommandD(string command);
        private CommandD onCommand;
        public NetRemotingUtil(CommandD onCommand)
        {
            this.onCommand = onCommand;
        }
        public string Command(string command)
        {
            if(onCommand == null)
            {
                return onCommand(command);
            }
            return "Err";
        }
    }
}
