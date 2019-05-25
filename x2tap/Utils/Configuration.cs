using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Windows.Forms;

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
			if (File.Exists("Data\\TUNTAP.ini"))
			{
				var parser = new IniParser.FileIniDataParser();
				var data = parser.ReadFile("Data\\TUNTAP.ini");

				if (IPAddress.TryParse(data["Generic"]["Address"], out var address))
				{
					Global.TUNTAP.Address = address;
				}

				if (IPAddress.TryParse(data["Generic"]["Netmask"], out var netmask))
				{
					Global.TUNTAP.Netmask = netmask;
				}

				if (IPAddress.TryParse(data["Generic"]["Gateway"], out var gateway))
				{
					Global.TUNTAP.Gateway = gateway;
				}

				var dns = new List<IPAddress>();
				foreach (var ip in data["Generic"]["DNS"].Split(','))
				{
					if (IPAddress.TryParse(ip, out var value))
					{
						dns.Add(value);
					}
				}

				if (Boolean.TryParse(data["Generic"]["UseCustomDNS"], out var useCustomDNS))
				{
					Global.TUNTAP.UseCustomDNS = useCustomDNS;
				}

				if (dns.Count > 0)
				{
					Global.TUNTAP.DNS = dns;
				}
			}

			if (File.Exists("Data\\Servers.json"))
			{
				Global.Servers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Objects.Server>>(File.ReadAllText("Data\\Servers.json"));
			}

			foreach (var name in Directory.GetFiles("Mode", "*.txt"))
			{
				var mode = new Objects.Mode();

				using (var sr = new StringReader(File.ReadAllText(name)))
				{
					var i = 0;
					var ok = true;
					string text;

					while ((text = sr.ReadLine()) != null)
					{
						if (i == 0)
						{
							var splited = text.Substring(1).Split(',');
							if (splited.Length == 3)
							{
								mode.Name = splited[0].Trim();
								mode.Type = int.Parse(splited[1]);
								mode.BypassChina = (int.Parse(splited[2]) == 1) ? true : false;
							}
							else
							{
								ok = false;
								break;
							}
						}
						else
						{
							if (!text.StartsWith("#"))
							{
								mode.Rule.Add(text.Trim());
							}
						}

						i++;
					}

					if (!ok)
					{
						break;
					}
				}

				Global.Modes.Add(mode);
			}
		}

		/// <summary>
		///		保存配置
		/// </summary>
		public static void Save()
		{
			if (!File.Exists("Data\\TUNTAP.ini"))
			{
				File.WriteAllBytes("Data\\TUNTAP.ini", Properties.Resources.defaultTUNTAP);
			}

			var parser = new IniParser.FileIniDataParser();
			var data = parser.ReadFile("Data\\TUNTAP.ini");

			data["Generic"]["Address"] = Global.TUNTAP.Address.ToString();
			data["Generic"]["Netmask"] = Global.TUNTAP.Netmask.ToString();
			data["Generic"]["Gateway"] = Global.TUNTAP.Gateway.ToString();

			var dns = "";
			foreach (var ip in Global.TUNTAP.DNS)
			{
				dns += ip.ToString();
				dns += ',';
			}
			dns = dns.Trim();
			data["Generic"]["DNS"] = dns.Substring(0, dns.Length - 1);

			data["Generic"]["UseCustomDNS"] = Global.TUNTAP.UseCustomDNS.ToString();

			parser.WriteFile("Data\\TUNTAP.ini", data);

			File.WriteAllText("Data\\Servers.json", Newtonsoft.Json.JsonConvert.SerializeObject(Global.Servers));
		}

		/// <summary>
		///		搜索出口
		/// </summary>
		public static void SearchOutbounds()
		{
			Logging.Info("正在搜索出口中");

			using (var client = new UdpClient("114.114.114.114", 53))
			{
				var address = ((IPEndPoint)client.Client.LocalEndPoint).Address;
				Global.Adapter.Address = address;

				Logging.Info($"当前 IP 地址：{Global.Adapter.Address}");

				var addressGeted = false;

				var adapters = NetworkInterface.GetAllNetworkInterfaces();
				foreach (var adapter in adapters)
				{
					var properties = adapter.GetIPProperties();

					foreach (var information in properties.UnicastAddresses)
					{
						if (information.Address.AddressFamily == AddressFamily.InterNetwork && Equals(information.Address, address))
						{
							addressGeted = true;
						}
					}

					foreach (var information in properties.GatewayAddresses)
					{
						if (information.Address.AddressFamily == AddressFamily.InterNetwork && addressGeted)
						{
							Global.Adapter.Index = properties.GetIPv4Properties().Index;
							Global.Adapter.Gateway = information.Address;

							Logging.Info($"当前 网关 地址：{Global.Adapter.Gateway}");
							break;
						}
					}

					if (addressGeted)
					{
						break;
					}
				}
			}

			// 搜索 TUN/TAP 适配器的索引
			Global.TUNTAP.ComponentID = TUNTAP.GetComponentID();
			if (String.IsNullOrEmpty(Global.TUNTAP.ComponentID))
			{
				MessageBox.Show(MultiLanguage.Translate("Please install TAP-Windows and create an TUN/TAP adapter manually"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				Environment.Exit(1);
			}

			var name = TUNTAP.GetName(Global.TUNTAP.ComponentID);
			foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (adapter.Name == name)
				{
					Global.TUNTAP.Adapter = adapter;
					Global.TUNTAP.Index = adapter.GetIPProperties().GetIPv4Properties().Index;

					break;
				}
			}
		}
	}
}
