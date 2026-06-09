using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaskManager.Models;
using TaskManager.ViewModels;


namespace TaskManager.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            string userName = App.CurrentUser ?? "Гость";
            Serilog.Log.ForContext("SourceContext", "AppModule")
               .Information("Пользователь '{UserName}' вышел из системы (Сессия завершена).", userName);
            Serilog.Log.ForContext("SourceContext", "AppModule")
               .Information("Приложение корректно завершило работу. Сессия закрыта.");
            Serilog.Log.CloseAndFlush();
        }
    }
}
