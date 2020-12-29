using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Backup.Classes
{
    public class SettingsProgram
    {
        #region Language
        [JsonIgnore]
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
        public SolidColorBrush ProgressColor { get; set; } = Brushes.Green;

        [JsonIgnore]
        public static string ProgramResourceFolder { get; } = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SergeyGovorunov\\{App.ProgramName}";
        /// <summary>
        /// Сохранение
        /// </summary>
        public void Save()
        {
            Directory.CreateDirectory(ProgramResourceFolder);
            using (StreamWriter streamWriter = new StreamWriter($"{ProgramResourceFolder}\\settings.json", false, Encoding.UTF8))
                streamWriter.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
        }
        /// <summary>
        /// Получает объект с загружеными настройками
        /// </summary>
        public static SettingsProgram GetInstance()
        {
            string settingsFile = $"{ProgramResourceFolder}\\settings.json";
            if (File.Exists(settingsFile))
                return JsonConvert.DeserializeObject<SettingsProgram>(File.ReadAllText(settingsFile, Encoding.UTF8));
            return new SettingsProgram();
        }
    }
}
