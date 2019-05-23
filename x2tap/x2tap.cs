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
			// 预先创建文件夹：日志
			if (!File.Exists("Logging"))
			{
				Directory.CreateDirectory("Logging");
			}

			// 预先创建文件夹：语言
			if (!Directory.Exists("Language"))
			{
				Directory.CreateDirectory("Language");
			}

			// 预先创建文件夹：数据
			if (!Directory.Exists("Data"))
			{
				Directory.CreateDirectory("Data");
			}

			// 预先创建文件夹：模式
			if (!Directory.Exists("Mode"))
			{
				Directory.CreateDirectory("Mode");
			}

			// 如果当前系统语言为中文，先从程序自带的资源中加载中文翻译
			if (CultureInfo.InstalledUICulture.Name == "zh-CN")
			{
				Global.i18N = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(Encoding.UTF8.GetString(Properties.Resources.zh_CN));
			}

			// 如果存在对应的语言文件，就使用语言的文件的翻译
			if (File.Exists(String.Format("Language\\" + CultureInfo.InstalledUICulture.Name)))
			{
				Global.i18N = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("Language\\" + CultureInfo.InstalledUICulture.Name));
			}

			// 加载 IP 数据库
			if (!File.Exists("Data\\MaxMind-GeoLite2.mmdb"))
			{
				File.WriteAllBytes("Data\\MaxMind-GeoLite2.mmdb", Properties.Resources.MaxMind_GeoLite2);
			}
			Utils.GeoIP.Database = new MaxMind.GeoIP2.DatabaseReader("Data\\MaxMind-GeoLite2.mmdb");

			// 加载配置文件
			Utils.Configuration.Init();

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(Global.MainForm = new Forms.MainForm());
		}
	}
}
