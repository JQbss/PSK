using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.medium
{
    public static class MTCP
    {
        public static void Start()
        {
            TcpClient client = new TcpClient(Config.host, Config.port);
            NetworkStream stream = client.GetStream();
            bool exitflag = false;
            while (!exitflag)
            {
                Console.WriteLine("Podaj komende");
                byte[] data = Encoding.ASCII.GetBytes(Command.SendCommand(Console.ReadLine()));
                stream.Write(data, 0, data.Length);
                byte[] response = new byte[256];
                StringBuilder responseStr = new StringBuilder();
                int bytes;
                do
                {
                    bytes = stream.Read(response, 0, response.Length);
                    responseStr.Append(Encoding.ASCII.GetString(response, 0, bytes));
                }
                while (stream.DataAvailable);
                exitflag = Command.Response(responseStr.ToString());
            }
        }
    }
}
