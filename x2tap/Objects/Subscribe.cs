using System;

namespace x2tap.Objects
{
	public class Subscribe
	{
		/// <summary>
		///		组 ID
		/// </summary>
		public string GroupID = Guid.NewGuid().ToString();

		/// <summary>
		///		组名
		/// </summary>
		public string GroupName = String.Empty;

		/// <summary>
		///		链接
		/// </summary>
		public string Link;
	}
}
