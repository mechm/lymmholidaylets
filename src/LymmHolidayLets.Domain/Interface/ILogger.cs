using Microsoft.AspNetCore.Http;
using LymmHolidayLets.Domain.Exception;

namespace LymmHolidayLets.Domain.Interface
{
	public interface ILogger
	{
		void LogFatal(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties);

		void LogFatal(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogFatal(string message, System.Exception ex, params object[] properties);

		void LogFatal(string message, params object[] properties);

		Task LogError(string message, HttpContext? httpContext, IBusinessTransaction? businessTransaction, System.Exception ex, params object[] properties);

		void LogError(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties);

		void LogError(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogError(string message, System.Exception ex, params object[] properties);

		void LogError(string message, params object[] properties);

		Task LogWarning(string message, HttpContext httpContext, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties);

		void LogWarning(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties);

		void LogWarning(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogWarning(string message, System.Exception ex, params object[] properties);

		void LogWarning(string message, params object[] properties);

		void LogInfo(string message, IBusinessTransaction businessTransaction, System.Exception ex, params object[] properties);

		void LogInfo(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogInfo(string message, System.Exception ex, params object[] properties);

		void LogInfo(string message, params object[] properties);

		void LogDebug(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogDebug(string message, params object[] properties);

		void LogTrace(string message, IBusinessTransaction businessTransaction, params object[] properties);

		void LogTrace(string message, params object[] properties);

		Task LogClientSideError(ClientSideException clientSideException, IBusinessTransaction businessTransaction,
					HttpContext httpContext);

		Task LogClientSideWarning(ClientSideException clientSideException, IBusinessTransaction businessTransaction,
			 HttpContext httpContext);
	}
}
