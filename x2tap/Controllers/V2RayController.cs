using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace x2tap.Controllers
{
	public class V2RayController
	{
		/// <summary>
		///		进程实例（ShadowsocksR）
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
			if (!File.Exists(String.Format("{0}\\Bin\\v2ray.exe", Directory.GetCurrentDirectory())))
			{
				return false;
			}

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
			File.WriteAllText("Data\\V2RayLast.json", Newtonsoft.Json.JsonConvert.SerializeObject(data));

			Instance = new Process();
			Instance.StartInfo.WorkingDirectory = String.Format("{0}\\Bin", Directory.GetCurrentDirectory());
			Instance.StartInfo.FileName = String.Format("{0}\\Bin\\v2ray.exe", Directory.GetCurrentDirectory());
			Instance.StartInfo.Arguments = String.Format("-config \"{0}\\Data\\V2RayLast.json\"", Directory.GetCurrentDirectory());
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
				else if (State == Objects.State.Stopped)
				{
					Stop();
					return false;
				}
			}

			return false;
		}

		public void Stop()
		{
			if (!Instance.HasExited)
			{
				Instance.Kill();

				State = Objects.State.Stopped;
			}
		}

		public void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!String.IsNullOrEmpty(e.Data))
			{
				File.AppendAllText("Logging\\v2ray.log", String.Format("{0}\r\n", e.Data.Trim()));

				if (State == Objects.State.Starting)
				{
					if (e.Data.Contains("started"))
					{
						State = Objects.State.Started;
					}
					else if (e.Data.Contains("failed"))
					{
						State = Objects.State.Stopped;
					}
				}
			}
		}
	}
}
