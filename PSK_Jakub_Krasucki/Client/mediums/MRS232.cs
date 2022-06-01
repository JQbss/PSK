using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.mediums
{
    public class MRS232 : Medium
    {
        SerialPort serialPort;

        public MRS232(SerialPort serialPort)
        {
            this.serialPort = serialPort;
        }
        public override string QA(string request)
        {
            string response = "";
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            
            serialPort.WriteLine(request);
            while (response.Equals(string.Empty))
            {
                response = serialPort.ReadExisting();
            }
            return response;
        }
    }
}
