using System;
using System.Drawing;
using System.Windows.Forms;

namespace x2tap.Forms.Server
{
	public partial class ShadowsocksR : Form
	{
		public int Index;

		public ShadowsocksR(int index = -1)
		{
			InitializeComponent();

			Index = index;
		}

		private void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			var cbx = sender as ComboBox;
			if (cbx != null)
			{
				e.DrawBackground();

				if (e.Index >= 0)
				{
					var sf = new StringFormat();
					sf.LineAlignment = StringAlignment.Center;
					sf.Alignment = StringAlignment.Center;

					var brush = new SolidBrush(cbx.ForeColor);

					if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
					{
						brush = SystemBrushes.HighlightText as SolidBrush;
					}

					e.Graphics.DrawString(cbx.Items[e.Index].ToString(), cbx.Font, brush, e.Bounds, sf);
				}
			}
		}

		private void ShadowsocksR_Load(object sender, EventArgs e)
		{
			ConfigurationGroupBox.Text = Utils.MultiLanguage.Translate("Configuration");
			RemarkLabel.Text = Utils.MultiLanguage.Translate("Remark");
			AddressLabel.Text = Utils.MultiLanguage.Translate("Address");
			PasswordLabel.Text = Utils.MultiLanguage.Translate("Password");
			EncryptMethodLabel.Text = Utils.MultiLanguage.Translate("Encrypt Method");
			ProtocolLabel.Text = Utils.MultiLanguage.Translate("Protocol");
			ProtocolParamLabel.Text = Utils.MultiLanguage.Translate("Protocol Param");
			OBFSLabel.Text = Utils.MultiLanguage.Translate("OBFS");
			OBFSParamLabel.Text = Utils.MultiLanguage.Translate("OBFS Param");
			ControlButton.Text = Utils.MultiLanguage.Translate("Save");

			foreach (var encrypt in Global.EncryptMethods.SSR)
			{
				EncryptMethodComboBox.Items.Add(encrypt);
			}

			foreach (var protocol in Global.Protocols)
			{
				ProtocolComboBox.Items.Add(protocol);
			}

			foreach (var obfs in Global.OBFSs)
			{
				OBFSComboBox.Items.Add(obfs);
			}

			if (Index != -1)
			{
				RemarkTextBox.Text = Global.Servers[Index].Remark;
				AddressTextBox.Text = Global.Servers[Index].Address;
				PortTextBox.Text = Global.Servers[Index].Port.ToString();
				PasswordTextBox.Text = Global.Servers[Index].Password;
				EncryptMethodComboBox.SelectedIndex = Global.EncryptMethods.SSR.IndexOf(Global.Servers[Index].EncryptMethod);
				ProtocolComboBox.SelectedIndex = Global.Protocols.IndexOf(Global.Servers[Index].Protocol);
				ProtocolParamTextBox.Text = Global.Servers[Index].ProtocolParam;
				OBFSComboBox.SelectedIndex = Global.OBFSs.IndexOf(Global.Servers[Index].OBFS);
				OBFSParamTextBox.Text = Global.Servers[Index].OBFSParam;
			}
			else
			{
				EncryptMethodComboBox.SelectedIndex = 0;
				ProtocolComboBox.SelectedIndex = 0;
				OBFSComboBox.SelectedIndex = 0;
			}
		}

		private void ShadowsocksR_FormClosing(object sender, FormClosingEventArgs e)
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
					Type = "ShadowsocksR",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					EncryptMethod = EncryptMethodComboBox.Text,
					Protocol = ProtocolComboBox.Text,
					ProtocolParam = ProtocolParamTextBox.Text,
					OBFS = OBFSComboBox.Text,
					OBFSParam = OBFSParamTextBox.Text
				});
			}
			else
			{
				Global.Servers[Index] = new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = Global.Servers[Index].GroupID,
					Type = "ShadowsocksR",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					EncryptMethod = EncryptMethodComboBox.Text,
					Protocol = ProtocolComboBox.Text,
					ProtocolParam = ProtocolParamTextBox.Text,
					OBFS = OBFSComboBox.Text,
					OBFSParam = OBFSParamTextBox.Text
				};
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}
