using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.mediums
{
    public class MFile : Medium
    {
        private string fileName;
        public MFile(string fileName)
        {
            this.fileName = fileName;
        }
        public override string QA(string request)
        {
            File.WriteAllText(fileName, request);
           
            Thread.Sleep(10);
            StreamReader streamReader = new StreamReader(fileName.Replace(".txt", ".csv"));
            string result = streamReader.ReadToEnd()+"\n";
            streamReader.Close();
            return result;
        }
    }
}
