

using System;
using System.IO;

namespace Gaga.Core.Logging
{
	public class DefaultLogger : ILogger
	{
		private readonly IWebHelper _webHelper;

		public DefaultLogger(IWebHelper webHelper)
		{
			this._webHelper = webHelper;
		}

		public bool IsEnabled(LogLevel level)
		{
			return true;
		}

		public void Log(LogLevel level, Exception exception, string format, params object[] args)
		{
			string filepath = this._webHelper.MapPath("~/logs") + "\\";
			if (!Directory.Exists(filepath))  //不存在文件夹，创建
			{
				Directory.CreateDirectory(filepath);  //创建新的文件夹
			}

			Utilities.FileOperate.WriteFile(filepath+DateTime.Now.ToString("yyyy-MM-dd")+".txt", level.ToString()+"->"+ DateTime.Now+"->"+ Environment.NewLine+ exception.Message + exception.StackTrace);

		}


	}
}
