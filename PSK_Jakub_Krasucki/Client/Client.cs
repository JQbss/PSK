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
                    string command = GetCommand(Console.ReadLine());
                    if (command == "exit")
                    {
                        break;
                    }
                    byte[] data = Encoding.ASCII.GetBytes(command + Environment.NewLine);
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
                    Console.WriteLine(responseStr);
                    Console.WriteLine("\n"+stopwatch.Elapsed);
                    Console.WriteLine("_____________________________");
                }
                stream.Close();
            }
        }

        private static string GetCommand(string command)
        {
            string[] splitted = command.Split();
            if (splitted[0].Equals("ping"))
            {
                return Ping.Query(int.Parse(splitted[1]), int.Parse(splitted[1]));
            }
            else
            {
                return command;
            }
        }
    }
}
