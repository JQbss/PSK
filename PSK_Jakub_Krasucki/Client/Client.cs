using Server.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string server = "localhost";
            int port = 12345;
            using (TcpClient client = new TcpClient(server, port))
            {
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    Console.WriteLine("Wprowadź komende");
                    string command = Console.ReadLine();
                    if (command == "exit")
                    {
                        break;
                    }
                    else
                    {
                        string[] splitted = command.Split();
                        string message = Ping.Query(int.Parse(splitted[1]), int.Parse(splitted[1]));
                        byte[] data = Encoding.ASCII.GetBytes(message);
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        stream.Write(data, 0, data.Length);
                        byte[] response = new byte[256];
                        string responseStr = string.Empty;
                        int bytes;
                        do
                        {
                            bytes = stream.Read(response, 0, response.Length);
                            responseStr += Encoding.ASCII.GetString(response, 0, bytes);
                        } while (stream.DataAvailable);
                        stopwatch.Stop();
                        Console.WriteLine("Ping " + stopwatch.ElapsedMilliseconds);
                    }
                }
                stream.Close();
            }
           
        }
    }
}
