using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.utils
{
    public class Ping
    {
        public static string Query(int request, int response)
        {
            string query = "ping " + response;
            return query+RandomString(request-query.Length-1) + "\n";
        }
        private static string RandomString(int length)
        {
            Random random = new Random();
            char[] array = new char[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = (char)random.Next(48, 122);
            }
            return new string(array);
        }
        public static string Pong(string line)
        {
            string[] array = line.Split();
            string response = "pong ";
            return response+RandomString(int.Parse(array[1]) - response.Length-2)+"\n";
        }
    }
}
