using System;
using System.IO;

namespace x2tap.Utils
{
	/// <summary>
	///		日志处理类
	/// </summary>
	public static class Logging
	{
		/// <summary>
		///		信息
		/// </summary>
		/// <param name="text"></param>
		public static void Info(string text)
		{
			File.AppendAllText("Logging\\application.log", String.Format("[{0}][INFO] {1}\r\n", DateTime.Now.ToString(), text));
		}
	}
}
