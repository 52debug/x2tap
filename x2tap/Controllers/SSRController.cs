using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace x2tap.Controllers
{
	public class SSRController
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
			if (!File.Exists(String.Format("{0}\\Bin\\ShadowsocksR.exe", Directory.GetCurrentDirectory())))
			{
				return false;
			}

			File.WriteAllText("Data\\Last.json", Newtonsoft.Json.JsonConvert.SerializeObject(new Objects.Information.SSR()
			{
				server = server.Address,
				server_port = server.Port,
				password = server.Password,
				method = server.EncryptMethod,
				protocol = server.Protocol,
				protocol_param = server.ProtocolParam,
				obfs = server.OBFS,
				obfs_param = server.OBFSParam
			}));

			Instance = new Process();
			Instance.StartInfo.WorkingDirectory = String.Format("{0}\\Bin", Directory.GetCurrentDirectory());
			Instance.StartInfo.FileName = String.Format("{0}\\Bin\\ShadowsocksR.exe", Directory.GetCurrentDirectory());
			Instance.StartInfo.Arguments = String.Format("-c \"{0}\\Data\\Last.json\" -u", Directory.GetCurrentDirectory());

			Utils.Logging.Info(String.Format("SSR 启动参数：{0}", Instance.StartInfo.Arguments));

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
				File.AppendAllText("Logging\\shadowsocksr.log", String.Format("{0}\r\n", e.Data.Trim()));

				if (State == Objects.State.Starting)
				{
					if (e.Data.Contains("listening at"))
					{
						State = Objects.State.Started;
					}
					else if (e.Data.Contains("Invalid config path") || e.Data.Contains("usage"))
					{
						State = Objects.State.Stopped;
					}
				}
			}
		}
	}
}
