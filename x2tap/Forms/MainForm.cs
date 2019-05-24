using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace x2tap.Forms
{
	public partial class MainForm : Form
	{
		/// <summary>
		///		当前状态
		/// </summary>
		public Objects.State State = Objects.State.Waiting;

		/// <summary>
		///		服务器 IP 地址
		/// </summary>
		public IPAddress[] ServerAddresses = new IPAddress[0];

		/// <summary>
		///		TUN/TAP 控制器
		/// </summary>
		public Controllers.TUNTAPController TUNTAPController;

		/// <summary>
		///		上行流量
		/// </summary>
		public long UplinkBandwidth = 0;

		/// <summary>
		///		上一次的上行流量
		/// </summary>
		public long LastUplinkBandwidth = 0;

		/// <summary>
		///		下行流量
		/// </summary>
		public long DownlinkBandwidth = 0;


		/// <summary>
		///		上一次的下行流量
		/// </summary>
		public long LastDownlinkBandwidth = 0;

		public MainForm()
		{
			InitializeComponent();

			// 去他妈的跨线程提示
			CheckForIllegalCrossThreadCalls = false;
		}

		/// <summary>
		///		加载服务器列表
		/// </summary>
		public void InitServers()
		{
			// 先清理一下
			ServerComboBox.Items.Clear();

			// 遍历列表数组并导入
			foreach (var server in Global.Servers)
			{
				ServerComboBox.Items.Add(server);
			}

			// 如果项目数量大于零，则选中第一个项目
			if (ServerComboBox.Items.Count > 0)
			{
				ServerComboBox.SelectedIndex = 0;
			}
		}

		private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			var cbx = sender as ComboBox;

			// 绘制背景颜色
			e.Graphics.FillRectangle(new SolidBrush(Color.White), e.Bounds);

			if (e.Index >= 0)
			{
				// 绘制 备注/名称 字符串
				e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, new SolidBrush(Color.Black), e.Bounds);

				if (cbx.Items[e.Index] is Objects.Server)
				{
					var item = cbx.Items[e.Index] as Objects.Server;

					// 计算延迟底色
					SolidBrush brush;
					if (item.Delay == 999)
					{
						// 灰色
						brush = new SolidBrush(Color.Gray);
					}
					else if (item.Delay > 200)
					{
						// 红色
						brush = new SolidBrush(Color.Red);
					}
					else if (item.Delay > 80)
					{
						// 黄色
						brush = new SolidBrush(Color.Yellow);
					}
					else
					{
						// 绿色
						brush = new SolidBrush(Color.FromArgb(50, 255, 56));
					}

					// 绘制国家图标
					e.Graphics.DrawImage(Utils.GeoIP.GetCountryImageByISOCode(item.CountryCode), 393, e.Bounds.Y, Properties.Resources.flagCN.Size.Width, Properties.Resources.flagCN.Size.Height + 4);
					
					// 绘制延迟底色
					e.Graphics.FillRectangle(brush, 415, e.Bounds.Y, 50, e.Bounds.Height);

					// 绘制延迟字符串
					e.Graphics.DrawString(item.Delay.ToString(), cbx.Font, new SolidBrush(Color.Black), 417, e.Bounds.Y);
				}
			}
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			// 加载翻译
			ServerToolStripDropDownButton.Text = Utils.MultiLanguage.Translate("Server");
			AddSocks5ServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [Socks5] Server");
			AddShadowsocksServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [Shadowsocks] Server");
			AddShadowsocksRServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [ShadowsocksR] Server");
			AddVMessServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [VMess] Server");
			AddServersFromClipboardToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add Servers From Clipboard");
			SubscribeToolStripDropDownButton.Text = Utils.MultiLanguage.Translate("Subscribe");
			AddSubscribeLinkToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add Subscribe Link");
			ManageSubscribeLinksToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Manage Subscribe Links");
			UpdateServersFromSubscriptionsToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Update Servers From Subscriptions");
			AboutToolStripDropDownButton.Text = Utils.MultiLanguage.Translate("About");
			GitHubProjectToolStripMenuItem.Text = Utils.MultiLanguage.Translate("GitHub Project");
			TelegramGroupToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Telegram Group");
			TelegramChannelToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Telegram Channel");
			ConfigurationGroupBox.Text = Utils.MultiLanguage.Translate("Configuration");
			ServerLabel.Text = Utils.MultiLanguage.Translate("Server");
			ModeLabel.Text = Utils.MultiLanguage.Translate("Mode");
			SettingsButton.Text = Utils.MultiLanguage.Translate("Settings");
			ControlButton.Text = Utils.MultiLanguage.Translate("Start");
			UplinkLabel.Text = $"↑{Utils.MultiLanguage.Translate(": ")}0KB/s";
			DownlinkLabel.Text = $"↓{Utils.MultiLanguage.Translate(": ")}0KB/s";
			StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Waiting for command");

			Utils.Configuration.SearchOutbounds();

			// 加载服务器列表
			InitServers();

			// 添加模式：绕过局域网和中国
			Global.Modes.Insert(0, new Objects.Mode()
			{
				Name = Utils.MultiLanguage.Translate("Bypass LAN and China"),
				IsInternal = true,
				Type = 0,
				BypassChina = true
			});

			// 添加模式：绕过局域网
			Global.Modes.Insert(1, new Objects.Mode()
			{
				Name = Utils.MultiLanguage.Translate("Bypass LAN"),
				IsInternal = true,
				Type = 0,
				BypassChina = false
			});

			// 加载模式列表
			foreach (var mode in Global.Modes)
			{
				ModeComboBox.Items.Add(mode);
			}
			ModeComboBox.SelectedIndex = 0;

			// 测延迟线程
			Task.Run(() =>
			{
				while (true)
				{
					// 必须在没有启动和窗体显示的情况下才能进行测延迟
					if ((State == Objects.State.Waiting || State == Objects.State.Stopped) && Visible)
					{
						// 遍历服务器列表
						foreach (var server in Global.Servers)
						{
							// 开一个 Task 来处理测延迟，防止阻塞
							Task.Run(server.Test);
						}
					}

					// 延迟 10 秒
					Thread.Sleep(10000);
				}
			});
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				// 防止用户在程序处理过程中关闭程序
				if (State != Objects.State.Waiting && State != Objects.State.Stopped)
				{
					if (State == Objects.State.Started)
					{
						// 如果已启动，提示需要先点击关闭按钮
						MessageBox.Show(Utils.MultiLanguage.Translate("Please click the Stop button first"), Utils.MultiLanguage.Translate("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
					else
					{
						// 如果正在 启动中、停止中，提示请等待当前操作完成
						MessageBox.Show(Utils.MultiLanguage.Translate("Please wait for the current operation to complete"), Utils.MultiLanguage.Translate("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}

					e.Cancel = true;
					return;
				}
			}

			// 保存配置
			Utils.Configuration.Save();
		}

		private void AddSocks5ServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Server.Socks5().Show();
			Hide();
		}

		private void AddShadowsocksServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Server.Shadowsocks().Show();
			Hide();
		}

		private void AddShadowsocksRServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Server.ShadowsocksR().Show();
			Hide();
		}

		private void AddVMessServerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new Server.VMess().Show();
			Hide();
		}

		private void AddServersFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var texts = Clipboard.GetText();

			using (var sr = new StringReader(texts))
			{
				string text;

				while ((text = sr.ReadLine()) != null)
				{
					var result = Utils.ShareLink.Parse(text);

					if (result != null)
					{
						Global.Servers.Add(result);
					}
				}
			}

			InitServers();
		}

		private void AddSubscribeLinkToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void ManageSubscribeLinksToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void UpdateServersFromSubscriptionsToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void GitHubProjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/hacking001/x2tap");
		}

		private void TelegramGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://t.me/x2tapChat");
		}

		private void TelegramChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("https://t.me/x2tap");
		}

		private void VersionToolStripLabel_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/hacking001/x2tap/releases");
		}

		private void EditPictureBox_Click(object sender, EventArgs e)
		{
			if (ServerComboBox.SelectedIndex != -1)
			{
				var item = Global.Servers[ServerComboBox.SelectedIndex];

				if (ServerComboBox.Text.StartsWith("[S5]"))
				{
					new Server.Socks5(ServerComboBox.SelectedIndex).Show();
				}
				else if (ServerComboBox.Text.StartsWith("[SS]"))
				{
					new Server.Shadowsocks(ServerComboBox.SelectedIndex).Show();
				}
				else if (ServerComboBox.Text.StartsWith("[SR]"))
				{
					new Server.ShadowsocksR(ServerComboBox.SelectedIndex).Show();
				}
				else if (ServerComboBox.Text.StartsWith("[V2]"))
				{
					new Server.VMess(ServerComboBox.SelectedIndex).Show();
				}
				else
				{
					return;
				}

				Hide();
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please select an server"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void DeletePictureBox_Click(object sender, EventArgs e)
		{
			if (ServerComboBox.SelectedIndex != -1)
			{
				var index = ServerComboBox.SelectedIndex;

				// 从全局列表中删除
				Global.Servers.RemoveAt(index);

				// 从服务器组合盒中删除
				ServerComboBox.Items.RemoveAt(index);

				if (ServerComboBox.Items.Count > 0)
				{
					ServerComboBox.SelectedIndex = (index != 0) ? index - 1 : index;
				}
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please select an server"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void SpeedPictureBox_Click(object sender, EventArgs e)
		{
			if (ServerComboBox.SelectedIndex != -1)
			{
				Task.Run(Global.Servers[ServerComboBox.SelectedIndex].Test);
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please select an server"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void SettingsButton_Click(object sender, EventArgs e)
		{
			new SettingForm().Show();
			Hide();
		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			if (ServerComboBox.SelectedIndex != -1)
			{
				ControlButton.Enabled = false;
				StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Processing");

				if (State == Objects.State.Waiting || State == Objects.State.Stopped)
				{
					State = Objects.State.Starting;
					ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = false;

					Task.Run(() =>
					{
						var server = ServerComboBox.SelectedItem as Objects.Server;
						var mode = ModeComboBox.SelectedItem as Objects.Mode;

						try
						{
							// 查询服务器 IP 地址
							var destination = Dns.GetHostAddressesAsync(server.Address);
							if (destination.Wait(1000))
							{
								if (destination.Result.Length == 0)
								{
									State = Objects.State.Stopped;
									StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Resolve server IP failed");
									ControlButton.Text = Utils.MultiLanguage.Translate("Start");
									ControlButton.Enabled = true;
									ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = true;
									return;
								}

								ServerAddresses = destination.Result;
							}

							// 启动实例
							TUNTAPController = new Controllers.TUNTAPController();
							if (TUNTAPController.Start(ServerComboBox.SelectedItem as Objects.Server))
							{
								TUNTAPController.Instance.Exited += OnExited;

								if (server.Type == "Shadowsocks")
								{
									TUNTAPController.SSController.Instance.Exited += OnExited;
								}
								else if (server.Type == "ShadowsocksR")
								{
									TUNTAPController.SRController.Instance.Exited += OnExited;
								}
							}
							else
							{
								State = Objects.State.Stopped;
								StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Starting failed");
								ControlButton.Text = Utils.MultiLanguage.Translate("Start");
								ControlButton.Enabled = true;
								ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = true;
								return;
							}

							// 让服务器 IP 走直连
							foreach (var address in ServerAddresses)
							{
								if (!IPAddress.IsLoopback(address))
								{
									NativeMethods.CreateRoute(address.ToString(), 32, Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
								}
							}

							if (mode.Type == 0) // 处理仅规则内走直连
							{
								// 创建默认路由
								if (!NativeMethods.CreateRoute("0.0.0.0", 0, Global.TUNTAP.Gateway.ToString(), Global.TUNTAP.Index, 10))
								{
									State = Objects.State.Stopped;

									foreach (var address in ServerAddresses)
									{
										NativeMethods.DeleteRoute(address.ToString(), 32, Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
									}

									TUNTAPController.Stop();
									StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Setting route table failed");
									ControlButton.Text = Utils.MultiLanguage.Translate("Start");
									ControlButton.Enabled = true;
									ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = true;
									return;
								}
								
								foreach (var ip in mode.Rule)
								{
									var info = ip.Split('/');
									
									if (info.Length == 2)
									{
										if (int.TryParse(info[1], out var prefix))
										{
											NativeMethods.CreateRoute(info[0], prefix, Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
										}
									}
								}
							}
							else if (mode.Type == 1) // 处理仅规则内走代理
							{
								foreach (var ip in mode.Rule)
								{
									var info = ip.Split('/');

									if (info.Length == 2)
									{
										if (int.TryParse(info[1], out var prefix))
										{
											NativeMethods.CreateRoute(info[0], prefix, Global.TUNTAP.Gateway.ToString(), Global.TUNTAP.Index);
										}
									}
								}
							}

							// 处理模式的绕过中国
							if (mode.BypassChina)
							{
								using (var sr = new StringReader(Encoding.UTF8.GetString(Properties.Resources.CNIP)))
								{
									string text;

									while ((text = sr.ReadLine()) != null)
									{
										var info = text.Split('/');

										NativeMethods.CreateRoute(info[0], int.Parse(info[1]), Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
									}
								}
							}

							// 设置状态：已启动
							State = Objects.State.Started;

							// 速度显示线程
							Task.Run(() =>
							{
								var first = true;

								while (State == Objects.State.Started)
								{
									var stats = Global.TUNTAP.Adapter.GetIPv4Statistics();

									if (first)
									{
										LastUplinkBandwidth = stats.BytesSent;
										LastDownlinkBandwidth = stats.BytesReceived;

										first = false;
										Thread.Sleep(1000);
										continue;
									}

									UplinkLabel.Text = String.Format("↑{0}{1}/s", Utils.MultiLanguage.Translate(": "), Utils.Util.ComputeBandwidth(stats.BytesSent - UplinkBandwidth));
									DownlinkLabel.Text = String.Format("↓{0}{1}/s", Utils.MultiLanguage.Translate(": "), Utils.Util.ComputeBandwidth(stats.BytesReceived - DownlinkBandwidth));
									UsedBandwidthLabel.Text = String.Format("{0}{1}{2}", Utils.MultiLanguage.Translate("Used"), Utils.MultiLanguage.Translate(": "), Utils.Util.ComputeBandwidth(stats.BytesSent + stats.BytesReceived - LastUplinkBandwidth - LastDownlinkBandwidth));

									UplinkBandwidth = stats.BytesSent;
									DownlinkBandwidth = stats.BytesReceived;

									Thread.Sleep(1000);
								}
							});

							StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Started");
							ControlButton.Text = Utils.MultiLanguage.Translate("Stop");
							ControlButton.Enabled = true;
						}
						catch (Exception ex)
						{
							Utils.Logging.Info(ex.ToString());

							if (TUNTAPController != null)
							{
								TUNTAPController.Stop();
							}

							StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Unknown error");
							ControlButton.Text = Utils.MultiLanguage.Translate("Start");
							ControlButton.Enabled = true;
							ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = true;
							return;
						}
					});
				}
				else
				{
					State = Objects.State.Stopping;

					Task.Run(() =>
					{
						var server = ServerComboBox.SelectedItem as Objects.Server;
						var mode = ModeComboBox.SelectedItem as Objects.Mode;

						TUNTAPController.Stop();

						if (mode.Type == 0)
						{
							NativeMethods.DeleteRoute("0.0.0.0", 0, Global.TUNTAP.Gateway.ToString(), Global.TUNTAP.Index, 10);

							foreach (var ip in mode.Rule)
							{
								var info = ip.Split('/');

								if (info.Length == 2)
								{
									if (int.TryParse(info[1], out var prefix))
									{
										NativeMethods.DeleteRoute(info[0], prefix, Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
									}
								}
							}
						}
						else if (mode.Type == 1)
						{
							foreach (var ip in mode.Rule)
							{
								var info = ip.Split('/');

								if (info.Length == 2)
								{
									if (int.TryParse(info[1], out var prefix))
									{
										NativeMethods.DeleteRoute(info[0], prefix, Global.TUNTAP.Gateway.ToString(), Global.TUNTAP.Index);
									}
								}
							}
						}

						if (mode.BypassChina)
						{
							using (var sr = new StringReader(Encoding.UTF8.GetString(Properties.Resources.CNIP)))
							{
								string text;

								while ((text = sr.ReadLine()) != null)
								{
									var info = text.Split('/');

									NativeMethods.DeleteRoute(info[0], int.Parse(info[1]), Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
								}
							}
						}

						foreach (var address in ServerAddresses)
						{
							if (!IPAddress.IsLoopback(address))
							{
								NativeMethods.DeleteRoute(address.ToString(), 32, Global.Adapter.Gateway.ToString(), Global.Adapter.Index);
							}
						}

						State = Objects.State.Stopped;
						StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(": ") + Utils.MultiLanguage.Translate("Stopped");
						ControlButton.Text = Utils.MultiLanguage.Translate("Start");
						ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = ControlButton.Enabled = true;
					});
				}
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please select an server"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		public void OnExited(object sender, EventArgs e)
		{
			if (State == Objects.State.Started)
			{
				ControlButton.PerformClick();
			}
		}
	}
}
