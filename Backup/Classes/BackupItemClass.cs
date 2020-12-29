using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backup
{
    public class BackupItem
    {
        public bool IsEnabled { get; set; }
        public string Path { get; set; }
        public bool IsFile { get; set; }
    }
}
