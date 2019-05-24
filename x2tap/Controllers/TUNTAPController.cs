using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace x2tap.Controllers
{
	public class TUNTAPController
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
		///		SS 控制器
		/// </summary>
		public SSController SSController;

		/// <summary>
		///		SSR 控制器
		/// </summary>
		public SSRController SSRController;

		/// <summary>
		///		启动
		/// </summary>
		/// <param name="server">配置</param>
		/// <returns>是否成功</returns>
		public bool Start(Objects.Server server)
		{
			if (server.Type == "Shadowsocks")
			{
				SSController = new SSController();
				if (!SSController.Start(server))
				{
					return false;
				}
			}
			else if (server.Type == "ShadowsocksR")
			{
				SSRController = new SSRController();
				if (!SSRController.Start(server))
				{
					return false;
				}
			}

			Instance = new Process();
			Instance.StartInfo.WorkingDirectory = String.Format("{0}\\Bin", Directory.GetCurrentDirectory());
			Instance.StartInfo.FileName = String.Format("{0}\\Bin\\tun2socks.exe", Directory.GetCurrentDirectory());

			var dns = "1.1.1.1";
			if (Global.TUNTAP.UseCustomDNS)
			{
				dns = "";
				foreach (var value in Global.TUNTAP.DNS)
				{
					dns += value;
					dns += ',';
				}

				dns = dns.Trim();
				dns = dns.Substring(0, dns.Length - 1);
			}

			if (Global.TUNTAP.UseFakeDNS)
			{
				dns += " -fakeDns";
			}

			if (server.Type == "Socks5")
			{
				Instance.StartInfo.Arguments = String.Format("-proxyServer {0}:{1} -tunAddr {2} -tunMask {3} -tunGw {4} -tunDns {5}", server.Address, server.Port, Global.TUNTAP.Address, Global.TUNTAP.Netmask, Global.TUNTAP.Gateway, dns);
			}
			else if (server.Type == "VMess")
			{
				var data = new Objects.Information.Main();
				var outbound = new Objects.Information.Outbound()
				{
					tag = "defaultOutbound"
				};

				if (server.Type == "VMess")
				{
					outbound.protocol = "vmess";

					var settings = new Objects.Information.Protocol.Outbound.VMess();
					settings.vnext.Add(new Objects.Information.Protocol.Outbound.VMessServer()
					{
						address = server.Address,
						port = server.Port,
						users = new List<Objects.Information.Protocol.Outbound.VMessUser>()
						{
							new Objects.Information.Protocol.Outbound.VMessUser()
							{
								id = server.UserID,
								alterId = server.AlterID,
								security = "auto"
							}
						}
					});

					outbound.settings = settings;
					outbound.streamSettings = new Objects.Information.OutboundStream();
					outbound.streamSettings.network = server.TransferProtocol;
					outbound.streamSettings.security = server.TLSSecure ? "tls" : "none";

					switch (server.TransferProtocol)
					{
						case "tcp":
							outbound.streamSettings.network = "tcp";
							if (server.FakeType == "http")
							{
								var tcpSettings = new Objects.Information.OutboundStreamTCP()
								{
									header = new Objects.Information.OutboundStreamTCPHTTPHeader()
									{
										request = new Objects.Information.OutboundStreamTCPHTTPRequestHeader()
										{
											headers = new Dictionary<string, List<string>>()
											{
												{ "Host", new List<string>() { server.Host } }
											}
										}
									}
								};
							}
							break;
						case "kcp":
							outbound.streamSettings.network = "kcp";
							outbound.streamSettings.kcpSettings = new Objects.Information.OutboundStreamKCP()
							{
								header = new Dictionary<string, string>()
								{
									{ "type", server.FakeType }
								}
							};
							break;
						case "ws":
							outbound.streamSettings.network = "ws";
							var wsSettings = new Objects.Information.OutboundStreamWebSocket()
							{
								path = server.Path
							};

							if (server.Host != "")
							{
								wsSettings.headers.Add("Host", server.Host);
							}

							outbound.streamSettings.wsSettings = wsSettings;
							break;
						case "http":
							outbound.streamSettings.network = "http";
							var httpSettings = new Objects.Information.OutboundStreamHTTP2();

							if (server.Host != "")
							{
								httpSettings.host = new List<string>() { server.Host };
							}

							httpSettings.path = server.Path;
							outbound.streamSettings.httpSettings = httpSettings;
							break;
						case "quic":
							outbound.streamSettings.network = "quic";
							outbound.streamSettings.quicSettings = new Objects.Information.OutboundStreamQUIC()
							{
								header = new Dictionary<string, string>()
								{
									{ "type", server.FakeType }
								},
								security = server.QUICSecurity,
								key = server.QUICSecret
							};
							break;
						default:
							outbound.streamSettings.network = "tcp";
							break;
					}
				}

				data.outbounds.Add(outbound);
				File.WriteAllText("Data\\Last.json", Newtonsoft.Json.JsonConvert.SerializeObject(data));

				Instance.StartInfo.Arguments = String.Format("-proxyType v2ray -vconfig \"{0}\" -tunAddr {1} -tunMask {2} -tunGw {3} -tunDns {4}", String.Format("{0}\\Data\\Last.json", Directory.GetCurrentDirectory()), Global.TUNTAP.Address, Global.TUNTAP.Netmask, Global.TUNTAP.Gateway, dns);
			}
			else
			{
				Instance.StartInfo.Arguments = String.Format("-proxyServer 127.0.0.1:2810 -tunAddr {0} -tunMask {1} -tunGw {2} -tunDns {3}", Global.TUNTAP.Address, Global.TUNTAP.Netmask, Global.TUNTAP.Gateway, dns);
			}

			Instance.StartInfo.CreateNoWindow = true;
			Instance.StartInfo.RedirectStandardError = true;
			Instance.StartInfo.RedirectStandardInput = true;
			Instance.StartInfo.RedirectStandardOutput = true;
			Instance.StartInfo.UseShellExecute = false;
			Instance.EnableRaisingEvents = true;
			Instance.ErrorDataReceived += OnOutputDataReceived;
			Instance.OutputDataReceived += OnOutputDataReceived;
			State = Objects.State.Starting;
			Instance.Start();
			Instance.BeginErrorReadLine();
			Instance.BeginOutputReadLine();
			for (int i = 0; i < 1000; i++)
			{
				Thread.Sleep(10);

				if (State == Objects.State.Started)
				{
					return true;
				}

				if (State == Objects.State.Stopped)
				{
					Stop();
					return false;
				}
			}

			return false;
		}

		/// <summary>
		///		停止
		/// </summary>
		public void Stop()
		{
			if (Instance != null && !Instance.HasExited)
			{
				Instance.Kill();
			}

			if (SSController != null)
			{
				SSController.Stop();
			}

			if (SSRController != null)
			{
				SSRController.Stop();
			}
		}

		public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Data))
			{
				File.AppendAllText("Logging\\tun2socks.log", String.Format("{0}\r\n", e.Data.Trim()));

				if (State == Objects.State.Starting)
				{
					if (e.Data.Contains("Running"))
					{
						State = Objects.State.Started;
					}
					else if (e.Data.Contains("failed") || e.Data.Contains("invalid vconfig file"))
					{
						State = Objects.State.Stopped;
					}
				}
			}
		}
	}
}
