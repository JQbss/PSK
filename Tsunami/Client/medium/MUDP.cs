using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.medium
{
    public static class MUDP
    {
        public static void Start(int fileSize, int packSize, int taskID)
        {
            int packLength = fileSize / packSize;
            bool[] received = new bool[packLength];
            string[] receivedStr = new string[packLength];
            for (int i = 0; i < packLength; i++)
            {
                received[i] = false;
                receivedStr[i] = string.Empty;
            }
            Console.WriteLine("Ilosc paczek: " + fileSize / packSize);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(Config.ipAddress), Config.port);
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(ipe);

            Console.WriteLine("Created UDP CONNECTION");

            byte[] data = Encoding.ASCII.GetBytes("get " + taskID);
            udpClient.Send(data, data.Length);

            if (udpClient.Available != 0)
            {
                byte[] receiveBytes = udpClient.Receive(ref ipe);
                string receiveString = Encoding.ASCII.GetString(receiveBytes);
                received[int.Parse(receiveString.Split()[0]) - 1] = true;
                receivedStr[int.Parse(receiveString.Split()[0]) - 1] = receiveString.Split()[1];
            }

            bool success = false;
            while (!success)
            {
                success = true;
                for (int i = 0; i < packLength; i++)
                {
                    if (received[i] == false)
                    {
                        success = false;
                        data = Encoding.ASCII.GetBytes("getpacket " + taskID + " " + i);
                        udpClient.Send(data, data.Length);

                        byte[] receiveBytes = udpClient.Receive(ref ipe);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes);
                        received[i] = true;
                        receivedStr[i] = receiveString.Split()[1];
                    }
                    if (receivedStr[i].Length != packSize)
                    {
                        success = false;
                        data = Encoding.ASCII.GetBytes("getpacket " + taskID + " " + i);
                        udpClient.Send(data, data.Length);

                        byte[] receiveBytes = udpClient.Receive(ref ipe);
                        string receiveString = Encoding.ASCII.GetString(receiveBytes);
                        received[i] = true;
                        receivedStr[i] = receiveString.Split()[1];
                    }
                }
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < packLength; i++)
            {
                result.Append(receivedStr[i]);
            }
            Utils.Base64Converter.Base64ToFile(result.ToString(), Config.fileName);

            Console.WriteLine("Pobieranie zakonczone sukcesem");
            data = Encoding.ASCII.GetBytes("end " + taskID);
            udpClient.Send(data, data.Length);
            udpClient.Close();
        }
    }
}
