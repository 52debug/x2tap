using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace x2tap.Objects
{
	public class Server
	{
		/// <summary>
		///		备注（S5、SS、SR、V2）
		/// </summary>
		public string Remark = String.Empty;

		/// <summary>
		///		组名（S5、SS、SR、V2）
		/// </summary>
		public string GroupID = "00000000-0000-0000-0000-000000000000";

		/// <summary>
		///		类型（S5、SS、SR、V2）
		/// </summary>
		public string Type = String.Empty; // Socks5、Shadowsocks、ShadowsocksR、VMess

		/// <summary>
		///		地址（S5、SS、SR、V2）
		/// </summary>
		public string Address = String.Empty;

		/// <summary>
		///		端口（S5、SS、SR、V2）
		/// </summary>
		public int Port = 0;

		/// <summary>
		///		密码（SS、SR）
		/// </summary>
		public string Password = String.Empty;

		/// <summary>
		///		用户 ID（V2）
		/// </summary>
		public string UserID = String.Empty;

		/// <summary>
		///		额外 ID（V2）
		/// </summary>
		public int AlterID = 0;

		/// <summary>
		///		加密方式（SS、SR、V2 QUIC）
		/// </summary>
		public string EncryptMethod = String.Empty;

		/// <summary>
		///		协议（SR）
		/// </summary>
		public string Protocol = "origin";

		/// <summary>
		///		协议参数（SR）
		/// </summary>
		public string ProtocolParam = String.Empty;

		/// <summary>
		///		混淆（SR）
		/// </summary>
		public string OBFS = "plain";

		/// <summary>
		///		混淆参数（SR）
		/// </summary>
		public string OBFSParam = String.Empty;

		/// <summary>
		///		传输协议（V2）
		/// </summary>
		public string TransferProtocol = "tcp";

		/// <summary>
		///		伪装类型（V2）
		/// </summary>
		public string FakeType = String.Empty;

		/// <summary>
		///		伪装域名（V2：HTTP、WebSocket、HTTP/2）
		/// </summary>
		public string Host = String.Empty;

		/// <summary>
		///		传输路径（V2：WebSocket、HTTP/2）
		/// </summary>
		public string Path = String.Empty;

		/// <summary>
		///		QUIC 加密方式（V2）
		/// </summary>
		public string QUICSecurity = "none";

		/// <summary>
		///		QUIC 加密密钥（V2）
		/// </summary>
		public string QUICSecret = String.Empty;

		/// <summary>
		///		TLS 底层传输安全（V2）
		/// </summary>
		public bool TLSSecure = false;

		/// <summary>
		///		国家代码
		/// </summary>
		public string CountryCode = "NN";

		/// <summary>
		///		延迟（S5、SS、SR、V2）
		/// </summary>
		public int Delay = 999;

		/// <summary>
		///		获取备注
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (String.IsNullOrEmpty(Remark))
			{
				Remark = String.Format("{0}:{1}", Address, Port);
			}

			if (CountryCode == "NN")
			{
				Task.Run(() =>
				{
					try
					{
						var result = Dns.GetHostAddressesAsync(Address);
						if (!result.Wait(1000))
						{
							return;
						}

						if (result.Result.Length > 0)
						{
							CountryCode = Utils.GeoIP.GetCountryISOCode(result.Result[0]);
						}
					}
					catch (Exception)
					{
						// 跳过
					}
				});
			}

			if (Type == "Socks5")
			{
				return String.Format("[{0}] {1}", "S5", Remark);
			}
			else if (Type == "Shadowsocks")
			{
				return String.Format("[{0}] {1}", "SS", Remark);
			}
			else if (Type == "ShadowsocksR")
			{
				return String.Format("[{0}] {1}", "SR", Remark);
			}
			else
			{
				return String.Format("[{0}] {1}", "V2", Remark);
			}
		}

		/// <summary>
		///		测试延迟
		/// </summary>
		/// <returns>延迟</returns>
		public int Test()
		{
			if (Type == "S5" || Type == "SS" || Type == "SR")
			{
				return TestTCP();
			}
			else
			{
				if (TransferProtocol == "tcp" || TransferProtocol == "ws" || TransferProtocol == "h2")
				{
					return TestTCP();
				}
				else
				{
					return TestICMP();
				}
			}
		}

		/// <summary>
		///		测试 TCP 延迟
		/// </summary>
		/// <returns>延迟</returns>
		public int TestTCP()
		{
			using (var client = new Socket(SocketType.Stream, ProtocolType.Tcp))
			{
				try
				{
					var destination = Dns.GetHostAddressesAsync(Address);
					if (!destination.Wait(1000))
					{
						return Delay = 460;
					}

					if (destination.Result.Length == 0)
					{
						return Delay = 460;
					}

					var watch = new Stopwatch();
					watch.Start();

					var task = client.BeginConnect(new IPEndPoint(destination.Result[0], Port), (result) =>
					{
						watch.Stop();
					}, 0);

					if (task.AsyncWaitHandle.WaitOne(460))
					{
						return Delay = (int)(watch.ElapsedMilliseconds >= 460 ? 460 : watch.ElapsedMilliseconds);
					}

					return Delay = 460;
				}
				catch (Exception)
				{
					return Delay = 460;
				}
			}
		}

		/// <summary>
		///		测试 ICMP 延迟
		/// </summary>
		/// <returns>延迟</returns>
		public int TestICMP()
		{
			using (var sender = new Ping())
			{
				try
				{
					var destination = Dns.GetHostAddressesAsync(Address);
					if (!destination.Wait(1000))
					{
						return Delay = 460;
					}

					if (destination.Result.Length == 0)
					{
						return Delay = 460;
					}

					var response = sender.Send(destination.Result[0], 460);

					return Delay = (int)(response.RoundtripTime == 0 ? 460 : response.RoundtripTime);
				}
				catch (Exception)
				{
					return Delay = 460;
				}
			}
		}
	}
}
