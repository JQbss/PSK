using Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.services
{
    internal class PingService : IServiceModule
    {
        public string AnswerCommand(string command)
        {
            return Ping.Pong(command);
        }
    }
}
