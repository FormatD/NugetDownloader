using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;
//using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace NupkgDownloader.Web.Controllers
{
    public class Logger<T> : NuGet.Common.ILogger
    {
        private ILogger<T> _logger;

        public Logger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Log(NuGet.Common.LogLevel level, string data)
        {
            _logger.LogInformation(data);
        }

        public void Log(ILogMessage message)
        {
            _logger.LogInformation(message.ToString());

        }

        public async Task LogAsync(NuGet.Common.LogLevel level, string data)
        {
            _logger.LogInformation(data);
        }

        public async Task LogAsync(ILogMessage message)
        {
            _logger.LogInformation(message.ToString());
        }

        public void LogDebug(string data)
        {
            _logger.LogDebug(data);
        }

        public void LogError(string data)
        {
            _logger.LogError(data);
        }

        public void LogInformation(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogInformationSummary(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogVerbose(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogWarning(string data)
        {
            _logger.LogWarning(data);
        }
    }
}