using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			File.AppendAllText("Logging\\Application.log", String.Format("[{0}][INFO] {1}\r\n", DateTime.Now.ToString(), text));
		}
	}
}
