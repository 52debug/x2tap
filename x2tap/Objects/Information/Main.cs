using System.Collections.Generic;

namespace x2tap.Objects.Information
{
	public class Main
	{
		public Log log = new Log();

		public DNS dns = new DNS();

		public Routing routing = new Routing();

		public List<Inbound> inbounds = new List<Inbound>()
		{
			new Inbound()
			{
				port = 1091,
				protocol = "http",
				tag = "defaultInbound"
			},
			new Inbound()
			{
				port = 1092,
				protocol = "socks",
				tag = "socksInbound"
			}
		};

		public List<Outbound> outbounds = new List<Outbound>();
	}
}
