using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Server.services
{
    internal class ChatService : IServiceModule
    {
        private List<MessageInformation> messages = new List<MessageInformation>();
        public string AnswerCommand(string command)
        {
            string[] splitted = command.Split();
            switch (splitted[1])
            {
                case "send":
                    AddMessage(command);
                    return "Dodano wiadomosc";
                case "get":
                    return GetMessage(command);
                default:
                    return "Nieprawidłowa komenda";
            }
        }

        private string GetMessage(string command)
        {
            string reciver = command.Split()[2];
            List<MessageInformation> messagesForUser = messages.Where(m => m.Reciver.Equals(reciver)).ToList();
            string result = string.Empty;
            foreach (MessageInformation message in messagesForUser)
            {
                result += message.Sender + ": " + message.Message;
            }
            if (result == string.Empty)
            {
                return "Brak wiadomosci";
            }
            return result;
        }

        private void AddMessage(string command)
        {
            string[] splitted = command.Split();
            string mess = string.Empty;
            for(int i = 4; i < splitted.Length; i++)
            {
                mess += splitted[i] +" ";
            }
            messages.Add(new MessageInformation(splitted[3], splitted[2], mess));
        }
    }
}
