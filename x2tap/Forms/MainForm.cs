using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
			Utils.Logging.Info("正在加载服务器列表中");

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
			Global.Servers.Add(new Objects.Server()
			{
				Remark = "N3RO 是最棒的 ！",
				Type = "SS",
				Address = "www.baidu.com",
				Port = 443
			});

			// 加载翻译
			Utils.Logging.Info("正在加载翻译中");
			ServerToolStripDropDownButton.Text = Utils.MultiLanguage.Translate("Server");
			AddSocks5ServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [Socks5] Server");
			AddShadowsocksServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [Shadowsocks] Server");
			AddShadowsocksRServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [ShadowsocksR] Server");
			AddVMessServerToolStripMenuItem.Text = Utils.MultiLanguage.Translate("Add [VMess] Server");
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

			// 加载服务器列表
			InitServers();

			// 添加模式：绕过局域网和中国
			ModeComboBox.Items.Add(new Objects.Mode()
			{
				Name = Utils.MultiLanguage.Translate("Bypass LAN and China"),
				IsInternal = true,
				Type = 0,
				BypassChina = true
			});

			// 添加模式：绕过局域网
			ModeComboBox.Items.Add(new Objects.Mode()
			{
				Name = Utils.MultiLanguage.Translate("Bypass LAN"),
				IsInternal = true,
				Type = 0,
				BypassChina = false
			});

			// 加载模式列表
			Utils.Logging.Info("加载模式列表中");
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
					// 必须在没有启动的情况下才能进行测延迟
					if (State == Objects.State.Waiting || State == Objects.State.Stopped)
					{
						Utils.Logging.Info("正在测试所有服务器延迟中");

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

		}

		private void AddShadowsocksServerToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void AddShadowsocksRServerToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void AddVMessServerToolStripMenuItem_Click(object sender, EventArgs e)
		{

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
			Utils.Logging.Info("正在打开 GitHub 项目中");
			Process.Start("https://github.com/hacking001/x2tap");
		}

		private void TelegramGroupToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Utils.Logging.Info("正在打开 Telegram 群组中");
			Process.Start("https://t.me/x2tapChat");
		}

		private void TelegramChannelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Utils.Logging.Info("正在打开 Telegram 频道中");
			Process.Start("https://t.me/x2tap");
		}

		private void VersionToolStripLabel_Click(object sender, EventArgs e)
		{
			Utils.Logging.Info("正在打开 GitHub 发布页中");
			Process.Start("https://github.com/hacking001/x2tap/releases");
		}

		private void EditPictureBox_Click(object sender, EventArgs e)
		{
			if (ServerComboBox.SelectedIndex != -1)
			{

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
			MessageBox.Show(Utils.MultiLanguage.Translate("Waiting to add this feature"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			ControlButton.Enabled = false;
			StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(":") + Utils.MultiLanguage.Translate("Processing");

			if (State == Objects.State.Waiting || State == Objects.State.Stopped)
			{
				State = Objects.State.Starting;
				ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = false;

				Task.Run(() =>
				{
					State = Objects.State.Started;
					StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(":") + Utils.MultiLanguage.Translate("Started");
					ControlButton.Text = Utils.MultiLanguage.Translate("Stop");
					ControlButton.Enabled = true;
				});
			}
			else
			{
				State = Objects.State.Stopping;

				Task.Run(() =>
				{
					State = Objects.State.Stopped;
					StatusLabel.Text = Utils.MultiLanguage.Translate("Status") + Utils.MultiLanguage.Translate(":") + Utils.MultiLanguage.Translate("Stopped");
					ControlButton.Text = Utils.MultiLanguage.Translate("Start");
					ToolStrip.Enabled = ConfigurationGroupBox.Enabled = SettingsButton.Enabled = ControlButton.Enabled = true;
				});
			}
		}
	}
}
