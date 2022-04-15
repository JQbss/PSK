using Server.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class Client
    {
        static void Main(string[] args)
        {
            StartTCP();
        }

        static void StartTCP()
        {
            while(true)
            {
                string server = "localhost";
                int port = 12345;
                TcpClient client = new TcpClient(server, port);
                NetworkStream stream = client.GetStream();

                while (true)
                {
                    string message = Ping.Query(1024,1024);

                    byte[] data = Encoding.ASCII.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Wysłane: {0}", message);

                    byte[] response = new byte[256];
                    string responseStr = string.Empty;
                    int bytes;
                    do
                    {
                        bytes = stream.Read(response, 0, response.Length);
                        responseStr += Encoding.ASCII.GetString(response, 0, bytes);
                    } while (stream.DataAvailable);
                    Console.WriteLine("Pobrane: {0}", responseStr);
                }
        
                client.Close();
            }
        }
    }
}
