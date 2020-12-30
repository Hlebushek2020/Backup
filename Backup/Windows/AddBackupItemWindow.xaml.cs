using System.Windows;
using System.Windows.Input;

namespace Backup
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class AddBackupItemWindow : Window
    {
        /// <summary>
        /// Словарь ресурсов для локализации
        /// </summary>
        private readonly ResourceDictionary localization = Application.Current.Resources.MergedDictionaries[0];

        private string biPath;
        private bool biIsFile;

        public BackupItem BackupItem { get; private set; } = null;

        public AddBackupItemWindow()
        {
            InitializeComponent();
        }

        private void Button_SelectFile_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog {
                Filter = $"{localization["abiw_InfoAllFiles"]}|*.*"
            })
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    biPath = ofd.FileName;
                    biIsFile = true;
                    textBlock_Path.Text = string.Format((string)localization["abiw_InfoSelectFileName"], biPath);
                }
            }
        }

        private void Button_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    biPath = fbd.SelectedPath;
                    biIsFile = false;
                    textBlock_Path.Text = string.Format((string)localization["abiw_InfoSelectDirectoryPath"], biPath);
                }
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) => 
            Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(biPath) == false || checkBox_Backup.IsChecked == true)
            {
                //BackupItem.IsEnabled = checkBox_Backup.IsChecked.Value;
                if (MessageBox.Show((string)localization["abiw_InfoSave"], this.Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(biPath))
                    {
                        e.Cancel = true;
                        MessageBox.Show((string)localization["abiw_InfoSaveWarning"], this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);            
                    }
                    else
                        BackupItem = new BackupItem(checkBox_Backup.IsChecked.Value, biIsFile, biPath);
                }
            }
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
                Close();
        }
    }
}
