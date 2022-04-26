using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.services
{
    internal class FtpService : IServiceModule
    {
        const string path = "PSK";

        public FtpService()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string AnswerCommand(string command)
        {
            string[] splitted = command.Split();
            switch (splitted[1])
            {
                case "send":
                    return SendFile(splitted);
                case "get":
                    return GetFile(splitted);
                case "list":
                    return FileList();
                default:
                    return "Nieprawidłowa komenda";
            }
        }

        private string FileList()
        {

            string[] list = Directory.GetFiles(path);
            return "Lista plikow:\n" + string.Join("\n", list)+"\n";
        }

        private string GetFile(string[] splitted)
        {
            string file = Utils.Base64Converter.FileToBase64(path + "\\" + splitted[2]);
            if (file != null)
            {
                return file;
            }
            else
            {
                return "Brak pliku";
            }
        }
        private string SendFile(string[] splitted)
        {
            Utils.Base64Converter.Base64ToFile(splitted[3], path + "\\" + splitted[2]);
            return "Udalo sie zapisac plik";
        }
    }
}
