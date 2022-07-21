using Client.medium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class Command
    {
        public static string SendCommand(string command)
        {
            if (command.Contains("getfile"))
            {
                Config.fileName = command.Split()[1];

            }
            if (command.Contains("end"))
            {
                command += " " + Config.taskId;
            }
            return command;
        }
        public static bool Response(string message)
        {
            if (message.ToLower().Contains("error"))
            {
                Console.WriteLine(message);
                return false;
            }

            if (message.ToLower().Contains("end success"))
            {
                Console.WriteLine(message);
                return true;
            }

            string[] tmp = message.Split();

            if (tmp.Length > 3)
            {
                int fileSize = int.Parse(tmp[1]);
                int packSize = int.Parse(tmp[2]);
                Config.taskId = int.Parse(tmp[3]);

                MUDP.Start(fileSize, packSize, Config.taskId);
            }
            else Console.WriteLine("Otrzymano błędne dane!");
            return false;
        }
    }
}
