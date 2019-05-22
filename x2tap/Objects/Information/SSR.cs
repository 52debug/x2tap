namespace x2tap.Objects.Information
{
	public class SSR
	{
		/// <summary>
		///		服务器地址
		/// </summary>
		public string server;

		/// <summary>
		///		服务器端口
		/// </summary>
		public int server_port;

		/// <summary>
		///		密钥
		/// </summary>
		public string password;

		/// <summary>
		///		加密方式
		/// </summary>
		public string method;

		/// <summary>
		///		协议
		/// </summary>
		public string protocol;

		/// <summary>
		///		协议参数
		/// </summary>
		public string protocol_param;

		/// <summary>
		///		混淆
		/// </summary>
		public string obfs;

		/// <summary>
		///		混淆参数
		/// </summary>
		public string obfs_param;

		/// <summary>
		///		本地地址
		/// </summary>
		public string local_address = "127.0.0.1";

		/// <summary>
		///		本地地址
		/// </summary>
		public int local_port = 2810;
	}
}
