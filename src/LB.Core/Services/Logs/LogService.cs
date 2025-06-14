using LB.Core.Containers;
using LB.Core.Services.Analyses;
using LB.Core.Services.Settings;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog.Templates;
using Serilog.Templates.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LB.Core.Services.Logs
{
    internal class LogService : ILogService
    {
        [Inject]
        private IContainer Container { get; init; }

        private ILog Log { get; set; }

        private ILogger _rootLogger;

        public void OnResolved()
        {
            var logFormat = "[{@t:yyyy-MM-dd HH:mm:ss.fff}]" +
                "[{@l:u3}]" +
                "{#if LogTag is not null}[{LogTag}]{#end}" +
                "{@m:lj}\n" +
                "{#if @x is not null}@x\n{#end}";

            var outputConsoleTemplate = new ExpressionTemplate(
                    logFormat, theme: TemplateTheme.Code
                );
            var outputFileTemplate = new ExpressionTemplate(
                    logFormat
                );

            var logFolder = Path.Combine(Utils.AppDataFolder, "Logs/main.log");

            Serilog.Log.Logger = _rootLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(outputConsoleTemplate)
                .WriteTo.File(outputFileTemplate, logFolder)
                .Enrich.FromLogContext()
                .CreateLogger();

            Container.RegisterType(typeof(ILog<>), OnCreateLog, false, typeof(ILog), false);

            Log = Container.Resolve<ILog>([new LogInfoAttribute() { Tag = "日志服务" }]);

            Log.Information($"OnResolved");
        }

        private object OnCreateLog(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            LogInfoAttribute logInfo = extraInfos?.FirstOrDefault(x => x is LogInfoAttribute) as LogInfoAttribute;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ILog<>))
            {
                var logType = type.GetGenericArguments()[0];
                var logInstance = Activator.CreateInstance(typeof(Log<>).MakeGenericType(logType), _rootLogger, logInfo?.Tag) as ILog;
                return logInstance;
            }
            else if (type == typeof(ILog))
            {
                return new LogNormal(_rootLogger, logInfo?.Tag);
            }
            return null;
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
        }

        public async Task OnServiceInitialize()
        {
            Log.Information($"OnServiceInitialize");
            await Task.CompletedTask;
        }

        public async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}