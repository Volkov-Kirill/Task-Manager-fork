using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger _logger = Log.ForContext<App>();

        public static string CurrentUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            LoggerConfig.Initialize();
            _logger.Information("WPF Приложение Task-Manager запускается...");

        }
    }
}
