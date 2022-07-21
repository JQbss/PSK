using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class Base64Converter
    {
        public static string FileToBase64(string path)
        {
            if (File.Exists(path))
            {
                FileStream fileStream = new FileStream(path, FileMode.Open);
                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, buffer.Length);
                string base64 = Convert.ToBase64String(buffer);
                fileStream.Close();
                return base64;
            }
            return null;
        }

        public static void Base64ToFile(string file, string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            byte[] buffer = Convert.FromBase64String(file);
            fileStream.Write(buffer, 0, buffer.Length);
            fileStream.Close();
        }
    }
}