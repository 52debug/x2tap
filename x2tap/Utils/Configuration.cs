using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace x2tap.Utils
{
	/// <summary>
	///		配置处理类
	/// </summary>
	public static class Configuration
	{
		/// <summary>
		///		初始化配置
		/// </summary>
		public static void Init()
		{
			if (File.Exists("Data\\Servers.json"))
			{
				Global.Servers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Objects.Server>>(File.ReadAllText("Data\\Servers.json"));
			}
		}

		/// <summary>
		///		保存配置
		/// </summary>
		public static void Save()
		{
			// 写入服务器配置信息
			Utils.Logging.Info("正在写入服务器配置信息");
			File.WriteAllText("Data\\Servers.json", Newtonsoft.Json.JsonConvert.SerializeObject(Global.Servers));
		}
	}
}
