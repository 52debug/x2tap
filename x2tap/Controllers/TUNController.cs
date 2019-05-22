using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x2tap.Controllers
{
	public class TUNController
	{
		/// <summary>
		///		进程实例（tun2socks）
		/// </summary>
		public Process Instance;

		/// <summary>
		///		当前状态
		/// </summary>
		public Objects.State State = Objects.State.Waiting;

		/// <summary>
		///		启动
		/// </summary>
		/// <param name="server">配置</param>
		/// <returns>是否成功</returns>
		public bool Start(Objects.Server server)
		{
			return false;
		}

		public void Stop()
		{
			if (Instance != null && !Instance.HasExited)
			{

			}
		}

		public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{

		}
	}
}
