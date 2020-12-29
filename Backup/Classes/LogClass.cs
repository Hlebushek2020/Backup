using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backup.Classes
{
    public class Log
    {
        private string logFile;

        public Log(string logFile) => this.logFile = logFile;

        public void WriteLine(string message)
        {
            if (App.Settings.WriteLog)
                using (StreamWriter sw = new StreamWriter(logFile, true, Encoding.UTF8))
                    sw.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
        }
    }
}