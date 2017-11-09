using System;
using System.Windows.Forms;
using DiagnosticTool.GUIViews;
using CommunicationsHandler;
using SerialPortInterface;

namespace BlueBoxTool
{
    public partial class MainView : Form
    {
        private static string CONNECT_TEXT = "Connect";
        private static string DISCONNECT_TEXT = "Disconnect";
        private ExtendedPIDSView ExtendedPIDSView;
        private AVCLANHandler BlueBoxCmcHandler;
        private SnifferHandler SnifferCmcHandler;

        /// <summary>
        /// 
        /// </summary>
        public MainView()
        {
            InitializeComponent();

            ExtendedPIDSView = new ExtendedPIDSView();
            panelExtendedPIDS.Controls.Add(ExtendedPIDSView);
            ExtendedPIDSView.BorderStyle = BorderStyle.None;
            ExtendedPIDSView.Dock = DockStyle.Fill;
            ExtendedPIDSView.Show();
            
            tabControl.Selected += new TabControlEventHandler(tabControlDiagnosticsSelected);
            this.FormClosing += new FormClosingEventHandler(formClosingEvent);

            SnifferCmcHandler = new SnifferHandler();
            SnifferCmcHandler.startSnifferHandler();

            BlueBoxCmcHandler = new AVCLANHandler();
            BlueBoxCmcHandler.startAVCLANHandler();
            BlueBoxCmcHandler.addAVCLANListener(tabControl.TabPages[0].Name.ToString(), ExtendedPIDSView);
            BlueBoxCmcHandler.addAVCLANListener("SnifferListener", SnifferCmcHandler);

            /* Force tab selection event */
            tabControlDiagnosticsSelected(null, null);
            loadAvailableComports();
            btnBlueBoxSerialPort.Text = CONNECT_TEXT;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formClosingEvent(object sender, FormClosingEventArgs e)
        {
            BlueBoxCmcHandler.stopAVCLANHandler();
            SnifferCmcHandler.stopSnifferHandler();
            Application.Exit();
        }

        /// <summary>
        /// Tab change event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlDiagnosticsSelected(object sender, TabControlEventArgs e)
        {
            String current_tab_name = tabControl.SelectedTab.Name.ToString();
            BlueBoxCmcHandler.setCurrentAVCLANListener(current_tab_name);
        }

        /// <summary>
        /// Connect to serial port event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBlueBoxSerialPort_Click(object sender, EventArgs e)
        {
            string button_action = btnBlueBoxSerialPort.Text;
            if (button_action.Equals(CONNECT_TEXT))
            {
                string com_selection = (string)cmbBoxBlueBoxSerialPorts.SelectedItem;

                if ((com_selection != null) && com_selection.Contains("COM"))
                {
                    if (BlueBoxCmcHandler.connectAVCLANPort(com_selection))
                    {
                        btnBlueBoxSerialPort.Text = DISCONNECT_TEXT;
                        cmbBoxBlueBoxSerialPorts.Enabled = false;
                    }
                }
            }
            else if (button_action.Equals(DISCONNECT_TEXT))
            {
                if (BlueBoxCmcHandler.closeAVCLANPort())
                {
                    btnBlueBoxSerialPort.Text = CONNECT_TEXT;
                    cmbBoxBlueBoxSerialPorts.Enabled = true;
                }
            }
        }

        private void btnSnifferSerialPort_Click(object sender, EventArgs e)
        {
            string button_action = btnSnifferSerialPort.Text;
            if (button_action.Equals(CONNECT_TEXT))
            {
                string com_selection = (string)cmbBoxSnifferSerialPorts.SelectedItem;

                if ((com_selection != null) && com_selection.Contains("COM"))
                {
                    if (SnifferCmcHandler.connectSnifferPort(com_selection))
                    {
                        btnSnifferSerialPort.Text = DISCONNECT_TEXT;
                        cmbBoxSnifferSerialPorts.Enabled = false;
                    }
                }
            }
            else if (button_action.Equals(DISCONNECT_TEXT))
            {
                if (SnifferCmcHandler.closeSnifferPort())
                {
                    btnSnifferSerialPort.Text = CONNECT_TEXT;
                    cmbBoxSnifferSerialPorts.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Load available serial ports
        /// </summary>
        private void loadAvailableComports()
        {
            string[] serialPorts = SerialPortHdlr.getAvailableSerialPorts();
            foreach (string com in serialPorts)
            {
                cmbBoxBlueBoxSerialPorts.Items.Add(com);
                cmbBoxSnifferSerialPorts.Items.Add(com);
            }
        }

    }
}
