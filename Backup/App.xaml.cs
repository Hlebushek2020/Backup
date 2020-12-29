using Backup.Classes;
using System;
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
        public static SettingsProgram Settings { get; private set; }
        /// <summary>
        /// Название программы
        /// </summary>
        public const string ProgramName = "Backup";

        [STAThread]
        private static void Main()
        {
            try
            {
                App application = new App();
                application.InitializeComponent();
                application.Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //try
            //{
            Settings = SettingsProgram.GetInstance();
            MainWindow = new MainWindow();
            MainWindow.Show();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message, ProgramName, MessageBoxButton.OK, MessageBoxImage.Error);
            //    Shutdown();
            //}
        }
    }
}
