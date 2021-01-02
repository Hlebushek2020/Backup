using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Backup
{
    public class BackupItem : INotifyPropertyChanged
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool IsFile { get; }

        public string Path { get; }

        public BackupItem(bool isEnabled, bool isFile, string path)
        {
            this.isEnabled = isEnabled;
            IsFile = isFile;
            Path = path;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
