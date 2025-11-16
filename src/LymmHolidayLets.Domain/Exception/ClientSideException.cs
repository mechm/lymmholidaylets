namespace LymmHolidayLets.Domain.Exception
{
	public sealed class ClientSideException
	{
		private readonly string _detail;
		private readonly string _stackTrace;
		private readonly string _scriptUrl;
		private readonly int? _lineNumber;
		private readonly int? _column;

		public ClientSideException(string detail, string stackTrace, string scriptUrl, int? lineNumber = null,
			int? column = null)
		{
			_detail = detail;
			_stackTrace = stackTrace;
			_scriptUrl = scriptUrl;
			_lineNumber = lineNumber;
			_column = column;
		}

		public string Message
		{
			get
			{
				string message = "Client-Side Error - ";
				if (_lineNumber.HasValue)
				{
					message += $"Line {_lineNumber.Value} ";
				}
				message += $"in {_scriptUrl}";
				return message;
			}
		}

		public string StackTrace => _stackTrace;

		public string ScriptUrl => _scriptUrl;

		public int? LineNumber => _lineNumber;

		public int? Column => _column;

		public override string ToString()
		{
			string exceptionString = $"{Message}{Environment.NewLine}{_detail}{Environment.NewLine}";
			if (_column.HasValue)
			{
				if (Column != null) exceptionString += $"Col: {Column.Value}{Environment.NewLine}";
			}
			exceptionString += _stackTrace;
			return exceptionString;
		}
	}
}
