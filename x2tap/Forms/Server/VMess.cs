using System;
using System.Drawing;
using System.Windows.Forms;

namespace x2tap.Forms.Server
{
	public partial class VMess : Form
	{
		public int Index;

		public VMess(int index = -1)
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

		private void VMess_Load(object sender, EventArgs e)
		{
			ConfigurationGroupBox.Text = Utils.MultiLanguage.Translate("Configuration");
			RemarkLabel.Text = Utils.MultiLanguage.Translate("Remark");
			AddressLabel.Text = Utils.MultiLanguage.Translate("Address");
			UserIDLabel.Text = Utils.MultiLanguage.Translate("User ID");
			AlterIDLabel.Text = Utils.MultiLanguage.Translate("Alter ID");
			EncryptMethodLabel.Text = Utils.MultiLanguage.Translate("Encrypt Method");
			TransferProtocolLabel.Text = Utils.MultiLanguage.Translate("Transfer Protocol");
			FakeTypeLabel.Text = Utils.MultiLanguage.Translate("Fake Type");
			HostLabel.Text = Utils.MultiLanguage.Translate("Host");
			PathLabel.Text = Utils.MultiLanguage.Translate("Path");
			QUICSecurityLabel.Text = Utils.MultiLanguage.Translate("QUIC Security");
			QUICSecretLabel.Text = Utils.MultiLanguage.Translate("QUIC Secret");
			ControlButton.Text = Utils.MultiLanguage.Translate("Save");

			foreach (var encrypt in Global.EncryptMethods.VMess)
			{
				EncryptMethodComboBox.Items.Add(encrypt);
			}

			foreach (var protocol in Global.TransferProtocols)
			{
				TransferProtocolComboBox.Items.Add(protocol);
			}

			foreach (var fake in Global.FakeTypes)
			{
				FakeTypeComboBox.Items.Add(fake);
			}

			foreach (var security in Global.EncryptMethods.VMessQUIC)
			{
				QUICSecurityComboBox.Items.Add(security);
			}

			if (Index != -1)
			{
				RemarkTextBox.Text = Global.Servers[Index].Remark;
				AddressTextBox.Text = Global.Servers[Index].Address;
				PortTextBox.Text = Global.Servers[Index].Port.ToString();
				UserIDTextBox.Text = Global.Servers[Index].UserID;
				AlterIDTextBox.Text = Global.Servers[Index].AlterID.ToString();
				EncryptMethodComboBox.SelectedIndex = Global.EncryptMethods.VMess.IndexOf(Global.Servers[Index].EncryptMethod);
				TransferProtocolComboBox.SelectedIndex = Global.TransferProtocols.IndexOf(Global.Servers[Index].TransferProtocol);
				FakeTypeComboBox.SelectedIndex = Global.FakeTypes.IndexOf(Global.Servers[Index].FakeType);
				HostTextBox.Text = Global.Servers[Index].Host;
				PathTextBox.Text = Global.Servers[Index].Path;
				QUICSecurityComboBox.SelectedIndex = Global.EncryptMethods.VMessQUIC.IndexOf(Global.Servers[Index].QUICSecurity);
				QUICSecretTextBox.Text = Global.Servers[Index].QUICSecret;
				TLSSecureCheckBox.Checked = Global.Servers[Index].TLSSecure;
			}
			else
			{
				EncryptMethodComboBox.SelectedIndex = 0;
				TransferProtocolComboBox.SelectedIndex = 0;
				FakeTypeComboBox.SelectedIndex = 0;
				QUICSecurityComboBox.SelectedIndex = 0;
			}
		}

		private void VMess_FormClosing(object sender, FormClosingEventArgs e)
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
					Type = "VMess",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					UserID = UserIDTextBox.Text,
					AlterID = int.Parse(AlterIDTextBox.Text),
					EncryptMethod = EncryptMethodComboBox.Text,
					TransferProtocol = TransferProtocolComboBox.Text,
					FakeType = FakeTypeComboBox.Text,
					Host = HostTextBox.Text,
					Path = PathTextBox.Text,
					QUICSecurity = QUICSecurityComboBox.Text,
					QUICSecret = QUICSecretTextBox.Text,
					TLSSecure = TLSSecureCheckBox.Checked
				});
			}
			else
			{
				Global.Servers[Index] = new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = Global.Servers[Index].GroupID,
					Type = "VMess",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					UserID = UserIDTextBox.Text,
					AlterID = int.Parse(AlterIDTextBox.Text),
					EncryptMethod = EncryptMethodComboBox.Text,
					TransferProtocol = TransferProtocolComboBox.Text,
					FakeType = FakeTypeComboBox.Text,
					Host = HostTextBox.Text,
					Path = PathTextBox.Text,
					QUICSecurity = QUICSecurityComboBox.Text,
					QUICSecret = QUICSecretTextBox.Text,
					TLSSecure = TLSSecureCheckBox.Checked
				};
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}
