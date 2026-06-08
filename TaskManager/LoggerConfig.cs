using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    public class LoggerConfig
    {
        public static void Initialize()
        {
            const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level:u3}] [{SourceContext}] - {Message:lj}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()

                .WriteTo.Debug(outputTemplate: outputTemplate)

                .WriteTo.File(
                    path: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log"),
                    outputTemplate: outputTemplate,
                    fileSizeLimitBytes: 5 * 1024 * 1024,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 5
                )
                .CreateLogger();

            Log.Information("Локальное логирование успешно запущено.");
        }
    }
}