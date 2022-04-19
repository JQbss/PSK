using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class MessageInformation
    {
        private string reciver;
        private string sender;
        private string message;
        public string Reciver
        {
            get { return reciver; }
            set { reciver = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string Sender
        {
            get { return sender; }
            set { sender = value; }
        }


        public MessageInformation(string reciver, string sender, string message)
        {
            this.reciver = reciver;
            this.sender = sender;
            this.message = message;
        }
    }
}
