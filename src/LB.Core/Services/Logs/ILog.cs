using System;

namespace LB.Core.Services.Logs
{
    public interface ILog
    {
        void Verbose(string messageTemplate);

        void Verbose(Exception exception, string messageTemplate);

        void Debug(string messageTemplate);

        void Debug(Exception exception, string messageTemplate);

        void Information(string messageTemplate);

        void Information(Exception exception, string messageTemplate);

        void Warning(string messageTemplate);

        void Warning(Exception exception, string messageTemplate);

        void Error(string messageTemplate);

        void Error(Exception exception, string messageTemplate);

        void Fatal(string messageTemplate);

        void Fatal(Exception exception, string messageTemplate);
    }
}