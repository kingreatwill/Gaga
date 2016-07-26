
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Diagnostics;

namespace Gaga.Core.Logging
{
	public class EntLib6Logger:ILogger
	{
		private readonly LogWriter _logWriter;//LogWriterFactory
		public EntLib6Logger()
		{
			this._logWriter = new LogWriterFactory().Create();
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="EntLib5Logger"/> class.
		/// </summary>
		/// <param name="typeName">The type name.</param>
		public EntLib6Logger(string typeName)
		{
			this._logWriter = new LogWriterFactory().Create();
			this._logWriter.SetContextItem("type", typeName);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntLib5Logger"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		public EntLib6Logger(Type type)
		{
			this._logWriter = new LogWriterFactory().Create();
			this._logWriter.SetContextItem("type", type.Name);
		}

		public bool IsEnabled(LogLevel level)
		{
			if (this._logWriter.IsLoggingEnabled())
				return true;
			else
				return false;
		}

		public void Log(LogLevel level, Exception exception, string format, params object[] args)
		{
			//if (IsDebugEnabled)
			//{
			//	System.Diagnostics.Debug.WriteLine(message, category);
			//}
			//else
			//{
			//	if (logWriter.IsLoggingEnabled())
			//		WriteMessage(message, TraceEventType.Verbose, category, exception);
			//}

			TraceEventType tELevel= TraceEventType.Transfer;
			switch (level)
			{
				case LogLevel.Debug:
					tELevel = TraceEventType.Verbose; break;
				case LogLevel.Error:
					tELevel = TraceEventType.Error; break;
				case LogLevel.Fatal:
					tELevel = TraceEventType.Critical; break;
				case LogLevel.Information:
					tELevel = TraceEventType.Information; break;
				case LogLevel.Warning:
					tELevel = TraceEventType.Warning; break;
			}
			WriteMessage(format, tELevel,null,exception);
		}

		private void WriteMessage(object message, TraceEventType eventType, object category = null, Exception exception = null)
		{
			var entry = new LogEntry();
			entry.Severity = eventType;

			//Add Message 
			if (message != null)
				entry.Message = message.ToString();

			//Add category 
			if (category != null)
				entry.Categories.Add(category.ToString());

			//Add exception, user name, detailed date & time if needed 
			entry.ExtendedProperties.Add("Exception", exception);
			entry.TimeStamp = DateTime.UtcNow;

			this._logWriter.Write(entry);
		}
	}
}
