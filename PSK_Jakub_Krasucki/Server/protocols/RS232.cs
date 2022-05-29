using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.protocols
{
    public class RS232L : IListener
    {
        private SerialPort serialPort;

        public RS232L(SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }

        public RS232L(string config)
        {
            if (config != "")
            {
                string[] splitted = config.Split(' ');
                serialPort = new SerialPort(splitted[0]);
            }
        }

        public void Start(CommunicatorD onConnect)
        {
            if(serialPort != null)
            {
                onConnect(new RS232C(serialPort));
            }
        }

        public void Stop()
        {
            if (serialPort != null)
            {
                serialPort.Close();
            }
        }
    }

    public class RS232C : ICommunicator
    {
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private SerialPort serialPort;

        public RS232C(SerialPort serialPort)
        {
            this.serialPort=serialPort;
            this.serialPort.DataReceived += new SerialDataReceivedEventHandler(Target);
        }

        private void Target(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort serialPort = (SerialPort)sender;
            string command = serialPort.ReadLine();
            serialPort.WriteLine(command);
        }

        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand=onCommand;
            this.onDisconnect=onDisconnect;
            serialPort.Open();
        }

        public void Stop()
        {
            serialPort.Close();
        }
    }
}
