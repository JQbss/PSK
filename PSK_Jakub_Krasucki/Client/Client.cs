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
            while (true)
            {
                Console.WriteLine("Wprowadź komende");
                string command = GetCommand(Console.ReadLine());
                if (command == "exit")
                {
                    break;
                }
                byte[] data = Encoding.ASCII.GetBytes(command + Environment.NewLine);
                Console.WriteLine("Wybierz medium");
                Console.WriteLine("---------------");
                Console.WriteLine("0 - TCP");
                Console.WriteLine("Default - TCP");
                Console.WriteLine("---------------");
                int medium = Convert.ToInt32(Console.ReadLine());
                switch (medium)
                {
                    case 1:
                        StartTCP(data);
                        break;
                    default:
                        StartTCP(data);
                        break;
                }
                
            }
            
        }

        static void StartTCP(byte[] data)
        {
            string server = "localhost";
            int port = 12345;
            TcpClient client = new TcpClient(server, port);
            NetworkStream stream = client.GetStream();
            //pomieszana usługa z medium. Klient wpisuje test ping 10 i rozmiar, daje 10 pingów i 10 odbiera. Dostaje z zewnątrz medium komunikacyjne, mierzy czas.
            //powinna być ogólna funkcja. Ogólna funkcja od realizowania zadań, zaparametryzowaną z medium komunikacyjnym,
            
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
            stream.Close();
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
