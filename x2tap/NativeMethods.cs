using System.Runtime.InteropServices;

namespace x2tap
{
	public static class NativeMethods
	{
		/// <summary>
		///		创建路由规则
		/// </summary>
		/// <param name="address">目标地址</param>
		/// <param name="netmask">掩码地址</param>
		/// <param name="gateway">网关地址</param>
		/// <param name="metric">跃点数</param>
		/// <returns>是否成功</returns>
		[DllImport("x2tapCore.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool CreateRoute(string address, int netmask, string gateway, int metric = 100);

		/// <summary>
		///		修改路由规则
		/// </summary>
		/// <param name="address">目标地址</param>
		/// <param name="netmask">掩码地址</param>
		/// <param name="gateway">网关地址</param>
		/// <param name="metric">跃点数</param>
		/// <returns>是否成功</returns>
		[DllImport("x2tapCore.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool ChangeRoute(string address, int netmask, string gateway, int metric);

		/// <summary>
		///		删除路由规则
		/// </summary>
		/// <param name="address">目标地址</param>
		/// <param name="netmask">掩码地址</param>
		/// <param name="gateway">网关地址</param>
		/// <param name="metric">跃点数</param>
		/// <returns>是否成功</returns>
		[DllImport("x2tapCore.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern bool DeleteRoute(string address, int netmask, string gateway, int metric = 100);
	}
}
