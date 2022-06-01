using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.protocols
{
    public class FilesManagerL : IListener
    {
        private string fileName;
        private FileSystemWatcher fileWatcher;
        private CommunicatorD onConnect;

        public FilesManagerL(string fileName)
        {
            this.fileName = fileName;
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            fileWatcher = new FileSystemWatcher(fileName);
        }
        public void Start(CommunicatorD onConnect)
        {
            this.onConnect = onConnect;
            fileWatcher.NotifyFilter = NotifyFilters.LastWrite;
            fileWatcher.Filter = "*.txt";
            fileWatcher.Changed += OnChanged;
            fileWatcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                onConnect(new FilesManagerC(e.FullPath));
            }
        }

        public void Stop()
        {
            fileWatcher.Dispose();
        }
    }

    public class FilesManagerC : ICommunicator
    {
        private CommandD onCommand;
        private CommunicatorD onDisconnect;
        private string fileName;
        public FilesManagerC(string fileName)
        {
            this.fileName = fileName;
        }
        public void Start(CommandD onCommand, CommunicatorD onDisconnect)
        {
            this.onCommand = onCommand;
            this.onDisconnect = onDisconnect;
            StreamReader streamReader = null;
            while (streamReader == null)
            {
                try
                {
                    streamReader = new StreamReader(fileName);
                }
                catch (Exception ex) { }
            }
            
            while (!streamReader.EndOfStream)
            {
                File.WriteAllText(fileName.Replace(".txt", ".csv"), this.onCommand(streamReader.ReadLine()));
            }
            streamReader.Close();
        }

        public void Stop()
        {
            onDisconnect(this);
        }
    }
}
