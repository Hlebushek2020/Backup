using Backup.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Backup
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Настройки программы
        /// </summary>
        public static SettingsProgram Settings { get; private set; } = new SettingsProgram();
        /// <summary>
        /// Название программы
        /// </summary>
        public const string ProgramName = "Backup";

        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveHandler);
            App application = new App();
            application.InitializeComponent();
            application.Run();
        }

        /// <summary>
        /// Отлавливаем ошибки загрузки сборок, если ядоро не найдено - пытаемся загрузить его из другого места 
        /// </summary>
        private static Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
        {
            Assembly assembly = null;
            if (args.Name.Contains("SergeyCoreNF"))
            {
                string snc = $"{Path.GetDirectoryName(Path.GetDirectoryName(ResourceAssembly.Location))}\\SergeyCoreNF.dll";
                if (File.Exists(snc))
                {
                    AssemblyName asName = AssemblyName.GetAssemblyName(snc);
                    assembly = Assembly.Load(asName);
                }
            }
            return assembly;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Settings.Load();
                MainWindow = new MainWindow();
                MainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}
