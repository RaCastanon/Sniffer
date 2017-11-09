namespace DiagnosticTool.GUIViews
{
    partial class ExtendedPIDSView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtBoxMessage = new System.Windows.Forms.TextBox();
            this.btnSendMessage = new System.Windows.Forms.Button();
            this.richTextBoxMessages = new System.Windows.Forms.RichTextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtBoxAddress = new System.Windows.Forms.TextBox();
            this.btnSetAddress = new System.Windows.Forms.Button();
            this.lblAVCLANAddr = new System.Windows.Forms.Label();
            this.lblAVCLANCurrentAddr = new System.Windows.Forms.Label();
            this.cmbBoxTimeUnit = new System.Windows.Forms.ComboBox();
            this.chkBoxPeriodically = new System.Windows.Forms.CheckBox();
            this.numUpDwnTime = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnTime)).BeginInit();
            this.SuspendLayout();
            // 
            // txtBoxMessage
            // 
            this.txtBoxMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxMessage.Location = new System.Drawing.Point(3, 6);
            this.txtBoxMessage.Name = "txtBoxMessage";
            this.txtBoxMessage.Size = new System.Drawing.Size(363, 20);
            this.txtBoxMessage.TabIndex = 0;
            // 
            // btnSendMessage
            // 
            this.btnSendMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendMessage.Location = new System.Drawing.Point(372, 4);
            this.btnSendMessage.Name = "btnSendMessage";
            this.btnSendMessage.Size = new System.Drawing.Size(75, 23);
            this.btnSendMessage.TabIndex = 1;
            this.btnSendMessage.Text = "Send";
            this.btnSendMessage.UseVisualStyleBackColor = true;
            this.btnSendMessage.Click += new System.EventHandler(this.btnSendMessage_Click);
            // 
            // richTextBoxMessages
            // 
            this.richTextBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxMessages.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.richTextBoxMessages.Location = new System.Drawing.Point(3, 86);
            this.richTextBoxMessages.Name = "richTextBoxMessages";
            this.richTextBoxMessages.ReadOnly = true;
            this.richTextBoxMessages.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.richTextBoxMessages.Size = new System.Drawing.Size(444, 280);
            this.richTextBoxMessages.TabIndex = 2;
            this.richTextBoxMessages.Text = "";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(372, 372);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtBoxAddress
            // 
            this.txtBoxAddress.Location = new System.Drawing.Point(3, 32);
            this.txtBoxAddress.Name = "txtBoxAddress";
            this.txtBoxAddress.Size = new System.Drawing.Size(51, 20);
            this.txtBoxAddress.TabIndex = 4;
            // 
            // btnSetAddress
            // 
            this.btnSetAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetAddress.Location = new System.Drawing.Point(258, 30);
            this.btnSetAddress.Name = "btnSetAddress";
            this.btnSetAddress.Size = new System.Drawing.Size(189, 23);
            this.btnSetAddress.TabIndex = 5;
            this.btnSetAddress.Text = "Set AVCLAN device address";
            this.btnSetAddress.UseVisualStyleBackColor = true;
            this.btnSetAddress.Click += new System.EventHandler(this.btnSetAddress_Click);
            // 
            // lblAVCLANAddr
            // 
            this.lblAVCLANAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAVCLANAddr.AutoSize = true;
            this.lblAVCLANAddr.Location = new System.Drawing.Point(72, 35);
            this.lblAVCLANAddr.Name = "lblAVCLANAddr";
            this.lblAVCLANAddr.Size = new System.Drawing.Size(126, 13);
            this.lblAVCLANAddr.TabIndex = 6;
            this.lblAVCLANAddr.Text = "Current AVCLAN address";
            // 
            // lblAVCLANCurrentAddr
            // 
            this.lblAVCLANCurrentAddr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAVCLANCurrentAddr.AutoSize = true;
            this.lblAVCLANCurrentAddr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.lblAVCLANCurrentAddr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblAVCLANCurrentAddr.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAVCLANCurrentAddr.Location = new System.Drawing.Point(204, 35);
            this.lblAVCLANCurrentAddr.Name = "lblAVCLANCurrentAddr";
            this.lblAVCLANCurrentAddr.Size = new System.Drawing.Size(45, 15);
            this.lblAVCLANCurrentAddr.TabIndex = 7;
            this.lblAVCLANCurrentAddr.Text = "XX XX";
            // 
            // cmbBoxTimeUnit
            // 
            this.cmbBoxTimeUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxTimeUnit.FormattingEnabled = true;
            this.cmbBoxTimeUnit.Location = new System.Drawing.Point(178, 55);
            this.cmbBoxTimeUnit.Name = "cmbBoxTimeUnit";
            this.cmbBoxTimeUnit.Size = new System.Drawing.Size(48, 21);
            this.cmbBoxTimeUnit.TabIndex = 12;
            // 
            // chkBoxPeriodically
            // 
            this.chkBoxPeriodically.AutoSize = true;
            this.chkBoxPeriodically.Location = new System.Drawing.Point(3, 58);
            this.chkBoxPeriodically.Name = "chkBoxPeriodically";
            this.chkBoxPeriodically.Size = new System.Drawing.Size(107, 17);
            this.chkBoxPeriodically.TabIndex = 11;
            this.chkBoxPeriodically.Text = "Send Periodically";
            this.chkBoxPeriodically.UseVisualStyleBackColor = true;
            this.chkBoxPeriodically.CheckedChanged += new System.EventHandler(this.chkBoxPeriodically_CheckedChanged);
            // 
            // numUpDwnTime
            // 
            this.numUpDwnTime.Location = new System.Drawing.Point(115, 55);
            this.numUpDwnTime.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numUpDwnTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numUpDwnTime.Name = "numUpDwnTime";
            this.numUpDwnTime.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.numUpDwnTime.Size = new System.Drawing.Size(57, 20);
            this.numUpDwnTime.TabIndex = 10;
            this.numUpDwnTime.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            // 
            // ExtendedPIDSView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.cmbBoxTimeUnit);
            this.Controls.Add(this.chkBoxPeriodically);
            this.Controls.Add(this.numUpDwnTime);
            this.Controls.Add(this.lblAVCLANCurrentAddr);
            this.Controls.Add(this.lblAVCLANAddr);
            this.Controls.Add(this.btnSetAddress);
            this.Controls.Add(this.txtBoxAddress);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.richTextBoxMessages);
            this.Controls.Add(this.btnSendMessage);
            this.Controls.Add(this.txtBoxMessage);
            this.Name = "ExtendedPIDSView";
            this.Size = new System.Drawing.Size(450, 398);
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnTime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBoxMessage;
        private System.Windows.Forms.Button btnSendMessage;
        private System.Windows.Forms.RichTextBox richTextBoxMessages;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtBoxAddress;
        private System.Windows.Forms.Button btnSetAddress;
        private System.Windows.Forms.Label lblAVCLANAddr;
        private System.Windows.Forms.Label lblAVCLANCurrentAddr;
        private System.Windows.Forms.ComboBox cmbBoxTimeUnit;
        private System.Windows.Forms.CheckBox chkBoxPeriodically;
        private System.Windows.Forms.NumericUpDown numUpDwnTime;
    }
}
