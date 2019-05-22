namespace x2tap.Forms
{
	partial class SettingForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingForm));
			this.TUNTAPGroupBox = new System.Windows.Forms.GroupBox();
			this.TUNTAPAddressTextBox = new System.Windows.Forms.TextBox();
			this.TUNTAPAddressLabel = new System.Windows.Forms.Label();
			this.TUNTAPNetmaskTextBox = new System.Windows.Forms.TextBox();
			this.TUNTAPNetmaskLabel = new System.Windows.Forms.Label();
			this.TUNTAPGatewayTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.TUNTAPDNS1TextBox = new System.Windows.Forms.TextBox();
			this.TUNTAPDNS1Label = new System.Windows.Forms.Label();
			this.TUNTAPDNS2TextBox = new System.Windows.Forms.TextBox();
			this.TUNTAPDNS2Label = new System.Windows.Forms.Label();
			this.ControlButton = new System.Windows.Forms.Button();
			this.GlobalBypassIPsButton = new System.Windows.Forms.Button();
			this.TUNTAPGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// TUNTAPGroupBox
			// 
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPDNS2Label);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPDNS2TextBox);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPDNS1Label);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPDNS1TextBox);
			this.TUNTAPGroupBox.Controls.Add(this.label1);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPGatewayTextBox);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPNetmaskLabel);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPNetmaskTextBox);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPAddressLabel);
			this.TUNTAPGroupBox.Controls.Add(this.TUNTAPAddressTextBox);
			this.TUNTAPGroupBox.Location = new System.Drawing.Point(12, 12);
			this.TUNTAPGroupBox.Name = "TUNTAPGroupBox";
			this.TUNTAPGroupBox.Size = new System.Drawing.Size(420, 169);
			this.TUNTAPGroupBox.TabIndex = 0;
			this.TUNTAPGroupBox.TabStop = false;
			this.TUNTAPGroupBox.Text = "TUN/TAP";
			// 
			// TUNTAPAddressTextBox
			// 
			this.TUNTAPAddressTextBox.Location = new System.Drawing.Point(120, 22);
			this.TUNTAPAddressTextBox.Name = "TUNTAPAddressTextBox";
			this.TUNTAPAddressTextBox.Size = new System.Drawing.Size(294, 23);
			this.TUNTAPAddressTextBox.TabIndex = 0;
			this.TUNTAPAddressTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TUNTAPAddressLabel
			// 
			this.TUNTAPAddressLabel.AutoSize = true;
			this.TUNTAPAddressLabel.Location = new System.Drawing.Point(9, 25);
			this.TUNTAPAddressLabel.Name = "TUNTAPAddressLabel";
			this.TUNTAPAddressLabel.Size = new System.Drawing.Size(56, 17);
			this.TUNTAPAddressLabel.TabIndex = 1;
			this.TUNTAPAddressLabel.Text = "Address";
			// 
			// TUNTAPNetmaskTextBox
			// 
			this.TUNTAPNetmaskTextBox.Location = new System.Drawing.Point(120, 51);
			this.TUNTAPNetmaskTextBox.Name = "TUNTAPNetmaskTextBox";
			this.TUNTAPNetmaskTextBox.Size = new System.Drawing.Size(294, 23);
			this.TUNTAPNetmaskTextBox.TabIndex = 2;
			this.TUNTAPNetmaskTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TUNTAPNetmaskLabel
			// 
			this.TUNTAPNetmaskLabel.AutoSize = true;
			this.TUNTAPNetmaskLabel.Location = new System.Drawing.Point(9, 54);
			this.TUNTAPNetmaskLabel.Name = "TUNTAPNetmaskLabel";
			this.TUNTAPNetmaskLabel.Size = new System.Drawing.Size(60, 17);
			this.TUNTAPNetmaskLabel.TabIndex = 3;
			this.TUNTAPNetmaskLabel.Text = "Netmask";
			// 
			// TUNTAPGatewayTextBox
			// 
			this.TUNTAPGatewayTextBox.Location = new System.Drawing.Point(120, 80);
			this.TUNTAPGatewayTextBox.Name = "TUNTAPGatewayTextBox";
			this.TUNTAPGatewayTextBox.Size = new System.Drawing.Size(294, 23);
			this.TUNTAPGatewayTextBox.TabIndex = 4;
			this.TUNTAPGatewayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 83);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(57, 17);
			this.label1.TabIndex = 5;
			this.label1.Text = "Gateway";
			// 
			// TUNTAPDNS1TextBox
			// 
			this.TUNTAPDNS1TextBox.Location = new System.Drawing.Point(120, 109);
			this.TUNTAPDNS1TextBox.Name = "TUNTAPDNS1TextBox";
			this.TUNTAPDNS1TextBox.Size = new System.Drawing.Size(294, 23);
			this.TUNTAPDNS1TextBox.TabIndex = 6;
			this.TUNTAPDNS1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TUNTAPDNS1Label
			// 
			this.TUNTAPDNS1Label.AutoSize = true;
			this.TUNTAPDNS1Label.Location = new System.Drawing.Point(9, 112);
			this.TUNTAPDNS1Label.Name = "TUNTAPDNS1Label";
			this.TUNTAPDNS1Label.Size = new System.Drawing.Size(45, 17);
			this.TUNTAPDNS1Label.TabIndex = 7;
			this.TUNTAPDNS1Label.Text = "DNS 1";
			// 
			// TUNTAPDNS2TextBox
			// 
			this.TUNTAPDNS2TextBox.Location = new System.Drawing.Point(120, 138);
			this.TUNTAPDNS2TextBox.Name = "TUNTAPDNS2TextBox";
			this.TUNTAPDNS2TextBox.Size = new System.Drawing.Size(294, 23);
			this.TUNTAPDNS2TextBox.TabIndex = 8;
			this.TUNTAPDNS2TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// TUNTAPDNS2Label
			// 
			this.TUNTAPDNS2Label.AutoSize = true;
			this.TUNTAPDNS2Label.Location = new System.Drawing.Point(9, 141);
			this.TUNTAPDNS2Label.Name = "TUNTAPDNS2Label";
			this.TUNTAPDNS2Label.Size = new System.Drawing.Size(45, 17);
			this.TUNTAPDNS2Label.TabIndex = 9;
			this.TUNTAPDNS2Label.Text = "DNS 2";
			// 
			// ControlButton
			// 
			this.ControlButton.Location = new System.Drawing.Point(357, 187);
			this.ControlButton.Name = "ControlButton";
			this.ControlButton.Size = new System.Drawing.Size(75, 23);
			this.ControlButton.TabIndex = 1;
			this.ControlButton.Text = "Save";
			this.ControlButton.UseVisualStyleBackColor = true;
			// 
			// GlobalBypassIPsButton
			// 
			this.GlobalBypassIPsButton.Location = new System.Drawing.Point(12, 187);
			this.GlobalBypassIPsButton.Name = "GlobalBypassIPsButton";
			this.GlobalBypassIPsButton.Size = new System.Drawing.Size(128, 23);
			this.GlobalBypassIPsButton.TabIndex = 2;
			this.GlobalBypassIPsButton.Text = "Global Bypass IPs";
			this.GlobalBypassIPsButton.UseVisualStyleBackColor = true;
			// 
			// SettingForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(444, 222);
			this.Controls.Add(this.GlobalBypassIPsButton);
			this.Controls.Add(this.ControlButton);
			this.Controls.Add(this.TUNTAPGroupBox);
			this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.Name = "SettingForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Settings";
			this.TUNTAPGroupBox.ResumeLayout(false);
			this.TUNTAPGroupBox.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox TUNTAPGroupBox;
		private System.Windows.Forms.TextBox TUNTAPAddressTextBox;
		private System.Windows.Forms.Label TUNTAPAddressLabel;
		private System.Windows.Forms.TextBox TUNTAPNetmaskTextBox;
		private System.Windows.Forms.Label TUNTAPNetmaskLabel;
		private System.Windows.Forms.TextBox TUNTAPGatewayTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label TUNTAPDNS1Label;
		private System.Windows.Forms.TextBox TUNTAPDNS1TextBox;
		private System.Windows.Forms.TextBox TUNTAPDNS2TextBox;
		private System.Windows.Forms.Label TUNTAPDNS2Label;
		private System.Windows.Forms.Button ControlButton;
		private System.Windows.Forms.Button GlobalBypassIPsButton;
	}
}