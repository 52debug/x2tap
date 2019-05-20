using System;
using System.Collections.Generic;

namespace x2tap.Objects
{
	public class Mode
	{
		/// <summary>
		///		名称
		/// </summary>
		public string Name;

		/// <summary>
		///		是否为内置规则
		/// </summary>
		public bool IsInternal = false;

		/// <summary>
		///		类型（0. 仅规则内走直连 1. 仅规则内走代理）
		/// </summary>
		public int Type;

		/// <summary>
		///		绕过中国（0. 无需额外绕过 1. 额外添加中国直连路由）
		/// </summary>
		public bool BypassChina;

		/// <summary>
		///		规则
		/// </summary>
		public List<string> Rule = new List<string>();

		/// <summary>
		///		获取备注
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("[{0}] {1}", IsInternal ? Utils.MultiLanguage.Translate("Internal") : Utils.MultiLanguage.Translate("External"), Name);
		}
	}
}
