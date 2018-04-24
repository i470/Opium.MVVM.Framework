using System;

namespace Opium.MVVM.Framework.Services
{
    public interface ILogger
    {
 
        void SetSeverity(LogSeverity minimumLevel);
        
        void Log(LogSeverity severity, string source, string message);

        void Log(LogSeverity severity, string source, Exception exception);

        void LogFormat(LogSeverity severity, string source, string messageTemplate, params object[] arguments);
    }
}
