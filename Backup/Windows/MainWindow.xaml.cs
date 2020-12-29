using Backup.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Backup
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Список элементов для отображения в DataGrid
        /// </summary>
        private readonly List<BackupItem> dataGridSourse = new List<BackupItem>();
        
        public MainWindow()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Count() > 1)
                if (File.Exists(args[1]))
                    OpenFileShell(args[1]);
            dataGrid_BackupList.ItemsSource = dataGridSourse;
        }

        /// <summary>
        /// Открытие файла списка
        /// </summary>
        private void MenuItem_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog
            {
                DefaultExt = "baclist",
                Filter = $"{Application.Current.Resources.MergedDictionaries[0]["mw_InfoListBackupExt"]}|*.baclist"
            })
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    OpenFileShell(ofd.FileName);
            }
        }

        /// <summary>
        /// Сохранение списка
        /// </summary>
        private void MenuItem_SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog
                {
                    DefaultExt = "baclist",
                    Filter = $"{Application.Current.Resources.MergedDictionaries[0]["mw_InfoListBackupExt"]}|*.baclist"
                })
                {
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        using (StreamWriter streamWriter = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            for (int i = 0; i < dataGridSourse.Count; i++)
                            {
                                streamWriter.WriteLine("BI-" + i + " {");
                                streamWriter.WriteLine($"Enabled={dataGridSourse[i].IsEnabled}");
                                streamWriter.WriteLine($"Path={dataGridSourse[i].Path}");
                                streamWriter.WriteLine($"File={dataGridSourse[i].IsFile}");
                                streamWriter.WriteLine("}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Добавление элемента
        /// </summary>
        private void MenuItem_AddBackupItem_Click(object sender, RoutedEventArgs e)
        {
            AddBackupItemWindow addBackupItemWindow = new AddBackupItemWindow();
            addBackupItemWindow.ShowDialog();
            if (addBackupItemWindow.AddBackupItem)
            {
                dataGridSourse.Add(addBackupItemWindow.BackupItem);
                dataGrid_BackupList.Items.Refresh();
            }
        }

        /// <summary>
        /// Удаление элемента
        /// </summary>
        private void MenuItem_DeleteBackupItem_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGrid_BackupList.SelectedIndex;
            if (index != -1)
            {
                if (MessageBox.Show(string.Format((string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoDelete"], dataGridSourse[index].Path), Title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    dataGridSourse.RemoveAt(index);
                    dataGrid_BackupList.Items.Refresh();
                }
            }
        }

        /// <summary>
        /// Запуск резервирования
        /// </summary>
        private async void MenuItem_StartBackup_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridSourse.Count != 0)
            {
                using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog
                {
                    DefaultExt = "*.zip",
                    Filter = "Zip|*.zip"
                })
                {
                    if (!App.Settings.StandartMode)
                        sfd.Filter += "|Rar|*.rar";
                    sfd.FileName = DateTime.Now.ToString("dd'-'MM'-'yyyy");
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string logFile = $"{Path.GetDirectoryName(sfd.FileName)}{Path.GetFileNameWithoutExtension(sfd.FileName)}.txt";
                        if (App.Settings.StandartMode)
                            await Task.Run(() => StartStandartBackup(sfd.FileName, logFile));
                        else
                            await Task.Run(() => StartWinRarBackup(sfd.FileName, logFile));
                    }
                }
            }
        }

        /// <summary>
        /// Открытие настроек
        /// </summary>
        private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void MenuItem_BackupOnOff_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGrid_BackupList.SelectedIndex;
            if (index != -1)
            {
                dataGridSourse[index].IsEnabled = !dataGridSourse[index].IsEnabled;
                dataGrid_BackupList.Items.Refresh();
            }
        }

        private void DataGrid_BackupList_MouseDoubleClick(object sender, MouseButtonEventArgs e) =>
            MenuItem_BackupOnOff_Click(null, null);

        private void MenuItem_SortByPath_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridSourse.Count != 0)
            {
                dataGridSourse.Sort((x, y) => x.Path.CompareTo(y.Path));
                dataGrid_BackupList.Items.Refresh();
            }
        }

        private void MenuItem_SortByMarkBackup_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridSourse.Count != 0)
            {
                dataGridSourse.Sort((x, y) => y.IsEnabled.CompareTo(x.IsEnabled));
                dataGrid_BackupList.Items.Refresh();
            }
        }

        /// <summary>
        /// Обрабатываем горячие клавиши
        /// </summary>
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
                MenuItem_OpenFile_Click(null, null);
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
                MenuItem_SaveToFile_Click(null, null);
            if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
                MenuItem_AddBackupItem_Click(null, null);
            if ((e.Key == Key.D && Keyboard.Modifiers == ModifierKeys.Control) || e.Key == Key.Delete)
                MenuItem_DeleteBackupItem_Click(null, null);
            if (e.Key == Key.R && Keyboard.Modifiers == ModifierKeys.Control)
                MenuItem_StartBackup_Click(null, null);
            if (e.Key == Key.M && Keyboard.Modifiers == ModifierKeys.Control)
                MenuItem_BackupOnOff_Click(null, null);
            if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
                Close();
        }

        /// <summary>
        /// Оболочка открытия файла
        /// </summary>
        private void OpenFileShell(string filepath)
        {
            try
            {
                dataGridSourse.Clear();
                using (StreamReader streamReader = new StreamReader(filepath, Encoding.UTF8, true))
                {
                    while (!streamReader.EndOfStream)
                    {
                        streamReader.ReadLine();
                        BackupItem backupItem = new BackupItem
                        {
                            IsEnabled = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 8)),
                            Path = streamReader.ReadLine().Remove(0, 5),
                            IsFile = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 5))
                        };
                        dataGridSourse.Add(backupItem);
                        streamReader.ReadLine();
                    }
                }
            }
            catch (Exception ex)
            {
                dataGridSourse.Clear();
                MessageBox.Show(ex.Message, Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            dataGrid_BackupList.Items.Refresh();
        }

        /// <summary>
        /// Стандартный режим 
        /// </summary>
        private void StartStandartBackup(string archivePath, string logPath)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    grid_General.IsEnabled = false;
                    progressBar_State1.Value = 0;
                    progressBar_State2.Value = 0;
                    progressBar_State2.Visibility = Visibility.Hidden;
                    progressBar_State1.IsIndeterminate = true;
                    textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoPreparing"];
                    textBlock_State2.Text = string.Empty;
                    grid_Progress.Visibility = Visibility.Visible;
                }));
                CompressionLevel compresLevel = CompressionLevel.Fastest;
                if (App.Settings.CompressLevel == 0)
                    compresLevel = CompressionLevel.NoCompression;
                if (App.Settings.CompressLevel == 2)
                    compresLevel = CompressionLevel.Optimal;
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    progressBar_State1.IsIndeterminate = false;
                    progressBar_State1.Maximum = dataGridSourse.Count;
                    progressBar_State2.Visibility = Visibility.Visible;
                }));
                using (FileStream archiveStream = new FileStream(archivePath, FileMode.Create, FileAccess.Write))
                {
                    using (ZipArchive archive = new ZipArchive(archiveStream, ZipArchiveMode.Create))
                    {
                        Log log = new Log(logPath);
                        for (int i = 0; i < dataGridSourse.Count; i++)
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State1.Text = dataGridSourse[i].Path));
                            string[] fileList;
                            if (dataGridSourse[i].IsFile)
                                fileList = new string[] { dataGridSourse[i].Path };
                            else
                                fileList = Directory.GetFiles(dataGridSourse[i].Path, "*", SearchOption.AllDirectories);
                            Application.Current.Dispatcher.Invoke((Action)(() => progressBar_State2.Maximum = fileList.Length));
                            for (int i1 = 0; i1 < fileList.Length; i1++)
                            {
                                Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State2.Text = fileList[i1]));
                                archive.CreateEntryFromFile(fileList[i1], fileList[i1].Remove(0, 3), compresLevel);
                                log.WriteLine(fileList[i1]);
                                Application.Current.Dispatcher.Invoke((Action)(() => progressBar_State2.Value++));
                            }
                            Application.Current.Dispatcher.Invoke((Action)(() => progressBar_State1.Value++));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (App.Settings.CloseAfterBackup)
                        Close();
                    else
                    {
                        progressBar_State1.IsIndeterminate = false;
                        grid_Progress.Visibility = Visibility.Hidden;
                        grid_General.IsEnabled = true;
                    }
                }));
            }
        }

        /// <summary>
        /// Режим с использованием WinRar
        /// </summary>
        private void StartWinRarBackup(string archivePath, string logPath)
        {
            try
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    grid_General.IsEnabled = false;
                    progressBar_State1.Value = 0;
                    progressBar_State2.Value = 0;
                    progressBar_State2.Visibility = Visibility.Hidden;
                    progressBar_State1.IsIndeterminate = true;
                    textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoPreparing"];
                    textBlock_State2.Text = string.Empty;
                    grid_Progress.Visibility = Visibility.Visible;
                }));
                //Создание файла списка
                string tempFile = $"{Path.GetTempPath()}\\Backup_WinRarList.txt";
                using (StreamWriter streamWriter = new StreamWriter(tempFile, false, Encoding.UTF8))
                {
                    Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoWinRarCreateList"]));
                    for (int i = 0; i < dataGridSourse.Count; i++)
                        if (dataGridSourse[i].IsEnabled)
                            streamWriter.WriteLine(dataGridSourse[i].Path);
                }
                Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoWinRarPreparing"]));
                string args = "A -ed ";
                if (App.Settings.WriteLog)
                    args += $"-logafpu=\"{logPath}\"";
                if (App.Settings.CompressLevel == 0)
                    args += " -m0 ";
                if (App.Settings.CompressLevel == 1)
                    args += " -m3 ";
                if (App.Settings.CompressLevel == 2)
                    args += " -m5 ";
                args += $"\"{archivePath}\" @\"{tempFile}\"";
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = App.Settings.WinRarExePath;
                    process.StartInfo.Arguments = args;
                    Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoWinRarRun"]));
                    process.Start();
                    Application.Current.Dispatcher.Invoke((Action)(() => textBlock_State1.Text = (string)Application.Current.Resources.MergedDictionaries[0]["mw_InfoWinRarRunning"]));
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (App.Settings.CloseAfterBackup)
                        Close();
                    else
                    {
                        progressBar_State1.IsIndeterminate = false;
                        grid_Progress.Visibility = Visibility.Hidden;
                        grid_General.IsEnabled = true;
                    }
                }));
            }
        }
    }
}