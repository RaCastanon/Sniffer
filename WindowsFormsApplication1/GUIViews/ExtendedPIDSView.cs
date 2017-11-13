﻿using System;
using System.Windows.Forms;
using ViewsHandler;
using SLIPProtocol;
using SerialPortInterface;
using BlueBoxProtocol;
using MessageContainerData;
using System.Collections.Generic;

namespace DiagnosticTool.GUIViews
{
    public partial class ExtendedPIDSView : UserControl , ViewInterface
    {
        /* GUI related variables */
        private static string CRLF = "\r\n";
        private ViewStatus CurrentViewStatus;
        private CommunicationStatus CurrentCommunicationStatus;
        private SendAVCLANMessage SendMessageHandler;
        private DisplayFormatData DisplayDataHandler;
        private Timer PeriodicMessageTimer;
        private Timer ProcessTimer;
        private Dictionary<int, string> TimeSettings;
        /* Logic related variables */
        private byte[] NodeAVCLANAddress = null;
        private uint StateStep = 0;

        /// <summary>
        /// 
        /// </summary>
        public ExtendedPIDSView()
        {
            InitializeComponent();
            CurrentViewStatus = ViewStatus.DISABLE;
            CurrentCommunicationStatus = CommunicationStatus.DISABLE;
            DisplayDataHandler = new DisplayFormatData(richTextBoxMessages);

            PeriodicMessageTimer = new Timer();
            PeriodicMessageTimer.Tick += new EventHandler(timerEvent);
            PeriodicMessageTimer.Enabled = false;

            ProcessTimer = new Timer();
            ProcessTimer.Tick += new EventHandler(processTimerEvent);
            ProcessTimer.Interval = 10;
            ProcessTimer.Enabled = false;

            // configure time settings combobox
            TimeSettings = new Dictionary<int, string>();
            TimeSettings.Add(1, "ms.");
            TimeSettings.Add(2, "sec.");
            TimeSettings.Add(3, "min.");
            cmbBoxTimeUnit.DataSource = new BindingSource(TimeSettings, null);
            cmbBoxTimeUnit.DisplayMember = "Value";
            cmbBoxTimeUnit.ValueMember = "Key";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void processIncomingMessage(MessageContainer message)
        {
            if (null != message)
            {
                if (richTextBoxMessages.InvokeRequired)
                {
                    ReceiveAVCLANCallback cbk = new ReceiveAVCLANCallback(processIncomingMessage);
                    Invoke(cbk, new object[] { message });
                }
                else
                {
                    if ((ViewStatus.ENABLE == CurrentViewStatus) && (CommunicationStatus.ENABLE == CurrentCommunicationStatus))
                    {
                        if (message.IsMessageDirection == MessageType.RX_FROM_DEVICE)
                        {
                            printSlipMessage(GUI_MessageType.Incoming, message.MessageObject);
                        }
                        else if (message.IsMessageDirection == MessageType.TX_TO_DEVICE)
                        {
                            printSlipMessage(GUI_MessageType.Outgoing, message.MessageObject);
                        }
                        else if (message.IsMessageDirection == MessageType.TX_TO_DEVICE_FROM_SNIFFER)
                        {
                            printSlipMessage(GUI_MessageType.Outgoing_sniffer, message.MessageObject);
                        }
                        else
                        {
                            /* DO NOTHING!!! */
                        }
                    }   
                }
            }
        }

        private void printSlipMessage(GUI_MessageType message_type, SLIPMessage message)
        {
            BlueBoxMessage bb_message = new BlueBoxMessage(message);
            
            if (BlueBoxMessage.TX_FRAME_CMD == bb_message.MessageType)
            {
                MessageTX_FRAME_CMD message_tx = MessageTX_FRAME_CMD.getObject(bb_message, NodeAVCLANAddress);
                DisplayDataHandler.displayData(message_type,
                    bb_message.TimeStampString + " - " + message_tx.ToString() + CRLF);
            }
            else if (BlueBoxMessage.RX_FRAME_IND == bb_message.MessageType)
            {
                MessageRX_FRAME_IND message_rx = MessageRX_FRAME_IND.getObject(bb_message, NodeAVCLANAddress);
                DisplayDataHandler.displayData(message_type,
                    bb_message.TimeStampString + " - " + message_rx.ToString() + CRLF);
            }
            else if (BlueBoxMessage.INTF_CNTRL_CMD == bb_message.MessageType)
            {
                MessageINTF_CNTRL_CMD message_intf_cntrl_cmd = MessageINTF_CNTRL_CMD.getObject(bb_message);
                NodeAVCLANAddress = message_intf_cntrl_cmd.GetAddress;
                lblAVCLANCurrentAddr.Text = message_intf_cntrl_cmd.ToString();
            }
            /*else
            {
                DisplayDataHandler.displayData(GUI_MessageType.Normal,
                    bb_message.ToString() + " - " + message.ToString() + CRLF);
            }
            DisplayDataHandler.displayData(GUI_MessageType.Normal,
                    bb_message.ToString() + " - " + message.ToString() + CRLF);*/

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        public void setCommunicationStatus(CommunicationStatus status)
        {
            CurrentCommunicationStatus = status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void setCommunicationDelegate(SendAVCLANMessage callback)
        {
            if (null != callback)
            {
                SendMessageHandler = callback;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        public void setViewStatus(ViewStatus status)
        {
            CurrentViewStatus = status;
            Console.WriteLine(this.Name + " " + status.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ListenerType getListenerType()
        {
            return ListenerType.GUI_LISTENER;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            if ((CurrentViewStatus == ViewStatus.ENABLE) && (CurrentCommunicationStatus == CommunicationStatus.ENABLE))
            {
                string message = txtBoxMessage.Text;
                string process_message = message.Replace(" ", "");
                if ((process_message.Length > 0) && ByteUtilities.isHexString(process_message) && ((process_message.Length % 2) == 0))
                {
                    SLIPMessage slip_message = new SLIPMessage(ByteUtilities.HexToByte(process_message));
                    MessageContainer message_to_send = new MessageContainer(slip_message, MessageType.TX_TO_DEVICE);
                    SendMessageHandler(message_to_send);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {
            richTextBoxMessages.Clear();
            richTextBoxMessages.ScrollToCaret();
        }

        private void btnSetAddress_Click(object sender, EventArgs e)
        {

        }

        private void chkBoxPeriodically_CheckedChanged(object sender, EventArgs e)
        {
            if ((CurrentViewStatus == ViewStatus.ENABLE) && (CurrentCommunicationStatus == CommunicationStatus.ENABLE))
            {

                if (chkBoxPeriodically.Checked == true)
                {
                    if (sendMessageAction())
                    {
                        btnSendMessage.Enabled = false;
                        cmbBoxTimeUnit.Enabled = false;
                        numUpDwnTime.Enabled = false;
                        txtBoxMessage.Enabled = false;
                        // Start timer
                        int periodic_time = (int)numUpDwnTime.Value;
                        int time_base = ((KeyValuePair<int, string>)cmbBoxTimeUnit.SelectedItem).Key;
                        // From dictionary get time base
                        if (1 == time_base)
                        {
                            // ms
                            periodic_time = periodic_time * 1;
                        }
                        else if (2 == time_base)
                        {
                            // sec
                            periodic_time = periodic_time * 1000;
                        }
                        if (3 == time_base)
                        {
                            // min
                            periodic_time = periodic_time * 1000 * 60;
                        }
                        else
                        {
                            // Do nothing
                        }
                        PeriodicMessageTimer.Interval = periodic_time;
                        PeriodicMessageTimer.Start();
                        PeriodicMessageTimer.Enabled = true;
                        ProcessTimer.Start();
                        ProcessTimer.Enabled = true;
                    }
                    else
                    {
                        chkBoxPeriodically.Checked = false;
                    }
                }
                else
                {
                    btnSendMessage.Enabled = true;
                    cmbBoxTimeUnit.Enabled = true;
                    numUpDwnTime.Enabled = true;
                    txtBoxMessage.Enabled = true;
                    // Stop timer
                    PeriodicMessageTimer.Stop();
                    PeriodicMessageTimer.Enabled = false;
                    ProcessTimer.Stop();
                    ProcessTimer.Enabled = false;
                }
            }
            else
            {
                chkBoxPeriodically.Checked = false;
                btnSendMessage.Enabled = true;
                cmbBoxTimeUnit.Enabled = true;
                numUpDwnTime.Enabled = true;
                txtBoxMessage.Enabled = true;
                // Stop timer
                PeriodicMessageTimer.Stop();
                PeriodicMessageTimer.Enabled = false;
                ProcessTimer.Stop();
                ProcessTimer.Enabled = false;
            }
        }

        private bool sendMessageAction()
        {
            bool valid_message = false;
            string message = txtBoxMessage.Text;
            string process_message = message.Replace(" ", "");

            if ((process_message.Length > 0) && ByteUtilities.isHexString(process_message) && ((process_message.Length % 2) == 0))
            {
                SLIPMessage slip_message = new SLIPMessage(ByteUtilities.HexToByte(process_message));
                MessageContainer message_to_send = new MessageContainer(slip_message, MessageType.TX_TO_DEVICE);
                SendMessageHandler(message_to_send);
                valid_message = true;
            }
            return valid_message;
        }

        private void sendMessage(string slipHexMsg)
        {
            if ((slipHexMsg.Length > 0) && ByteUtilities.isHexString(slipHexMsg) && ((slipHexMsg.Length % 2) == 0))
            {
                SLIPMessage slip_message = new SLIPMessage(ByteUtilities.HexToByte(slipHexMsg));
                MessageContainer message_to_send = new MessageContainer(slip_message, MessageType.TX_TO_DEVICE);
                SendMessageHandler(message_to_send);
            }           
        }

        private bool sendMessageSequence()
        {
            bool valid_message = false;
            /*
             * Vol up msg     = 0101000804400025749D01
             * Vol down msg   = 0101000804400025749C01
             * Prep msg       = 010100080440005601BA00
             * Exec msg       = 010100080440005601BA01
             * Source change  = 0101000804400011748E62
             * Src change msg = 
             */
            string process_message_1 = "010100070440005601BA00";
            string process_message_2 = "010100070440005601BA01";
            string process_message_3 = "0101000704400011748E62";

            
            System.Threading.Thread.Sleep(100);
            //Message One
            sendMessage(process_message_1);
            System.Threading.Thread.Sleep(100);
            //Message Two
            sendMessage(process_message_2);
            System.Threading.Thread.Sleep(100);
            //Message Three
            sendMessage(process_message_3);
            valid_message = true;
            
            return valid_message;
        }

        private void timerEvent(object sender, EventArgs e)
        {
            if ((CurrentViewStatus == ViewStatus.ENABLE) && (CurrentCommunicationStatus == CommunicationStatus.ENABLE))
            {
                string process_message = txtBoxMessage.Text;
                if ((process_message.Length > 0) && ("10" == process_message))
                {
                    StateStep = 0;
                    ProcessTimer.Enabled = true;
                }                
            }
        }

        private void processTimerEvent(object sender, EventArgs e)
        {
            string process_message_1 = "010100070440005601BA00";
            string process_message_2 = "010100070440005601BA01";
            string process_message_3 = "0101000704400011748E62";

            switch (StateStep)
            {
                case 0:
                    sendMessage(process_message_1);
                    ProcessTimer.Interval = 100;
                    StateStep = 1;
                    break;

                case 1:
                    sendMessage(process_message_2);
                    ProcessTimer.Interval = 100;
                    StateStep = 2;
                    break;

                case 2:
                    sendMessage(process_message_3);
                    ProcessTimer.Interval = 100;
                    StateStep = 3;
                    ProcessTimer.Enabled = false;
                    break;

                case 3:
                    break;

            }


        }
    }
}
