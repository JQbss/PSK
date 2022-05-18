using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.mediums
{
    public class MTCP : Medium
    {
        private NetworkStream stream;
        public MTCP(NetworkStream stream)
        {
            this.stream = stream;
        }
        public override string QA(string request)
        {
            byte[] data = Encoding.ASCII.GetBytes(request);
            stream.Write(data, 0, data.Length);
            byte[] response = new byte[256];
            string responseStr = string.Empty;
            int bytes;
            do
            {
                bytes = stream.Read(response, 0, response.Length);
                responseStr += Encoding.ASCII.GetString(response, 0, bytes);
            } while (stream.DataAvailable);
            return responseStr;
        }
    }
}
