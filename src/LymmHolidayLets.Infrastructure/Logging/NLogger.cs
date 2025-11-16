using LymmHolidayLets.Domain.Exception;
using LymmHolidayLets.Domain.Interface;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Text;
using ILogger = LymmHolidayLets.Domain.Interface.ILogger;
using NLoggerLog = NLog.Logger;

namespace LymmHolidayLets.Infrastructure.Logging
{
	public sealed class NLogger : ILogger
	{
		private static readonly NLoggerLog Logger = LogManager.GetCurrentClassLogger();

		private readonly IEnumerable<string> _httpVariablesToInclude = new List<string>
		{
			"User",
			"Content-Length",
			"Content-Type",
			"Accept",
			"Connection",
			"Host",
			"Method",
			"Referer",
			"User-Agent",
			"Addr",
			"Path-Info",
			"Remote-Addr",
			"Server-Protocol",
			"X-Original-For",

			"AUTH_USER",
			"CONTENT_LENGTH",
			"CONTENT_TYPE",
			"HTTP_ACCEPT",
			"HTTP_CONNECTION",
			"HTTP_HOST",
			"HTTP_METHOD",
			"HTTP_REFERER",
			"HTTP_USER_AGENT",
			"LOCAL_ADDR",
			"LOGON_USER",
			"PATH_INFO",
			"REMOTE_ADDR",
			"SERVER_PROTOCOL"
		};

		public void LogFatal(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Fatal, message, businessTransaction, ex, properties);
		}

