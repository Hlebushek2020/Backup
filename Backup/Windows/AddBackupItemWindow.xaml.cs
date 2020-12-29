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

        /// <summary>
        /// Объект для добавления в список
        /// </summary>
        public BackupItem BackupItem { get; private set; } = new BackupItem();
        /// <summary>
        /// Флаг указывающий добавлять элемент или нет
        /// </summary>
        public bool AddBackupItem { get; private set; } = false;

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
                    BackupItem.Path = ofd.FileName;
                    BackupItem.IsFile = true;
                    textBlock_Path.Text = string.Format((string)localization["abiw_InfoSelectFileName"], BackupItem.Path);
                }
            }
        }

        private void Button_SelectDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    BackupItem.Path = fbd.SelectedPath;
                    BackupItem.IsFile = false;
                    textBlock_Path.Text = string.Format((string)localization["abiw_InfoSelectDirectoryPath"], BackupItem.Path);
                }
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) => 
            Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(BackupItem.Path) == false || checkBox_Backup.IsChecked == true)
            {
                BackupItem.IsEnabled = checkBox_Backup.IsChecked.Value;
                if (MessageBox.Show((string)localization["abiw_InfoSave"], this.Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (string.IsNullOrEmpty(BackupItem.Path))
                    {
                        e.Cancel = true;
                        MessageBox.Show((string)localization["abiw_InfoSaveWarning"], this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);            
                    }
                    else
                        AddBackupItem = true;
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
