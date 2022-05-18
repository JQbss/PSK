using Client.mediums;
using Server.utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    
    public class Client
    {
        static string server = "localhost";
        static int port = 12345;
        private static void PingCommand(Medium medium, string[] splitted)
        {
            int numberOfPings = splitted.Length > 3 ? int.Parse(splitted[3]) : 1;
            List<TimeSpan> times = new List<TimeSpan>();
            for (int i = 0; i < numberOfPings; i++)
            {
                string request = Ping.Query(int.Parse(splitted[1]), int.Parse(splitted[1]));
                Stopwatch stopwatch = Stopwatch.StartNew();
                string response = medium.QA(request);
                stopwatch.Stop();
                times.Add(stopwatch.Elapsed);
                Console.Write(stopwatch.Elapsed);
                Console.WriteLine(" bytes:"+Encoding.ASCII.GetByteCount(response));
                Thread.Sleep(1000);
            }
            Console.Write("Średni czas: ");
            Console.WriteLine(AvargeTime(times));
        }

        private static void ChatCommand(Medium medium, string command)
        {
            string response = medium.QA(command);
            Console.WriteLine(response);
        }
        private static void FtpCommand(Medium medium, string command)
        {
            string response = medium.QA(command);
            Console.WriteLine(response);
        }

        private static void ConfigCommand(Medium medium, string command)
        {
            string response = medium.QA(command);
            Console.WriteLine(response);
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Wybierz medium");
                Console.WriteLine("---------------");
                Console.WriteLine("1 - TCP");
                Console.WriteLine("Default - TCP");
                Console.WriteLine("---------------");
                int protocol = Convert.ToInt32(Console.ReadLine());
                Medium medium;
                switch (protocol)
                {
                    case 1:
                    default:
                        TcpClient client = new TcpClient(server, port);
                        NetworkStream stream = client.GetStream();
                        medium = new MTCP(stream);
                        break;
                }
                Console.WriteLine("Wprowadź komende");
                string command = GetCommand(Console.ReadLine(), medium);
                if (command == "exit")
                {
                    break;
                }
            }

        }

        private static string GetCommand(string command, Medium medium)
        {
            string[] splitted = command.Split();
            if (splitted[0].Equals("ping"))
            {
                PingCommand(medium, splitted);
            }
            else if (splitted[0].Equals("chat"))
            {
                ChatCommand(medium,command);
            }
            else if (splitted[0].Equals("ftp"))
            {
                FtpCommand(medium, command);
            }
            else if (splitted[0].Equals("config"))
            {
                ConfigCommand(medium, command);
            }
            return splitted[0];
        }

        private static TimeSpan AvargeTime(List<TimeSpan> times)
        {
            return TimeSpan.FromMilliseconds(times.Select(s => s.TotalMilliseconds).Average());
        }
    }
}
