using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace x2tap.Utils
{
	/// <summary>
	///		多语言处理类
	/// </summary>
	public static class MultiLanguage
	{
		/// <summary>
		///		将文本翻译成系统语言对应的
		/// </summary>
		/// <param name="text">需要翻译的文本</param>
		/// <returns></returns>
		public static string Translate(string text)
		{
			if (Global.i18N.ContainsKey(text))
			{
				return Global.i18N[text];
			}

			return text;
		}
	}
}
