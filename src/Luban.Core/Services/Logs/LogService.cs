using Luban.Core.Containers;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Luban.Core.Services.Logs
{
    public static class LogParams
    {
        public const string RelativeFilePath = "RelativeFilePath";
        public const string LogTag = "LogTag";
    }

    internal class LogService : ILogService
    {
        private ILogger _rootLogger;

        public override void OnResolved()
        {
            var logFormat = "[{@t:yyyy-MM-dd HH:mm:ss.fff}]" +
              "[{@l:u3}]" +
              "{#if " + LogParams.LogTag + " is not null}[{" + LogParams.LogTag + "}]{#end}" +
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
#if DEBUG
                    t.Debug();
#endif
                    if (Utils.IsWeb)
                    {
                        t.BrowserConsole();
                    }
                    else
                    {
                        t.Console(outputConsoleTemplate);

                        t.Map(LogParams.RelativeFilePath, (relativeFilePath, lc) =>
                        {
                            var filePath = Utils.PathCombine(Utils.AppLogFolder, relativeFilePath);
                            lc.File(outputFileTemplate, filePath, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day);
                        });
                        t.Logger(lc =>
                        {
                            var filePath = Utils.PathCombine(Utils.AppLogFolder, $"main_.log");
                            lc.Filter.ByExcluding(t => t.Properties.ContainsKey(LogParams.RelativeFilePath))
                                .WriteTo.File(outputFileTemplate, filePath, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day);
                        });
                    }
                })
                .CreateLogger();

            Container.RegisterType(typeof(ILog), OnCreateLog, false, null, false);
        }

        private object OnCreateLog(IRegistration regist, Type type, List<object> extraInfos, object[] args)
        {
            var dict = new Dictionary<string, object>();

            var logAttr = extraInfos?.FirstOrDefault(x => x is LogAttribute) as LogAttribute;
            if (logAttr != null)
            {
                if (!string.IsNullOrEmpty(logAttr.Tag))
                {
                    dict[LogParams.LogTag] = logAttr.Tag;
                }

                var injectTarget = extraInfos.FirstOrDefault(x => x is InjectTarget) as InjectTarget;
                if (injectTarget != null && injectTarget.Target != null)
                {
                    var propertyDict = logAttr.GetCustomProperty(injectTarget.Target);
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

        public override void OnInstanceReleased()
        {
            Log.Information($"OnInstanceReleased");
            Serilog.Log.CloseAndFlush();
        }

        public override async Task OnServiceInitialing()
        {
            await base.OnServiceInitialing();
            await Task.CompletedTask;
        }

        public override async Task OnServiceInitialized()
        {
            await base.OnServiceInitialized();
            Log.Information($"OnServiceInitialized");
            await Task.CompletedTask;
        }

        public override async Task OnServiceShutdown()
        {
            Log.Information($"OnServiceShutdown");
            await Task.CompletedTask;
        }
    }
}