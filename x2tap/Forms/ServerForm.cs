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
	public partial class ServerForm : Form
	{
		public int Index = -1;

		public ServerForm(object data = null)
		{
			InitializeComponent();

			if (data is String)
			{
				TypeComboBox.SelectedIndex = TypeComboBox.Items.IndexOf(data as String);
			}
			else if (data is Objects.Server)
			{
				var server = data as Objects.Server;
				Index = Global.Servers.IndexOf(server);

				RemarkTextBox.Text = server.Remark;
				GroupIDTextBox.Text = server.GroupID;
				TypeComboBox.SelectedIndex = TypeComboBox.Items.IndexOf(server.Type);
				AddressTextBox.Text = server.Address;
				PortTextBox.Text = server.Port.ToString();
				PasswordTextBox.Text = server.Password;
				UserIDTextBox.Text = server.UserID;
				AlterIDTextBox.Text = server.AlterID.ToString();
				EncryptMethodComboBox.SelectedIndex = EncryptMethodComboBox.Items.IndexOf(server.EncryptMethod);
				ProtocolComboBox.SelectedIndex = ProtocolComboBox.Items.IndexOf(server.Protocol);
				ProtocolParamTextBox.Text = server.ProtocolParam;
				OBFSComboBox.SelectedIndex = OBFSComboBox.Items.IndexOf(server.OBFS);
				OBFSParamTextBox.Text = server.OBFSParam;
				TransferProtocolComboBox.SelectedIndex = TransferProtocolComboBox.Items.IndexOf(server.TransferProtocol);
				FakeTypeComboBox.SelectedIndex = FakeTypeComboBox.Items.IndexOf(server.FakeType);
				HostTextBox.Text = server.Host;
				PathTextBox.Text = server.Path;
				QUICSecretTextBox.Text = server.QUICSecret;
				TLSSecureCheckBox.Checked = server.TLSSecure;
			}
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

		private void ServerForm_Load(object sender, EventArgs e)
		{
			
		}

		private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Global.MainForm.InitServers();
			Global.MainForm.Show();
		}

		private void TypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TypeComboBox.SelectedIndex == 0) // Socks5
			{
				PasswordTextBox.Enabled = false;
				UserIDTextBox.Enabled = false;
				AlterIDTextBox.Enabled = false;
				EncryptMethodComboBox.Enabled = false;
				ProtocolComboBox.Enabled = false;
				ProtocolParamTextBox.Enabled = false;
				OBFSComboBox.Enabled = false;
				OBFSParamTextBox.Enabled = false;
				TransferProtocolComboBox.Enabled = false;
				FakeTypeComboBox.Enabled = false;
				HostTextBox.Enabled = false;
				PathTextBox.Enabled = false;
				QUICSecretTextBox.Enabled = false;
				TLSSecureCheckBox.Enabled = false;
			}
			else if (TypeComboBox.SelectedIndex == 1) // Shadowsocks
			{
				PasswordTextBox.Enabled = true;
				UserIDTextBox.Enabled = false;
				AlterIDTextBox.Enabled = false;
				EncryptMethodComboBox.Enabled = true;
				ProtocolComboBox.Enabled = false;
				ProtocolParamTextBox.Enabled = false;
				OBFSComboBox.Enabled = false;
				OBFSParamTextBox.Enabled = false;
				TransferProtocolComboBox.Enabled = false;
				FakeTypeComboBox.Enabled = false;
				HostTextBox.Enabled = false;
				PathTextBox.Enabled = false;
				QUICSecretTextBox.Enabled = false;
				TLSSecureCheckBox.Enabled = false;

				EncryptMethodComboBox.Items.Clear();
				foreach (var encrypt in Global.EncryptMethods.SS)
				{
					EncryptMethodComboBox.Items.Add(encrypt);
				}
			}
			else if (TypeComboBox.SelectedIndex == 2) // ShadowsocksR
			{
				PasswordTextBox.Enabled = true;
				UserIDTextBox.Enabled = false;
				AlterIDTextBox.Enabled = false;
				EncryptMethodComboBox.Enabled = true;
				ProtocolComboBox.Enabled = true;
				ProtocolParamTextBox.Enabled = true;
				OBFSComboBox.Enabled = true;
				OBFSParamTextBox.Enabled = true;
				TransferProtocolComboBox.Enabled = false;
				FakeTypeComboBox.Enabled = false;
				HostTextBox.Enabled = false;
				PathTextBox.Enabled = false;
				QUICSecretTextBox.Enabled = false;
				TLSSecureCheckBox.Enabled = false;

				EncryptMethodComboBox.Items.Clear();
				foreach (var encrypt in Global.EncryptMethods.SSR)
				{
					EncryptMethodComboBox.Items.Add(encrypt);
				}

				ProtocolComboBox.Items.Clear();
				foreach (var protocol in Global.Protocols)
				{
					ProtocolComboBox.Items.Add(protocol);
				}

				OBFSComboBox.Items.Clear();
				foreach (var obfs in Global.OBFSs)
				{
					OBFSComboBox.Items.Add(obfs);
				}
			}
			else if (TypeComboBox.SelectedIndex == 3) // V2Ray
			{
				PasswordTextBox.Enabled = false;
				UserIDTextBox.Enabled = true;
				AlterIDTextBox.Enabled = true;
				EncryptMethodComboBox.Enabled = false;
				ProtocolComboBox.Enabled = false;
				ProtocolParamTextBox.Enabled = false;
				OBFSComboBox.Enabled = false;
				OBFSParamTextBox.Enabled = false;
				TransferProtocolComboBox.Enabled = true;
				FakeTypeComboBox.Enabled = true;
				HostTextBox.Enabled = true;
				PathTextBox.Enabled = true;
				QUICSecretTextBox.Enabled = false;
				TLSSecureCheckBox.Enabled = true;

				TransferProtocolComboBox.Items.Clear();
				foreach (var transferProtocol in Global.TransferProtocols)
				{
					TransferProtocolComboBox.Items.Add(transferProtocol);
				}
				TransferProtocolComboBox.SelectedIndex = 0;

				FakeTypeComboBox.Items.Clear();
				foreach (var fakeType in Global.FakeTypes)
				{
					FakeTypeComboBox.Items.Add(fakeType);
				}
				FakeTypeComboBox.SelectedIndex = 0;
			}
		}

		private void TransferProtocolComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (TransferProtocolComboBox.SelectedIndex >= 0)
			{
				if (TransferProtocolComboBox.Items[TransferProtocolComboBox.SelectedIndex].ToString() == "quic")
				{
					EncryptMethodComboBox.Enabled = true;
					EncryptMethodComboBox.Items.Clear();
					foreach (var encrypt in Global.EncryptMethods.VMessQUIC)
					{
						EncryptMethodComboBox.Items.Add(encrypt);
					}
					EncryptMethodComboBox.SelectedIndex = 0;
					QUICSecretTextBox.Enabled = true;
				}
				else
				{
					EncryptMethodComboBox.Enabled = false;
					EncryptMethodComboBox.Items.Clear();
					QUICSecretTextBox.Enabled = false;
				}
			}
		}

		private void ShareLinkTextBox_TextChanged(object sender, EventArgs e)
		{
			using (var qrGenerator = new QRCoder.QRCodeGenerator())
			{
				var qrCodeData = qrGenerator.CreateQrCode(ShareLinkTextBox.Text, QRCoder.QRCodeGenerator.ECCLevel.L);
				var qrCode = new QRCoder.QRCode(qrCodeData);

				QRCodePictureBox.Image = qrCode.GetGraphic(5);
			}
		}

		private void ControlButton_Click(object sender, EventArgs e)
		{
			if (Index == -1)
			{
				Global.Servers.Add(new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = GroupIDTextBox.Text,
					Type = TypeComboBox.Text,
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					UserID = UserIDTextBox.Text,
					AlterID = int.Parse(AlterIDTextBox.Text),
					EncryptMethod = EncryptMethodComboBox.Text,
					Protocol = ProtocolComboBox.Text,
					ProtocolParam = ProtocolParamTextBox.Text,
					OBFS = OBFSComboBox.Text,
					OBFSParam = OBFSParamTextBox.Text,
					TransferProtocol = TransferProtocolComboBox.Text,
					FakeType = FakeTypeComboBox.Text,
					Host = HostTextBox.Text,
					Path = PathTextBox.Text,
					QUICSecret = QUICSecretTextBox.Text,
					TLSSecure = TLSSecureCheckBox.Checked
				});
			}
			else
			{
				Global.Servers[Index] = new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = GroupIDTextBox.Text,
					Type = TypeComboBox.Text,
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					UserID = UserIDTextBox.Text,
					AlterID = int.Parse(AlterIDTextBox.Text),
					EncryptMethod = EncryptMethodComboBox.Text,
					Protocol = ProtocolComboBox.Text,
					ProtocolParam = ProtocolParamTextBox.Text,
					OBFS = OBFSComboBox.Text,
					OBFSParam = OBFSParamTextBox.Text,
					TransferProtocol = TransferProtocolComboBox.Text,
					FakeType = FakeTypeComboBox.Text,
					Host = HostTextBox.Text,
					Path = PathTextBox.Text,
					QUICSecret = QUICSecretTextBox.Text,
					TLSSecure = TLSSecureCheckBox.Checked
				};
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}
