using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public delegate string CommandD(string command);
    public delegate void CommunicatorD(ICommunicator commander);
    public interface ICommunicator
    {
        void Start(CommandD onCommand, CommunicatorD onDisconnect);
        void Stop();
    }

    public interface IServiceModule
    {
        string AnswerCommand(string command);
    }

    public interface IListener
    {
        void Start(CommunicatorD onConnect);
        void Stop();
    }
}
