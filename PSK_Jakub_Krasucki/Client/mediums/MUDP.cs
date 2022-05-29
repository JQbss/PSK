using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.mediums
{
    public class MUDP : Medium
    {
        IPEndPoint ip;
        UdpClient client;
        public MUDP(IPEndPoint ip)
        {
            this.ip = ip;
            client = new UdpClient();
            client.Connect(ip);
        }
        public override string QA(string request)
        {
            byte[] data = Encoding.ASCII.GetBytes(request);
            client.Send(data, data.Length);
            byte[] received = client.Receive(ref ip);
            return Encoding.ASCII.GetString(received);
        }
    }
}
