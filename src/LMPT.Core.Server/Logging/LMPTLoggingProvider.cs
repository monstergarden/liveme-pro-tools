using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LMPT.Core.Server.ViewModels;

namespace LMPT.Core.Server.Logging
{
    public class LmptLoggingProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public LmptLoggingProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            var analytics = _serviceProvider.GetService<Analytics>();
            return new LmptLogger(categoryName, analytics);
        }
    }
}