using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Reflection;

namespace LB.Core.Services.Logs
{
    internal class Log : ILog, ILogEventEnricher
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

        public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (!string.IsNullOrEmpty(_tag))
            {
                var property = propertyFactory.CreateProperty("LogTag", _tag, true);
                logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}