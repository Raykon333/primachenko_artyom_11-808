using System;
using System.IO;
using System.Linq;
namespace MailWorker
{
    public class FileWorker
    {
        public string FileDirectory {get;}
        public FileWorker(string directory)
        {
            FileDirectory = directory;
        }

        public string[] GetIncomingMessages(string address)
        {
            string incomingDir = FileDirectory + @"\" + address + @"\Incoming";
            return Directory
                .GetFiles(incomingDir)
                .Select(file => File.ReadAllText(incomingDir + @"\" + file))
                .ToArray();
        }

        public string[] GetOutcomingMessages(string address)
        {
            string outcomingDir = FileDirectory + @"\" + address + @"\Outcoming";
            return Directory
                .GetFiles(outcomingDir)
                .Select(file => File.ReadAllText(outcomingDir + @"\" + file))
                .ToArray();
        }

        /*public string [] GetSpamMessages(string address)
        {
            string spamDir = FileDirectory + @"\" + address + @"\Spam";
            return Directory
                .GetFiles(spamDir)
                .Select(file => File.ReadAllText(spamDir + @"\" + file))
                .ToArray();
        }*/
    }
}
