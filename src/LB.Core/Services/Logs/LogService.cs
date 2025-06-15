using LB.Core.Containers;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

            Serilog.Log.Logger = _rootLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Async(t =>
                {
                    t.Console(outputConsoleTemplate);
                    t.Map("RelativeFilePath", (relativeFilePath, lc) =>
                    {
                        var filePath = Path.Combine(Utils.AppLogFolder, relativeFilePath).StandardizedPath();
                        lc.File(outputFileTemplate, filePath, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day);
                    });
                    t.Logger(lc =>
                    {
                        var filePath = Path.Combine(Utils.AppLogFolder, $"main_.log").StandardizedPath();
                        lc.Filter.ByExcluding(t => t.Properties.ContainsKey("RelativeFilePath"))
                            .WriteTo.File(outputFileTemplate, filePath, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day);
                    });
                })
                .CreateLogger();

            Container.RegisterType(typeof(ILog), OnCreateLog, false, null, false);

            Log = Container.Resolve<ILog>([new LogAttribute() { Tag = "日志服务" }]);

            Log.Information($"OnResolved");
        }

        private object OnCreateLog(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            var dict = new Dictionary<string, object>();

            var logAttr = extraInfos?.FirstOrDefault(x => x is LogAttribute) as LogAttribute;
            if (logAttr != null && !string.IsNullOrEmpty(logAttr.Tag))
            {
                dict["LogTag"] = logAttr.Tag;
            }

            var injectTarget = extraInfos.FirstOrDefault(x => x is InjectTarget) as InjectTarget;
            if (injectTarget != null && injectTarget.Target != null)
            {
                var logRootAttr = injectTarget.Target.GetType().GetCustomAttribute<LogRootAttribute>(true);
                if (logRootAttr != null)
                {
                    var propertyDict = logRootAttr.GetLogPropertyDict(injectTarget.Target);
                    if (propertyDict != null && propertyDict.Count > 0)
                    {
                        foreach (var kvp in propertyDict)
                        {
                            dict[kvp.Key] = kvp.Value;
                        }
                    }
                }
            }

            return new Log(_rootLogger, dict);
        }

        public void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
            Serilog.Log.CloseAndFlush();
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