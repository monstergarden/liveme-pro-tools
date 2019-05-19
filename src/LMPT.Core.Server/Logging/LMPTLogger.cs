using System;
using LMPT.Core.Services.Model;
using LMPT.Core.Server.ViewModels;
using Microsoft.Extensions.Logging;

namespace LMPT.Core.Server.Logging
{
    public class LmptLogger : ILogger
    {
        private readonly Analytics _analytics;
        private readonly string _name;

        public LmptLogger(string name, Analytics analytics)
        {
            _name = name;
            _analytics = analytics;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (eventId == LogEventIds.ForFooter) _analytics.FooterInfo = state.ToString();

            // if((int)logLevel < 2)
            // { 
            //     return;
            // }


            if (_name.Contains("LMPT"))
            {
                var name = _name;
                // Remove class name coming from logs triggered in UI classes.
                if (name.Contains("LMPT.Core.Server.Components.Shared")) name = "";

                var logVm = new LogViewModel
                {
                    DateTimeFormatted = DateTime.Now.ToString("H:mm:ss"),
                    Source = name,
                    LogLevel = (int) logLevel,
                    Log = state.ToString()
                };
                _analytics.AddLog(logVm);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}