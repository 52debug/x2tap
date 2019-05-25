using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace x2tap.Forms
{
	public partial class GlobalBypassIPForm : Form
	{
		public GlobalBypassIPForm()
		{
			InitializeComponent();
		}

		private void GlobalBypassIPForm_Load(object sender, EventArgs e)
		{
			IPListBox.Items.AddRange(Global.BypassIPs.ToArray());

			for (var i = 32; i >= 1; i--)
			{
				PrefixComboBox.Items.Add(i);
			}
			PrefixComboBox.SelectedIndex = 0;
		}

		private void GlobalBypassIPForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Global.SettingForm.Show();
		}

		private void AddButton_Click(object sender, EventArgs e)
		{	
			if (!String.IsNullOrEmpty(IPTextBox.Text))
			{
				if (IPAddress.TryParse(IPTextBox.Text, out var address))
				{
					IPListBox.Items.Add(String.Format("{0}/{1}", address, PrefixComboBox.SelectedItem));
				}
				else
				{
					MessageBox.Show(Utils.MultiLanguage.Translate("Please enter a correct IP address"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please enter an IP"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void DeleteButton_Click(object sender, EventArgs e)
		{
			if (IPListBox.SelectedIndex != -1)
			{
				IPListBox.Items.RemoveAt(IPListBox.SelectedIndex);
			}
			else
			{
				MessageBox.Show(Utils.MultiLanguage.Translate("Please select an IP"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			Global.BypassIPs.Clear();
			foreach (var ip in IPListBox.Items)
			{
				Global.BypassIPs.Add(ip as String);
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
