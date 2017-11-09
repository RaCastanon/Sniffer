namespace BlueBoxTool
{
    partial class MainView
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
            this.grpBoxSerialConnection = new System.Windows.Forms.GroupBox();
            this.btnSnifferSerialPort = new System.Windows.Forms.Button();
            this.cmbBoxSnifferSerialPorts = new System.Windows.Forms.ComboBox();
            this.lblSnifferPort = new System.Windows.Forms.Label();
            this.btnBlueBoxSerialPort = new System.Windows.Forms.Button();
            this.cmbBoxBlueBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.lblBlueBoxSerialPort = new System.Windows.Forms.Label();
            this.grpDiagnostics = new System.Windows.Forms.GroupBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.pidsTab = new System.Windows.Forms.TabPage();
            this.panelExtendedPIDS = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpBoxSerialConnection.SuspendLayout();
            this.grpDiagnostics.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.pidsTab.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpBoxSerialConnection
            // 
            this.grpBoxSerialConnection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBoxSerialConnection.Controls.Add(this.btnSnifferSerialPort);
            this.grpBoxSerialConnection.Controls.Add(this.cmbBoxSnifferSerialPorts);
            this.grpBoxSerialConnection.Controls.Add(this.lblSnifferPort);
            this.grpBoxSerialConnection.Controls.Add(this.btnBlueBoxSerialPort);
            this.grpBoxSerialConnection.Controls.Add(this.cmbBoxBlueBoxSerialPorts);
            this.grpBoxSerialConnection.Controls.Add(this.lblBlueBoxSerialPort);
            this.grpBoxSerialConnection.Location = new System.Drawing.Point(13, 27);
            this.grpBoxSerialConnection.Name = "grpBoxSerialConnection";
            this.grpBoxSerialConnection.Size = new System.Drawing.Size(515, 77);
            this.grpBoxSerialConnection.TabIndex = 0;
            this.grpBoxSerialConnection.TabStop = false;
            this.grpBoxSerialConnection.Text = "Serial Connection";
            // 
            // btnSnifferSerialPort
            // 
            this.btnSnifferSerialPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSnifferSerialPort.Location = new System.Drawing.Point(430, 46);
            this.btnSnifferSerialPort.Name = "btnSnifferSerialPort";
            this.btnSnifferSerialPort.Size = new System.Drawing.Size(75, 23);
            this.btnSnifferSerialPort.TabIndex = 5;
            this.btnSnifferSerialPort.Text = "Connect";
            this.btnSnifferSerialPort.UseVisualStyleBackColor = true;
            this.btnSnifferSerialPort.Click += new System.EventHandler(this.btnSnifferSerialPort_Click);
            // 
            // cmbBoxSnifferSerialPorts
            // 
            this.cmbBoxSnifferSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxSnifferSerialPorts.FormattingEnabled = true;
            this.cmbBoxSnifferSerialPorts.Location = new System.Drawing.Point(119, 48);
            this.cmbBoxSnifferSerialPorts.Name = "cmbBoxSnifferSerialPorts";
            this.cmbBoxSnifferSerialPorts.Size = new System.Drawing.Size(69, 21);
            this.cmbBoxSnifferSerialPorts.TabIndex = 3;
            // 
            // lblSnifferPort
            // 
            this.lblSnifferPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSnifferPort.AutoSize = true;
            this.lblSnifferPort.Location = new System.Drawing.Point(6, 51);
            this.lblSnifferPort.Name = "lblSnifferPort";
            this.lblSnifferPort.Size = new System.Drawing.Size(98, 13);
            this.lblSnifferPort.TabIndex = 4;
            this.lblSnifferPort.Text = "\"Sniffer\" Serial Port";
            // 
            // btnBlueBoxSerialPort
            // 
            this.btnBlueBoxSerialPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBlueBoxSerialPort.Location = new System.Drawing.Point(430, 19);
            this.btnBlueBoxSerialPort.Name = "btnBlueBoxSerialPort";
            this.btnBlueBoxSerialPort.Size = new System.Drawing.Size(75, 23);
            this.btnBlueBoxSerialPort.TabIndex = 2;
            this.btnBlueBoxSerialPort.Text = "Connect";
            this.btnBlueBoxSerialPort.UseVisualStyleBackColor = true;
            this.btnBlueBoxSerialPort.Click += new System.EventHandler(this.btnBlueBoxSerialPort_Click);
            // 
            // cmbBoxBlueBoxSerialPorts
            // 
            this.cmbBoxBlueBoxSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBoxBlueBoxSerialPorts.FormattingEnabled = true;
            this.cmbBoxBlueBoxSerialPorts.Location = new System.Drawing.Point(119, 21);
            this.cmbBoxBlueBoxSerialPorts.Name = "cmbBoxBlueBoxSerialPorts";
            this.cmbBoxBlueBoxSerialPorts.Size = new System.Drawing.Size(69, 21);
            this.cmbBoxBlueBoxSerialPorts.TabIndex = 0;
            // 
            // lblBlueBoxSerialPort
            // 
            this.lblBlueBoxSerialPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblBlueBoxSerialPort.AutoSize = true;
            this.lblBlueBoxSerialPort.Location = new System.Drawing.Point(6, 24);
            this.lblBlueBoxSerialPort.Name = "lblBlueBoxSerialPort";
            this.lblBlueBoxSerialPort.Size = new System.Drawing.Size(107, 13);
            this.lblBlueBoxSerialPort.TabIndex = 0;
            this.lblBlueBoxSerialPort.Text = "\"BlueBox\" Serial Port";
            // 
            // grpDiagnostics
            // 
            this.grpDiagnostics.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDiagnostics.Controls.Add(this.tabControl);
            this.grpDiagnostics.Location = new System.Drawing.Point(13, 110);
            this.grpDiagnostics.Name = "grpDiagnostics";
            this.grpDiagnostics.Size = new System.Drawing.Size(515, 293);
            this.grpDiagnostics.TabIndex = 1;
            this.grpDiagnostics.TabStop = false;
            this.grpDiagnostics.Text = "Diagnostics";
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.pidsTab);
            this.tabControl.Location = new System.Drawing.Point(3, 16);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(506, 271);
            this.tabControl.TabIndex = 0;
            // 
            // pidsTab
            // 
            this.pidsTab.Controls.Add(this.panelExtendedPIDS);
            this.pidsTab.Location = new System.Drawing.Point(4, 22);
            this.pidsTab.Name = "pidsTab";
            this.pidsTab.Padding = new System.Windows.Forms.Padding(3);
            this.pidsTab.Size = new System.Drawing.Size(498, 245);
            this.pidsTab.TabIndex = 0;
            this.pidsTab.Text = "PIDS";
            this.pidsTab.UseVisualStyleBackColor = true;
            // 
            // panelExtendedPIDS
            // 
            this.panelExtendedPIDS.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelExtendedPIDS.Location = new System.Drawing.Point(0, 0);
            this.panelExtendedPIDS.Name = "panelExtendedPIDS";
            this.panelExtendedPIDS.Size = new System.Drawing.Size(497, 244);
            this.panelExtendedPIDS.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(152, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 415);
            this.Controls.Add(this.grpDiagnostics);
            this.Controls.Add(this.grpBoxSerialConnection);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainView";
            this.Text = "AVCLAN BlueBox Interface";
            this.grpBoxSerialConnection.ResumeLayout(false);
            this.grpBoxSerialConnection.PerformLayout();
            this.grpDiagnostics.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.pidsTab.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpBoxSerialConnection;
        private System.Windows.Forms.GroupBox grpDiagnostics;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ComboBox cmbBoxBlueBoxSerialPorts;
        private System.Windows.Forms.Label lblBlueBoxSerialPort;
        private System.Windows.Forms.Button btnBlueBoxSerialPort;
        private System.Windows.Forms.Button btnSnifferSerialPort;
        private System.Windows.Forms.ComboBox cmbBoxSnifferSerialPorts;
        private System.Windows.Forms.Label lblSnifferPort;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage pidsTab;
        private System.Windows.Forms.Panel panelExtendedPIDS;
    }
}

