using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Reflection;

namespace LB.Core.Services.Logs
{
    public interface ILog<T> : ILog
    {
    }

    internal class Log<T> : ILog<T>, ILogEventEnricher
    {
        private readonly ILogger _logger;
        private readonly string _tag;

        public Log(ILogger rootlogger, string tag)
        {
            _logger = rootlogger.ForContext(this);
            _tag = tag;
        }

        public void Verbose(string messageTemplate)
        {
            _logger.Verbose(messageTemplate);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            _logger.Verbose(exception, messageTemplate);
        }

        public void Debug(string messageTemplate)
        {
            _logger.Debug(messageTemplate);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            _logger.Debug(exception, messageTemplate);
        }

        public void Information(string messageTemplate)
        {
            _logger.Information(messageTemplate);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            _logger.Information(exception, messageTemplate);
        }

        public void Warning(string messageTemplate)
        {
            _logger.Warning(messageTemplate);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            _logger.Warning(exception, messageTemplate);
        }

        public void Error(string messageTemplate)
        {
            _logger.Error(messageTemplate);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            _logger.Error(exception, messageTemplate);
        }

        public void Fatal(string messageTemplate)
        {
            _logger.Fatal(messageTemplate);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            _logger.Fatal(exception, messageTemplate);
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string tag = _tag ?? typeof(T).Name;
            var property = propertyFactory.CreateProperty("LogTag", tag, true);
            logEvent.AddPropertyIfAbsent(property);
        }
    }

    internal class LogNormal : Log<LogNormal>
    {
        public LogNormal(ILogger rootLogger, string tag) : base(rootLogger, tag)
        {
        }
    }
}