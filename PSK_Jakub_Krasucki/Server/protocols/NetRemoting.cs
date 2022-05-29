using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.protocols
{
    public class NetRemotingL : IListener
    {
        private TcpChannel tcpChannel;
        private int port;

        public NetRemotingL(TcpChannel tcpChannel)
        {
            this.tcpChannel = tcpChannel;
        }

        public NetRemotingL(string port)
        {
            tcpChannel = new TcpChannel(int.Parse(port));
            this.port = int.Parse(port);
        }

        public void Start(CommunicatorD onConnect)
        {
            onConnect(new NetRemotingC(tcpChannel));
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(tcpChannel);
        }
    }

    public class NetRemotingC : ICommunicator
    {
        private TcpChannel tcpChannel;

        public NetRemotingC(TcpChannel tcpChannel)
        {
            this.tcpChannel=tcpChannel;
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            ChannelServices.RegisterChannel(tcpChannel, false);
            NetRemotingUtil common = new NetRemotingUtil(new NetRemotingUtil.CommandD(onCommand));
            RemotingServices.Marshal(common, "command");
        }

        public void Stop()
        {
            ChannelServices.UnregisterChannel(tcpChannel);
        }
    }
}
