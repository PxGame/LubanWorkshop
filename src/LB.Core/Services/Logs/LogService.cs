using LB.Core.Containers;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                "{#if @x is not null}{@x}\n{#end}";

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

            Container.RegisterType(typeof(ILog), OnCreateLog, false, null, false);

            Log = Container.Resolve<ILog>([new LogAttribute() { Tag = "日志服务" }]);

            Log.Information($"OnResolved");
        }

        private object OnCreateLog(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            LogAttribute logInfo = extraInfos?.FirstOrDefault(x => x is LogAttribute) as LogAttribute;

            return new Log(_rootLogger, logInfo?.Tag);
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