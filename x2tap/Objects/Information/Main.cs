using System.Collections.Generic;

namespace x2tap.Objects.Information
{
	public class Main
	{
		public Log log = new Log();

		public DNS dns = new DNS();

		public Routing routing = new Routing();

		public List<Inbound> inbounds = new List<Inbound>();

		public List<Outbound> outbounds = new List<Outbound>();
	}
}
