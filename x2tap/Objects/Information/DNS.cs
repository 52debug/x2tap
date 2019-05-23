using System.Collections.Generic;

namespace x2tap.Objects.Information
{
	/// <summary>
	///		DNS 配置
	/// </summary>
	public class DNS
	{
		/// <summary>
		///		服务器列表
		/// </summary>
		public List<object> servers = new List<object>()
		{
			"1.1.1.1"
		};
	}

	/// <summary>
	///		DNS 服务器配置
	/// </summary>
	public class DNSServer
	{
		/// <summary>
		///		地址
		/// </summary>
		public string address;

		/// <summary>
		///		端口
		/// </summary>
		public int port = 53;

		/// <summary>
		///		走匹配这个规则的域名列表
		/// </summary>
		public List<string> domains = new List<string>();
	}
}
