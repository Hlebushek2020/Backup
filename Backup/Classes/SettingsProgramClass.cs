using SergeyCoreNF.File;
using SergeyCoreNF.Registry;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Backup.Classes
{
    public class SettingsProgram
    {
        #region Language
        private string language;
        /// <summary>
        /// Язык
        /// </summary>
        public string Language
        {
            get
            {
                return language;
            }
            set
            {
                string source = $"{Path.GetDirectoryName(App.ResourceAssembly.Location)}\\Language\\{value}.xaml";
                if (File.Exists(source))
                {
                    ResourceDictionary resource = new ResourceDictionary
                    {
                        Source = new Uri(source, UriKind.Absolute)
                    };
                    if (resource.Count == 47)
                    {
                        if (Application.Current.Resources.MergedDictionaries.Count == 0)
                            Application.Current.Resources.MergedDictionaries.Add(resource);
                        else
                            Application.Current.Resources.MergedDictionaries[0] = resource;
                        language = value;
                    }
                    else
                        throw new Exception("Unable to change language, localization file is damaged.");
                }
                else
                    throw new Exception("Unable to change language, localization file not found.");
            }
        }
        #endregion

        /// <summary>
        /// Использовать встренный архиватор или нет
        /// </summary>
        public bool StandartMode { get; set; } = true;
        /// <summary>
        /// Запись лога
        /// </summary>
        public bool WriteLog { get; set; } = false;
        /// <summary>
        /// Закрыть после резервирования
        /// </summary>
        public bool CloseAfterBackup { get; set; } = false;
        /// <summary>
        /// Уровень сжатия
        /// </summary>
        public int CompressLevel { get; set; } = 1;
        /// <summary>
        /// Путь к исполняемуему файлу WinRAR
        /// </summary>
        public string WinRarExePath { get; set; } = string.Empty;
        /// <summary>
        /// Цвет полоски прогресса
        /// </summary>
        public SolidColorBrush ProgressColor { get; private set; } = Brushes.Green;
        /// <summary>
        /// Путь к конфигу
        /// </summary>
        private string pathConfig = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\Sergey Govorunov\\Settings";
        /// <summary>
        /// Сохранение
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(pathConfig))
                Directory.CreateDirectory(pathConfig);
            ConfigFile config = new ConfigFile($"{pathConfig}\\{App.ProgramName}.cfg");
            config.Config.Add(";0", "DO NOT EDIT THIS SECTION");
            config.Config.Add(";1", "Basic Settings");
            config.Config.Add("Language", Language);
            config.Config.Add("WriteLog", WriteLog.ToString());
            config.Config.Add("CloseAfterBackup", CloseAfterBackup.ToString());
            config.Config.Add("CompressionLevel", CompressLevel.ToString());
            config.Config.Add("StandartMode", StandartMode.ToString());
            if (WinRarExePath != string.Empty)
                config.Config.Add("WinRar.ExePath", WinRarExePath);
            config.Config.Add(";2", "Additional settings");
            config.Config.Add("ProgressColor", $"{ProgressColor.Color.R}:{ProgressColor.Color.G}:{ProgressColor.Color.B}");
            config.Save();
        }
        /// <summary>
        /// Загрузка настроек
        /// </summary>
        public void Load()
        {
            pathConfig = Registry.GetPathSettings(pathConfig);
            ConfigFile config = new ConfigFile($"{pathConfig}\\{App.ProgramName}.cfg");
            if (config.Load())
            {        
                if (config.Config.ContainsKey("WriteLog"))
                    WriteLog = Convert.ToBoolean(config.Config["WriteLog"]);
                if (config.Config.ContainsKey("CloseAfterBackup"))
                    CloseAfterBackup = Convert.ToBoolean(config.Config["CloseAfterBackup"]);
                if (config.Config.ContainsKey("CompressionLevel"))
                    CompressLevel = Convert.ToInt32(config.Config["CompressionLevel"]);
                if (config.Config.ContainsKey("StandartMode"))
                    StandartMode = Convert.ToBoolean(config.Config["StandartMode"]);
                if (config.Config.ContainsKey("WinRar.ExePath"))
                    WinRarExePath = config.Config["WinRar.ExePath"];
                if (config.Config.ContainsKey("ProgressColor"))
                {
                    string[] col = config.Config["ProgressColor"].Split(':');
                    ProgressColor = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(col[0]), Convert.ToByte(col[1]), Convert.ToByte(col[2])));
                }
                if (config.Config.Keys.Contains("Language"))
                    Language = config.Config["Language"].ToLower();
                else
                    Language = "english";
            }
            else
            {
                Language = "english";
            }
        }
    }
}