		public void LogFatal(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Fatal, message, businessTransaction, null, properties);
		}

		public void LogFatal(string message, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Fatal, message, string.Empty, ex, properties);
		}

		public void LogFatal(string message, params object[] properties)
		{
            Log(LogLevel.Fatal, message, string.Empty, null, properties);
		}

		public async Task LogError(string message, HttpContext? httpContext, IBusinessTransaction? businessTransaction, System.Exception ex,
			params object[] properties)
		{
			await LogWithContext(LogLevel.Error, message, httpContext, businessTransaction, ex, properties);
		}

		public void LogError(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Error, message, businessTransaction, ex, properties);
		}

		public void LogError(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Error, message, businessTransaction, null, properties);
		}

		public void LogError(string message, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Error, message, string.Empty, ex, properties);
		}

		public void LogError(string message, params object[] properties)
		{
            Log(LogLevel.Error, message, string.Empty, null, properties);
		}

		public void LogInfo(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Info, message, businessTransaction, ex, properties);
		}

		public void LogInfo(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Info, message, businessTransaction, null, properties);
		}

		public void LogInfo(string message, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Info, message, string.Empty, ex, properties);
		}

		public void LogInfo(string message, params object[] properties)
		{
            Log(LogLevel.Info, message, string.Empty, null, properties);
		}

		public async Task LogWarning(string message, HttpContext httpContext, IBusinessTransaction businessTransaction, System.Exception ex,
			params object[] properties)
		{
			await LogWithContext(LogLevel.Warn, message, httpContext, businessTransaction, ex, properties);
		}

		public void LogWarning(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Warn, message, businessTransaction, ex, properties);
		}

		public void LogWarning(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Warn, message, businessTransaction, null, properties);
		}

		public void LogWarning(string message, System.Exception ex, params object[] properties)
		{
            Log(LogLevel.Warn, message, string.Empty, ex, properties);
		}

		public void LogWarning(string message, params object[] properties)
		{
            Log(LogLevel.Warn, message, string.Empty, null, properties);
		}

		public void LogDebug(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Debug, message, businessTransaction, null, properties);
		}

		public void LogDebug(string message, params object[] properties)
		{
            Log(LogLevel.Debug, message, string.Empty, null, properties);
		}

		public void LogTrace(string message, IBusinessTransaction businessTransaction, params object[] properties)
		{
            Log(LogLevel.Trace, message, businessTransaction, null, properties);
		}

		public void LogTrace(string message, params object[] properties)
		{
            Log(LogLevel.Trace, message, string.Empty, null, properties);
		}

		public static void LogMetric(string message, IBusinessTransaction? businessTransaction, System.Exception? lastException, params object[] properties)
		{
			LogEventInfo logEvent = LogEventInfo.Create(LogLevel.Trace, Logger.Name, null, message, properties);
			logEvent.Properties.Add("diagnosticType", "metric");
			if (businessTransaction != null && !string.IsNullOrEmpty(businessTransaction.CorrelationId))
			{
				logEvent.Properties.Add("correlationId", businessTransaction.CorrelationId);
			}
			logEvent.Properties.Add("hasException", lastException != null);
			if (lastException != null)
			{
				logEvent.Properties.Add("exceptionType", lastException.GetType().FullName);
			}
			logEvent.Properties.Add("machineName", Environment.MachineName);
			logEvent.Properties.Add("appVersion", GetAppVersion());
			Logger.Log(logEvent);
		}

		private static void Log(LogLevel logLevel, string message, IBusinessTransaction businessTransaction, System.Exception? exception,
			params object[] properties)
		{
            Log(logLevel, message, businessTransaction.CorrelationId, exception, properties);
		}

		private static void Log(LogLevel logLevel, string message, string correlationId, System.Exception? exception, params object[] properties)
		{
			LogEventInfo logEvent = LogEventInfo.Create(logLevel, Logger.Name, exception, null, message, properties);
			logEvent.Properties.Add("diagnosticType", "event");
			if (!string.IsNullOrEmpty(correlationId))
			{
				logEvent.Properties.Add("correlationId", correlationId);
			}
			logEvent.Properties.Add("machineName", Environment.MachineName);
			logEvent.Properties.Add("tier", "server");
			logEvent.Properties.Add("appVersion", GetAppVersion());
			// add exception type
			if (exception != null)
			{
				logEvent.Properties.Add("exceptionType", exception.GetType().FullName);
			}
			Logger.Log(logEvent);
		}

		private async Task LogWithContext(LogLevel logLevel, string message, HttpContext? httpContext, IBusinessTransaction? businessTransaction,
			System.Exception? exception, params object[] properties)
		{
			LogEventInfo logEvent = LogEventInfo.Create(logLevel, Logger.Name, exception, null, message, properties);
			logEvent.Properties.Add("diagnosticType", "event");
			if (businessTransaction != null && !string.IsNullOrEmpty(businessTransaction.CorrelationId))
			{
				logEvent.Properties.Add("correlationId", businessTransaction.CorrelationId);
			}
			logEvent.Properties.Add("machineName", Environment.MachineName);
			logEvent.Properties.Add("tier", "server");
			logEvent.Properties.Add("appVersion", GetAppVersion());

			if (exception != null)
			{
				logEvent.Properties.Add("exceptionType", exception.GetType()?.FullName);
			}

			logEvent = await AddHttpContextDetail(logEvent, httpContext);

			Logger.Log(logEvent);
		}

		private async Task<LogEventInfo> AddHttpContextDetail(LogEventInfo logEvent, HttpContext? httpContext)
		{
			if (httpContext?.Request == null) return logEvent;

			foreach (string httpVariableName in _httpVariablesToInclude)
			{
				if (httpContext.Request.Headers.TryGetValue(httpVariableName, out var header))
				{
					logEvent.Properties.Add($"request_{httpVariableName}", header);
				}
			}

			logEvent.Properties.Add("request_url", $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}");

			if (!string.IsNullOrEmpty(httpContext.Request.QueryString.Value))
			{
				logEvent.Properties.Add("request_queryString", httpContext.Request.QueryString.Value);
			}

			if (httpContext.Connection?.RemoteIpAddress != null)
			{
				logEvent.Properties.Add("request_remoteIpAddress", httpContext.Connection.RemoteIpAddress.ToString());
			}

			string httpContextBody = await RetrieveHttpContextBody(httpContext);

			if (!string.IsNullOrEmpty(httpContextBody))
			{
				logEvent.Properties.Add("request_body", httpContextBody);
			}

			return logEvent;
		}

		private static async Task<string> RetrieveHttpContextBody(HttpContext context)
		{
			MemoryStream injectedRequestStream = new();

			try
			{
				var requestLog =
					$"REQUEST HttpMethod: {context.Request.Method}, Path: {context.Request.Path}";

                using var bodyReader = new StreamReader(context.Request.Body);
                var bodyAsText = await bodyReader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                {
                    requestLog += $", Body : {bodyAsText}";
                }

                var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
                injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
                injectedRequestStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = injectedRequestStream;

                return requestLog;
			}
			finally
			{
				await injectedRequestStream.DisposeAsync();
			}
		}

		public async Task LogClientSideError(ClientSideException clientSideException, IBusinessTransaction businessTransaction,
					HttpContext httpContext)
		{
			await LogClientSide(clientSideException, businessTransaction, httpContext, LogLevel.Error);
		}

		public async Task LogClientSideWarning(ClientSideException clientSideException, IBusinessTransaction businessTransaction,
			HttpContext httpContext)
		{
			await LogClientSide(clientSideException, businessTransaction, httpContext, LogLevel.Warn);
		}

		private async Task LogClientSide(ClientSideException clientSideException, IBusinessTransaction? businessTransaction,
			HttpContext httpContext, LogLevel logLevel)
		{
			LogEventInfo logEvent = LogEventInfo.Create(logLevel, Logger.Name, null, null, clientSideException.Message);

			if (businessTransaction != null && !string.IsNullOrEmpty(businessTransaction.CorrelationId))
			{
				logEvent.Properties.Add("correlationId", businessTransaction.CorrelationId);
			}

			logEvent.Properties.Add("exception", clientSideException.ToString());
			logEvent.Properties.Add("diagnosticType", "event");
			logEvent.Properties.Add("tier", "client");
			logEvent.Properties.Add("appVersion", GetAppVersion());
			logEvent.Properties.Add("machineName", Environment.MachineName);
			logEvent = await AddHttpContextDetail(logEvent, httpContext);
			Logger.Log(logEvent);
		}

		private static string GetAppVersion()
		{
            var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version;

			return version != null ? version.ToString() : string.Empty;
        }
	}
}
