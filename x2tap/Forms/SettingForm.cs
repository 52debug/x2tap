using System;
using System.Net;
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
			TUNTAPAddressLabel.Text = Utils.MultiLanguage.Translate("Address");
			TUNTAPNetmaskLabel.Text = Utils.MultiLanguage.Translate("Netmask");
			TUNTAPGatewayLabel.Text = Utils.MultiLanguage.Translate("Gateway");
			TUNTAPUseCustomDNSCheckBox.Text = Utils.MultiLanguage.Translate("Use Custom DNS");
			GlobalBypassIPsButton.Text = Utils.MultiLanguage.Translate("Global Bypass IPs");
			ControlButton.Text = Utils.MultiLanguage.Translate("Save");

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
			TUNTAPUseCustomDNSCheckBox.Checked = Global.TUNTAP.UseCustomDNS;
		}

		private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Global.MainForm.Show();
		}

		private void GlobalBypassIPsButton_Click(object sender, EventArgs e)
		{

		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			Global.TUNTAP.Address = IPAddress.Parse(TUNTAPAddressTextBox.Text);
			Global.TUNTAP.Netmask = IPAddress.Parse(TUNTAPNetmaskTextBox.Text);
			Global.TUNTAP.Gateway = IPAddress.Parse(TUNTAPGatewayTextBox.Text);

			Global.TUNTAP.DNS.Clear();
			foreach (var ip in TUNTAPDNSTextBox.Text.Split(','))
			{
				Global.TUNTAP.DNS.Add(IPAddress.Parse(ip));
			}

			Global.TUNTAP.UseCustomDNS = TUNTAPUseCustomDNSCheckBox.Checked;

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
