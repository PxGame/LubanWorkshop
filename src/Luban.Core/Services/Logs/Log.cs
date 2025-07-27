using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;

namespace Luban.Core.Services.Logs
{
    internal class Log : ILog, ILogEventEnricher
    {
        private readonly ILogger _logger;
        private readonly Dictionary<string, object> _propertyDict;

        public Log(ILogger rootlogger, Dictionary<string, object> propertyDict)
        {
            _logger = rootlogger?.ForContext(this);
            _propertyDict = propertyDict;
        }

        public void Verbose(string messageTemplate)
        {
            _logger?.Verbose(messageTemplate);
        }

        public void Verbose(Exception exception, string messageTemplate)
        {
            _logger?.Verbose(exception, messageTemplate);
        }

        public void Debug(string messageTemplate)
        {
            _logger?.Debug(messageTemplate);
        }

        public void Debug(Exception exception, string messageTemplate)
        {
            _logger?.Debug(exception, messageTemplate);
        }

        public void Information(string messageTemplate)
        {
            _logger?.Information(messageTemplate);
        }

        public void Information(Exception exception, string messageTemplate)
        {
            _logger?.Information(exception, messageTemplate);
        }

        public void Warning(string messageTemplate)
        {
            _logger?.Warning(messageTemplate);
        }

        public void Warning(Exception exception, string messageTemplate)
        {
            _logger?.Warning(exception, messageTemplate);
        }

        public void Error(string messageTemplate)
        {
            _logger?.Error(messageTemplate);
        }

        public void Error(Exception exception, string messageTemplate)
        {
            _logger?.Error(exception, messageTemplate);
        }

        public void Fatal(string messageTemplate)
        {
            _logger?.Fatal(messageTemplate);
        }

        public void Fatal(Exception exception, string messageTemplate)
        {
            _logger?.Fatal(exception, messageTemplate);
        }

        public virtual void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (_propertyDict != null)
            {
                foreach (var item in _propertyDict)
                {
                    var property = propertyFactory.CreateProperty(item.Key, item.Value, true);
                    logEvent.AddPropertyIfAbsent(property);
                }
            }
        }
    }
}