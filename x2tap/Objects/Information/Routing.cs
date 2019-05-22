using System.Collections.Generic;

namespace x2tap.Objects.Information
{
	public class Routing
	{
		public string strategy = "rules";

		public RoutingSetting settings = new RoutingSetting();
	}

	public class RoutingSetting
	{
		public string domainStrategy = "IPIfNonMatch";

		public List<RoutingRule> rules = new List<RoutingRule>();
	}

	public class RoutingRule
	{
		public string type = "field";

		public List<string> domain;

		public List<string> ip;

		public string outboundTag;
	}
}
