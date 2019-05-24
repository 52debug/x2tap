﻿using System;
using System.Windows.Forms;

namespace x2tap.Forms.Server
{
	public partial class Socks5 : Form
	{
		public int Index;

		public Socks5(int index = -1)
		{
			InitializeComponent();

			Index = index;
		}

		private void Socks5_Load(object sender, EventArgs e)
		{
			ConfigurationGroupBox.Text = Utils.MultiLanguage.Translate("Configuration");
			RemarkLabel.Text = Utils.MultiLanguage.Translate("Remark");
			AddressLabel.Text = Utils.MultiLanguage.Translate("Address");
			ControlButton.Text = Utils.MultiLanguage.Translate("Save");

			if (Index != -1)
			{
				RemarkTextBox.Text = Global.Servers[Index].Remark;
				AddressTextBox.Text = Global.Servers[Index].Address;
				PortTextBox.Text = Global.Servers[Index].Port.ToString();
			}
		}

		private void Socks5_FormClosing(object sender, FormClosingEventArgs e)
		{
			Global.MainForm.InitServers();
			Global.MainForm.Show();
		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			if (Index == -1)
			{
				Global.Servers.Add(new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					Type = "Socks5",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text)
				});
			}
			else
			{
				Global.Servers[Index] = new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = Global.Servers[Index].GroupID,
					Type = "Socks5",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text)
				};
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}
