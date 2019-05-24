using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace x2tap.Utils
{
	public static class ShareLink
	{
		/// <summary>
		///		URL 传输安全的 Base64 解码
		/// </summary>
		/// <param name="text">需要解码的字符串</param>
		/// <returns>解码后的字符串</returns>
		public static string UrlSafeBase64Decode(string text)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(text.Replace("-", "+").Replace("_", "/").PadRight(text.Length + (4 - text.Length % 4) % 4, '=')));
		}

		/// <summary>
		///		生成分享链接
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static string Generate(Objects.Server server)
		{
			return String.Empty;
		}

		/// <summary>
		///		解析分享链接
		/// </summary>
		/// <param name="server"></param>
		/// <returns></returns>
		public static Objects.Server Parse(string text)
		{
			var data = new Objects.Server();

			if (text.StartsWith("ss://"))
			{
				data.Type = "Shadowsocks";

				try
				{
					var finder = new Regex("^(?i)ss://([A-Za-z0-9+-/=_]+)(#(.+))?", RegexOptions.IgnoreCase);
					var parser = new Regex("^((?<method>.+):(?<password>.*)@(?<address>.+?):(?<port>\\d+?))$", RegexOptions.IgnoreCase);
					var match = finder.Match(text);
					if (!match.Success)
					{
						throw new FormatException();
					}

					match = parser.Match(UrlSafeBase64Decode(match.Groups[1].Value));

					data.Address = match.Groups["address"].Value;
					data.Port = int.Parse(match.Groups["port"].Value);
					data.Password = match.Groups["password"].Value;
					data.EncryptMethod = match.Groups["method"].Value;

					if (!Global.EncryptMethods.SS.Contains(data.EncryptMethod))
					{
						Logging.Info(String.Format("不支持的 SS 加密方式：{0}", data.EncryptMethod));
						return null;
					}
				}
				catch (FormatException)
				{
					var uri = new Uri(text);
					var userinfo = UrlSafeBase64Decode(uri.UserInfo).Split(':');

					data.Remark = uri.GetComponents(UriComponents.Fragment, UriFormat.Unescaped);
					data.Address = uri.IdnHost;
					data.Port = uri.Port;

					if (!Global.EncryptMethods.SS.Contains(data.EncryptMethod))
					{
						Logging.Info(String.Format("不支持的 SS 加密方式：{0}", data.EncryptMethod));
						return null;
					}
				}
			}
			else if (text.StartsWith("ssr://"))
			{
				data.Type = "ShadowsocksR";

				text = text.Substring(6);
				var shadowsocksr = UrlSafeBase64Decode(text).Split(':');

				data.Address = shadowsocksr[0];
				data.Port = int.Parse(shadowsocksr[1]);
				data.Protocol = shadowsocksr[2];
				if (!Global.Protocols.Contains(data.Protocol))
				{
					Logging.Info(String.Format("不支持的 SSR 协议：{0}", data.Protocol));
					return null;
				}

				data.EncryptMethod = shadowsocksr[3];
				if (!Global.EncryptMethods.SSR.Contains(data.EncryptMethod))
				{
					Logging.Info(String.Format("不支持的 SSR 加密方式：{0}", data.EncryptMethod));
					return null;
				}

				data.OBFS = shadowsocksr[4];
				if (!Global.OBFSs.Contains(data.OBFS))
				{
					Logging.Info(String.Format("不支持的 SSR 混淆：{0}", data.OBFS));
					return null;
				}

				var info = shadowsocksr[5].Split('/');
				data.Password = UrlSafeBase64Decode(info[0]);

				var dict = new Dictionary<string, string>();
				foreach (var str in info[1].Substring(1).Split('&'))
				{
					var splited = str.Split('=');

					dict.Add(splited[0], splited[1]);
				}

				if (dict.ContainsKey("remarks"))
				{
					data.Remark = UrlSafeBase64Decode(dict["remarks"]);
				}

				if (dict.ContainsKey("protoparam"))
				{
					data.ProtocolParam = UrlSafeBase64Decode(dict["protoparam"]);
				}

				if (dict.ContainsKey("obfsparam"))
				{
					data.OBFSParam = UrlSafeBase64Decode(dict["obfsparam"]);
				}
			}
			else if (text.StartsWith("vmess://"))
			{
				data.Type = "VMess";

				text = text.Substring(8);
				var vmess = SimpleJSON.JSON.Parse(UrlSafeBase64Decode(text));

				data.Remark = vmess["ps"].Value;
				data.Address = vmess["add"].Value;
				data.Port = vmess["port"].AsInt;
				data.UserID = vmess["id"].Value;
				data.AlterID = vmess["aid"].AsInt;

				data.TransferProtocol = vmess["net"].Value;
				if (!Global.TransferProtocols.Contains(data.TransferProtocol))
				{
					Logging.Info(String.Format("不支持的 VMess 传输协议：{0}", data.TransferProtocol));
					return null;
				}

				data.FakeType = vmess["type"].Value;
				if (!Global.FakeTypes.Contains(data.FakeType))
				{
					Logging.Info(String.Format("不支持的 VMess 伪装类型：{0}", data.FakeType));
					return null;
				}

				data.Host = vmess["host"].Value;
				data.Path = vmess["path"].Value;
				data.TLSSecure = vmess["tls"].Value == "tls";
			}
			else
			{
				return null;
			}

			return data;
		}
	}
}
