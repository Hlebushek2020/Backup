using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Backup
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// Словарь ресурсов для локализации
        /// </summary>
        private readonly ResourceDictionary localization = Application.Current.Resources.MergedDictionaries[0];

        public SettingsWindow()
        {
            InitializeComponent();
            if (Directory.Exists($"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language"))
            {
                string[] langs = Directory.GetFiles($"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language", "*.xaml");
                for (int i = 0; i < langs.Length; i++)
                    comboBox_Language.Items.Add(Path.GetFileNameWithoutExtension(langs[i]).ToLower());
            }
            comboBox_Language.SelectedItem = App.Settings.Language;
            string[] items = ((string)localization["sw_CompressLevels"]).Split(';');
            comboBox_CompessLevel.Items.Add(items[0]);
            comboBox_CompessLevel.Items.Add(items[1]);
            comboBox_CompessLevel.Items.Add(items[2]);
            comboBox_CompessLevel.SelectedIndex = 1;
            checkBox_WriteLog.IsChecked = App.Settings.WriteLog;
            checkBox_CloseAfterBackup.IsChecked = App.Settings.CloseAfterBackup;
            comboBox_CompessLevel.SelectedIndex = App.Settings.CompressLevel;
            if (App.Settings.StandartMode)
                radioButton_StandartMode.IsChecked = true;
            else
                radioButton_WinRarMode.IsChecked = true;
            textBlock_WinRarPath.Text = App.Settings.WinRarExePath;
        }

        private void RadioButton_WinRarMode_Checked(object sender, RoutedEventArgs e)
        {
            groupBox_WinRar.IsEnabled = true;
            groupBox_WinRar.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void RadioButton_StandartMode_Checked(object sender, RoutedEventArgs e)
        {
            groupBox_WinRar.IsEnabled = false;
            groupBox_WinRar.Foreground = new SolidColorBrush(Colors.Silver);
        }

        private void Button_WinRarPath_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog {
                Filter = $"{localization["sw_InfoExe"]}|*.exe",
                DefaultExt = "exe"
            })
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    textBlock_WinRarPath.Text = ofd.FileName;
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e) => 
            Close();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string language = App.Settings.Language;
            if (comboBox_Language.SelectedItem != null)
                language = comboBox_Language.SelectedItem.ToString();
            if (radioButton_StandartMode.IsChecked.Value != App.Settings.StandartMode ||
                checkBox_WriteLog.IsChecked.Value != App.Settings.WriteLog ||
                textBlock_WinRarPath.Text != App.Settings.WinRarExePath ||
                comboBox_CompessLevel.SelectedIndex != App.Settings.CompressLevel ||
                checkBox_CloseAfterBackup.IsChecked.Value != App.Settings.CloseAfterBackup ||
                language != App.Settings.Language)
            {
                if (MessageBox.Show((string)localization["sw_InfoSaveChanges"], Title, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (radioButton_StandartMode.IsChecked.Value == false
                        && string.IsNullOrEmpty(textBlock_WinRarPath.Text))
                    {
                        e.Cancel = true;
                        MessageBox.Show((string)localization["sw_InfoWarningExe"], this.Title, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    App.Settings.CloseAfterBackup = checkBox_CloseAfterBackup.IsChecked.Value;
                    App.Settings.StandartMode = radioButton_StandartMode.IsChecked.Value;
                    App.Settings.CompressLevel = comboBox_CompessLevel.SelectedIndex;
                    App.Settings.WriteLog = checkBox_WriteLog.IsChecked.Value;
                    App.Settings.WinRarExePath = textBlock_WinRarPath.Text;
                    App.Settings.Language = language;
                    App.Settings.Save();
                }
            }
        }

        private void Button_Extension_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (((Button)sender).Tag.ToString() == "0")
                //{
                //    string iconPath = $"{Path.GetDirectoryName(Application.ResourceAssembly.Location)}\\ExtensionIco.ico";
                //    if (File.Exists(iconPath) == false)
                //        iconPath = null;
                //    Registry.AssociateExtension(".baclist", App.ProgramName, Application.ResourceAssembly.Location, iconPath);
                //}
                //else
                //    Registry.RemoveAssociateExtension(".baclist", App.ProgramName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Выравниваем элементы в основном GroupBox
        /// </summary>
        private void Grid_Main_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox_Language.Margin = new Thickness(label_Language.ActualWidth + 15, comboBox_Language.Margin.Top, 0, 0);
            comboBox_CompessLevel.Margin = new Thickness(label_CompressLevel.ActualWidth + 15, comboBox_CompessLevel.Margin.Top, 0, 0);
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
                Close();
        }
    }
}