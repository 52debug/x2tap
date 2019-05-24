using System;
using System.Drawing;
using System.Windows.Forms;

namespace x2tap.Forms.Server
{
	public partial class Shadowsocks : Form
	{
		public int Index;

		public Shadowsocks(int index = -1)
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

		private void Shadowsocks_Load(object sender, EventArgs e)
		{
			ConfigurationGroupBox.Text = Utils.MultiLanguage.Translate("Configuration");
			RemarkLabel.Text = Utils.MultiLanguage.Translate("Remark");
			AddressLabel.Text = Utils.MultiLanguage.Translate("Address");
			PasswordLabel.Text = Utils.MultiLanguage.Translate("Password");
			EncryptMethodLabel.Text = Utils.MultiLanguage.Translate("Encrypt Method");
			ControlButton.Text = Utils.MultiLanguage.Translate("Save");

			foreach (var encrypt in Global.EncryptMethods.SS)
			{
				EncryptMethodComboBox.Items.Add(encrypt);
			}

			if (Index != -1)
			{
				RemarkTextBox.Text = Global.Servers[Index].Remark;
				AddressTextBox.Text = Global.Servers[Index].Address;
				PortTextBox.Text = Global.Servers[Index].Port.ToString();
				PasswordTextBox.Text = Global.Servers[Index].Password;
				EncryptMethodComboBox.SelectedIndex = Global.EncryptMethods.SS.IndexOf(Global.Servers[Index].EncryptMethod);
			}
			else
			{
				EncryptMethodComboBox.SelectedIndex = 0;
			}
		}

		private void Shadowsocks_FormClosing(object sender, FormClosingEventArgs e)
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
					Type = "Shadowsocks",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					EncryptMethod = EncryptMethodComboBox.Text
				});
			}
			else
			{
				Global.Servers[Index] = new Objects.Server()
				{
					Remark = RemarkTextBox.Text,
					GroupID = Global.Servers[Index].GroupID,
					Type = "Shadowsocks",
					Address = AddressTextBox.Text,
					Port = int.Parse(PortTextBox.Text),
					Password = PasswordTextBox.Text,
					EncryptMethod = EncryptMethodComboBox.Text
				};
			}

			MessageBox.Show(Utils.MultiLanguage.Translate("Saved"), Utils.MultiLanguage.Translate("Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
			Close();
		}
	}
}
