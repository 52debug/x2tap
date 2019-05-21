using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace x2tap
{
	public static class x2tap
	{
		/// <summary>
		///		应用程序的主入口点
		/// </summary>
		[STAThread]
		public static void Main()
		{
#if DEBUG
			if (Directory.Exists("Logging"))
			{
				Directory.Delete("Logging", true);
			}

			if (Directory.Exists("Language"))
			{
				Directory.Delete("Language", true);
			}

			if (Directory.Exists("Data"))
			{
				Directory.Delete("Data", true);
			}
#endif

			// 预先创建文件夹：日志
			if (!File.Exists("Logging"))
			{
				Directory.CreateDirectory("Logging");
			}

			// 程序正在启动中
			Utils.Logging.Info("程序正在启动中");

			// 预先创建文件夹：语言
			if (!Directory.Exists("Language"))
			{
				Utils.Logging.Info("检测到 语言 目录不存在，正在创建中");
				Directory.CreateDirectory("Language");
			}

			// 预先创建文件夹：数据
			if (!Directory.Exists("Data"))
			{
				Utils.Logging.Info("检测到 数据 目录不存在，正在创建中");
				Directory.CreateDirectory("Data");
			}

			// 如果当前系统语言为中文，先从程序自带的资源中加载中文翻译
			if (CultureInfo.InstalledUICulture.Name == "zh-CN")
			{
				// Global.i18N = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Properties.Resources.zh_CN));
			}

			// 如果存在对应的语言文件，就使用语言的文件的翻译
			Utils.Logging.Info(String.Format("检测到当前语言为 {0}", CultureInfo.InstalledUICulture.Name));
			if (File.Exists(String.Format("Language\\" + CultureInfo.InstalledUICulture.Name)))
			{
				Utils.Logging.Info(String.Format("检测到存在语言 {0} 的翻译文件，正在加载中", CultureInfo.InstalledUICulture.Name));
				Global.i18N = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Language\\" + CultureInfo.InstalledUICulture.Name));
			}

			// 加载 IP 数据库
			if (!File.Exists("Data\\MaxMind-GeoLite2.mmdb"))
			{
				Utils.Logging.Info("检测到 IP 数据库不存在，正在从资源读取并写入到磁盘中");
				File.WriteAllBytes("Data\\MaxMind-GeoLite2.mmdb", Properties.Resources.MaxMind_GeoLite2);
			}

			Utils.Logging.Info("正在加载 IP 数据库中");
			Utils.GeoIP.Database = new MaxMind.GeoIP2.DatabaseReader("Data\\MaxMind-GeoLite2.mmdb");

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(Global.MainForm = new Forms.MainForm());
		}
	}
}
