using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace x2tap.Forms
{
	public partial class SettingForm : Form
	{
		public SettingForm()
		{
			InitializeComponent();
		}

		private void SettingForm_Load(object sender, EventArgs e)
		{
			TUNTAPAddressTextBox.Text = Global.TUNTAP.Address.ToString();
			TUNTAPNetmaskTextBox.Text = Global.TUNTAP.Netmask.ToString();
			TUNTAPGatewayTextBox.Text = Global.TUNTAP.Gateway.ToString();

			var dns = "";
			foreach (var ip in Global.TUNTAP.DNS)
			{
				dns += ip.ToString();
				dns += ',';
			}
			dns = dns.Trim();
			TUNTAPDNSTextBox.Text = dns.Substring(0, dns.Length - 1);
		}

		private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Global.MainForm.Show();
		}
	}
}
