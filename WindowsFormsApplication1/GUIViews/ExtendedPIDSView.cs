using System;
using System.Windows.Forms;
using ViewsHandler;
using SLIPProtocol;
using SerialPortInterface;
using BlueBoxProtocol;
using MessageContainerData;
using System.Collections.Generic;

namespace DiagnosticTool.GUIViews
{
    public partial class ExtendedPIDSView : UserControl, ViewInterface
    {
        public byte Source;
        /* GUI related variables */
        private static string CRLF = "\r\n";
        private ViewStatus CurrentViewStatus;
        private CommunicationStatus CurrentCommunicationStatus;
        private SendAVCLANMessage SendMessageHandler;
        private DisplayFormatData DisplayDataHandler;
        private Timer PeriodicMessageTimer;

        //HF test cases
        private Timer testCase_1;
        private Timer testCase_2;
        private Timer testCase_3;

        //Beeps test cases
        private Timer testCase_5;
        private Timer testCase_6;
        private Timer testCase_7;
        private Timer testCase_8;
        private Timer testCase_9;
        private Timer testCase_10;

        private Dictionary<int, string> TimeSettings;
        /* Logic related variables */
        private byte[] NodeAVCLANAddress = null;
        private uint StateStep = 0;
        private uint vol_cnt = 0;
        private uint InitialStep = 0;

        private byte[] F5_Status = new byte[] { 0x00, 0x00, 0x74, 0x31, 0xF5, 0x03, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E }; /*Volume response*/
        private byte[] Source_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x11, 0x8F, 0x00, 0x00 }; /* Source change response*/
        private byte[] Device_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x6D, 0x83, 0x00 }; /* HF device change response*/
        private int upDown = 1; /* 0 = down, 1 = up */
        private int HFdevice = 0; /* Range 0 - 5 */
        private int audioSource; /* 0 = TONE, 1 = TALK */
        private bool response_rx = false;
        private const int MAXIMUM_AUDIO_SOURCE = 17;

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

            testCase_1 = new Timer();
            testCase_1.Tick += new EventHandler(testCase_1_TimerEvent);
            testCase_1.Interval = 100;
            testCase_1.Enabled = false;

            testCase_2 = new Timer();
            testCase_2.Tick += new EventHandler(testCase_2_TimerEvent);
            testCase_2.Interval = 100;
            testCase_2.Enabled = false;

            testCase_3 = new Timer();
            testCase_3.Tick += new EventHandler(testCase_3_TimerEvent);
            testCase_3.Interval = 100;
            testCase_3.Enabled = false;

            /*Timers for Beep tests*/

            testCase_5 = new Timer();
            testCase_5.Tick += new EventHandler(testCase_5_TimerEvent);
            testCase_5.Interval = 30;
            testCase_5.Enabled = false;

            testCase_6 = new Timer();
            testCase_6.Tick += new EventHandler(testCase_6_TimerEvent);
            testCase_6.Interval = 3000;
            testCase_6.Enabled = false;

            testCase_7 = new Timer();
            testCase_7.Tick += new EventHandler(testCase_7_TimerEvent);
            testCase_7.Interval = 1000;
            testCase_7.Enabled = false;

            testCase_8 = new Timer();
            testCase_8.Tick += new EventHandler(testCase_8_TimerEvent);
            testCase_8.Interval = 1000;
            testCase_8.Enabled = false;

            testCase_9 = new Timer();
            testCase_9.Tick += new EventHandler(testCase_9_TimerEvent);
            testCase_9.Interval = 1000;
            testCase_9.Enabled = false;

            testCase_10 = new Timer();
            testCase_10.Tick += new EventHandler(testCase_10_TimerEvent);
            testCase_10.Interval = 1000;
            testCase_10.Enabled = false;

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

                CompareMessage(bb_message.MessageData, bb_message.MessageLength);
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
                        InitialStep = 0;
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
                    // Stop timers
                    PeriodicMessageTimer.Stop();
                    PeriodicMessageTimer.Enabled = false;
                    testCase_1.Stop();
                    testCase_1.Enabled = false;
                    testCase_2.Stop();
                    testCase_2.Enabled = false;
                    testCase_3.Stop();
                    testCase_3.Enabled = false;
                    testCase_5.Stop();
                    testCase_5.Enabled = false;
                    testCase_6.Stop();
                    testCase_6.Enabled = false;
                    testCase_7.Stop();
                    testCase_7.Enabled = false;
                    testCase_8.Stop();
                    testCase_8.Enabled = false;
                    testCase_9.Stop();
                    testCase_9.Enabled = false;
                    testCase_10.Stop();
                    testCase_10.Enabled = false;
                    StateStep = 0;
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
                testCase_1.Stop();
                testCase_1.Enabled = false;
                testCase_2.Stop();
                testCase_2.Enabled = false;
                testCase_3.Stop();
                testCase_3.Enabled = false;
                testCase_5.Stop();
                testCase_5.Enabled = false;
                testCase_6.Stop();
                testCase_6.Enabled = false;
                testCase_7.Stop();
                testCase_7.Enabled = false;
                testCase_8.Stop();
                testCase_8.Enabled = false;
                testCase_9.Stop();
                testCase_9.Enabled = false;
                testCase_10.Stop();
                testCase_10.Enabled = false;
                StateStep = 0;
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

        private void CompareMessage(byte[] message, int lenght)
        {
            uint index;
            if ((message[2] == 0x74) && (message[3] == 0x31) && (message[4] == 0xF5))
            {
                for (index = 2; index < lenght; index++)
                {
                    if (message[index] != F5_Status[index])
                    {
                        DisplayDataHandler.displayData(DiagnosticTool.GUIViews.GUI_MessageType.Error, "Expected:" + BitConverter.ToString(F5_Status).Replace("-", " ") + CRLF);
                        return;
                    }
                }
            }

            if ((message[3] == 0x74) && (message[4] == 0x11) && (message[5] == 0x8F))
            {
                response_rx = true;
                for (index = 3; index < lenght; index++)
                {
                    if (message[index] != Source_Status[index])
                    {
                        DisplayDataHandler.displayData(DiagnosticTool.GUIViews.GUI_MessageType.Error, "Expected:" + BitConverter.ToString(Source_Status).Replace("-", " ") + CRLF);
                        return;
                    }
                }
            }

            if ((message[3] == 0x74) && (message[4] == 0x6D) && (message[5] == 0x83))
            {
                response_rx = true;
                for (index = 3; index < lenght; index++)
                {
                    if (message[index] != Device_Status[index])
                    {
                        DisplayDataHandler.displayData(DiagnosticTool.GUIViews.GUI_MessageType.Error, "Expected:" + BitConverter.ToString(Device_Status).Replace("-", " ") + CRLF);
                        return;
                    }
                }
            }
        }

        private void timerEvent(object sender, EventArgs e)
        {
            if ((CurrentViewStatus == ViewStatus.ENABLE) && (CurrentCommunicationStatus == CommunicationStatus.ENABLE))
            {
                //Test case order
                string testCase = txtBoxMessage.Text;
                if (0 < testCase.Length)
                {
                    switch (testCase)
                    {
                        case "0001":
                            StateStep = InitialStep;
                            testCase_1.Interval = 100;
                            testCase_1.Start();
                            testCase_1.Enabled = true;
                            break;

                        case "0010":
                            StateStep = InitialStep;
                            testCase_2.Interval = 10;
                            testCase_2.Start();
                            testCase_2.Enabled = true;
                            break;

                        case "0011":
                            StateStep = InitialStep;
                            testCase_3.Interval = 10;
                            testCase_3.Start();
                            testCase_3.Enabled = true;
                            break;

                        case "0100":
                            break;

                        /*Beeps test cases*/
                        case "0101":
                            testCase_5.Interval = 30;
                            testCase_5.Start();
                            StateStep = InitialStep;
                            break;

                        case "0110":
                            testCase_6.Start();
                            StateStep = InitialStep;
                            break;

                        case "0111":
                            testCase_7.Start();
                            StateStep = InitialStep;
                            break;

                        case "1000":
                            testCase_8.Start();
                            StateStep = InitialStep;
                            break;

                        case "1001":
                            testCase_9.Start();
                            StateStep = InitialStep;
                            break;

                        case "1010":
                            testCase_10.Start();
                            StateStep = InitialStep;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /*
            Case 0001
            - Set audio source to HF Tone or HF Talk
            - Select HF device 1
            - Every 100 ms:
                -Increase/decrease volume
                -Switch to next HF device (rotate from last to first device)
                -Repeat
        */
        private void testCase_1_TimerEvent(object sender, EventArgs e)
        {
            string prep_msg = "010100070440005601BA00";
            string exec_msg = "010100070440005601BA01";
            string HF_TalkSrc_msg = "0101000704400011748E6D";
            string vol_up_msg = "0101000704400025749C01";
            string vol_dw_msg = "0101000804400025749D01";
            string HF_dev0_msg = "010100070440006D749300";
            string HF_dev1_msg = "010100070440006D749301";
            string HF_dev2_msg = "010100070440006D749302";
            string HF_dev3_msg = "010100070440006D749303";
            string HF_dev4_msg = "010100070440006D749304";
            string HF_dev5_msg = "010100070440006D749305";

            switch (StateStep)
            {
                case 0: //Prep command
                    sendMessage(prep_msg);
                    testCase_1.Interval = 100;
                    StateStep = 1;

                    upDown = 1;
                    HFdevice = 0;
                    audioSource = 1; //HF_Talk device
                    response_rx = false;
                    F5_Status = new byte[] { 0x00, 0x00, 0x74, 0x31, 0xF5, 0x03, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E };
                    Source_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x11, 0x8F, 0x00, 0x00 }; /* Source change response*/
                    Device_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x6D, 0x83, 0x00 }; /* HF device change response*/
                    break;

                case 1: //Execute command
                    sendMessage(exec_msg);
                    testCase_1.Interval = 100;
                    StateStep = 2;
                    break;

                case 2: //Set HF_talk source cmd
                    sendMessage(HF_TalkSrc_msg);
                    testCase_1.Interval = 100;
                    Source_Status[6] = 0x6D;
                    StateStep = 3;
                    break;

                case 3: //Set device 0 (Tone)
                    sendMessage(HF_dev0_msg);
                    testCase_1.Interval = 100;
                    Device_Status[6] = 0x00;
                    StateStep = 4;
                    InitialStep = 4;
                    break;

                /* Test Steps */
                case 4: //Vol up command
                    //Verify upDown bit
                    if (1 == upDown)
                    {
                        sendMessage(vol_up_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]++;

                        if (0x3F == F5_Status[MAXIMUM_AUDIO_SOURCE])
                        {
                            upDown = 0;
                        }
                    }
                    else if (0 == upDown)
                    {
                        sendMessage(vol_dw_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]--;

                        if (0x00 == F5_Status[MAXIMUM_AUDIO_SOURCE])
                        {
                            upDown = 1;
                        }
                    }
                    testCase_1.Interval = 100;
                    StateStep = 5;
                    break;

                case 5: //Change device
                    switch (HFdevice)
                    {
                        case 0:
                            sendMessage(HF_dev1_msg);
                            Device_Status[6] = 0x01;
                            HFdevice = 1;
                            break;

                        case 1:
                            sendMessage(HF_dev2_msg);
                            Device_Status[6] = 0x02;
                            HFdevice = 2;
                            break;

                        case 2:
                            sendMessage(HF_dev3_msg);
                            Device_Status[6] = 0x03;
                            HFdevice = 3;
                            break;

                        case 3:
                            sendMessage(HF_dev4_msg);
                            Device_Status[6] = 0x04;
                            HFdevice = 4;
                            break;

                        case 4:
                            sendMessage(HF_dev5_msg);
                            Device_Status[6] = 0x05;
                            HFdevice = 5;
                            break;

                        case 5:
                            sendMessage(HF_dev0_msg);
                            Device_Status[6] = 0x00;
                            HFdevice = 0;
                            break;

                        default:
                            break;
                    }
                    testCase_1.Interval = 100;
                    StateStep = 6;
                    break;

                case 6: //Wait for amp response
                    if (response_rx == true)
                    {
                        response_rx = false;
                        StateStep = 7;
                        testCase_1.Interval = 10;
                    }
                    else
                    {
                        testCase_1.Interval = 10;
                    }
                    break;

                default:
                    break;
            }
        }

        /*
            Case 0002
            - Set audio source to HF Tone
            - Select HF device 1
            - Every 100 ms:
                -Set audio source to HF Tone
                -Increase/decrease volume
                -Switch to HF TALK
                -Increase/decrease volume
                -Repeat
        */
        private void testCase_2_TimerEvent(object sender, EventArgs e)
        {
            string prep_msg = "010100070440005601BA00";
            string exec_msg = "010100070440005601BA01";
            string HF_TalkSrc_msg = "0101000704400011748E6D";
            string HF_Tone_msg = "0101000704400011748EC6";
            string vol_up_msg = "0101000704400025749C01";
            string vol_dw_msg = "0101000804400025749D01";
            string HF_dev0_msg = "010100070440006D749300";

            HFdevice = 0; //This test will only be performed in one device

            switch (StateStep)
            {
                //Set amp at default conditions
                case 0: //Prep msg
                    sendMessage(prep_msg);
                    testCase_2.Interval = 100;
                    StateStep = 1;

                    F5_Status = new byte[] { 0x00, 0x00, 0x74, 0x31, 0xF5, 0x03, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E };
                    break;

                case 1: //Exec msg
                    sendMessage(exec_msg);
                    testCase_2.Interval = 100;
                    StateStep = 2;
                    break;

                case 2: //Set tone audio source
                    sendMessage(HF_Tone_msg);
                    testCase_2.Interval = 100;
                    StateStep = 3;
                    audioSource = 0;
                    Source = 0xC6;
                    Source_Status[6] = 0xC6;
                    break;

                case 3: //Send device 0 msg
                    sendMessage(HF_dev0_msg);
                    testCase_2.Interval = 100;
                    StateStep = 4;
                    InitialStep = 4;
                    break;

                /*Init test*/
                case 4: //Send volume command
                    //Verify upDown bit
                    if (1 == upDown)
                    {
                        sendMessage(vol_up_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]++;

                        if (0x3F == F5_Status[7]) //Verify volume overload on the device 0
                        {
                            upDown = 0;
                        }
                    }
                    else if (0 == upDown)
                    {
                        sendMessage(vol_dw_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]--;

                        if (0x00 == F5_Status[7]) //Verify volume overload on the device 0
                        {
                            upDown = 1;
                        }
                    }
                    testCase_1.Interval = 100;
                    StateStep = 5;
                    break;

                case 5: //Send audio source change command
                    switch (audioSource)
                    {
                        case 0:
                            sendMessage(HF_TalkSrc_msg);
                            Source = 0x6D;
                            Source_Status[6] = 0x6D;
                            audioSource = 1;
                            break;

                        case 1:
                            sendMessage(HF_Tone_msg);
                            Source = 0xC6;
                            Source_Status[6] = 0xC6;
                            audioSource = 0;
                            break;

                        default:
                            break;
                    }
                    testCase_2.Interval = 100;
                    StateStep = 6;
                    break;

                case 6:
                    if (response_rx == true)
                    {
                        response_rx = false;
                        StateStep = 7;
                        testCase_2.Interval = 10;
                    }
                    else
                    {
                        testCase_2.Interval = 10;
                    }
                    break;

                default:
                    break;

            }
        }

        private void testCase_3_TimerEvent(object sender, EventArgs e)
        {
            string PersonalInitPrep_msg = "010100070440005601BA00";
            string PersonalInitExce_msg = "010100070440005601BA01";
            string SourceCDP_msg = "0101000704400011748E62";
            string SourceHFTone_msg = "0101000704400011748EC6";
            string SourceHFTalk_msg = "0101000704400011748E6D";
            string SelectHFDevice0_msg = "010100070440006D749300";
            string SelectHFDevice1_msg = "010100070440006D749301";
            string SelectHFDevice2_msg = "010100070440006D749302";
            string SelectHFDevice3_msg = "010100070440006D749303";
            string SelectHFDevice4_msg = "010100070440006D749304";
            string SelectHFDevice5_msg = "010100070440006D749305";
            string VolumeUp_msg = "0101000704400025749C01";
            string VolumeDown_msg = "0101000704400025749D01";

            switch (StateStep)
            {
                case 0:
                    sendMessage(PersonalInitPrep_msg);
                    testCase_3.Interval = 100;
                    StateStep = 1;

                    upDown = 1;
                    HFdevice = 0;
                    audioSource = 0;
                    InitialStep = 4;
                    response_rx = false;
                    F5_Status = new byte[] { 0x00, 0x00, 0x74, 0x31, 0xF5, 0x03, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E, 0x1E };
                    Source_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x11, 0x8F, 0x00, 0x00 }; /* Source change response*/
                    Device_Status = new byte[] { 0x00, 0x00, 0x00, 0x74, 0x6D, 0x83, 0x00 }; /* HF device change response*/
                    break;

                case 1:
                    sendMessage(PersonalInitExce_msg);
                    testCase_3.Interval = 100;
                    StateStep = 2;
                    break;

                case 2:
                    sendMessage(SourceHFTone_msg);
                    Source_Status[6] = 0xC6;
                    testCase_3.Interval = 100;
                    StateStep = 3;
                    break;

                case 3:
                    sendMessage(SelectHFDevice0_msg);
                    Device_Status[6] = 0x00;
                    testCase_3.Interval = 100;
                    StateStep = 0xFFFFFFFF;
                    break;

                /* Test Steps*/
                case 4:
                    if (upDown == 1)
                    {
                        sendMessage(VolumeUp_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]++;

                        if (F5_Status[6 + (HFdevice * 2) + audioSource] == 0x3F)
                        {
                            upDown = 0;
                        }
                    }
                    else if (upDown == 0)
                    {
                        sendMessage(VolumeDown_msg);
                        F5_Status[6 + (HFdevice * 2) + audioSource]--;

                        if (F5_Status[6 + (HFdevice * 2) + audioSource] == 0x00)
                        {
                            upDown = 1;
                        }
                    }

                    testCase_3.Interval = 100;
                    StateStep = 5;
                    break;

                case 5:

                    switch (audioSource)
                    {
                        case 0:
                            sendMessage(SourceHFTalk_msg);
                            Source = 0x6D;
                            Source_Status[6] = 0x6D;
                            audioSource = 1;
                            break;

                        case 1:
                            sendMessage(SourceHFTone_msg);
                            Source = 0xC6;
                            Source_Status[6] = 0xC6;
                            audioSource = 0;
                            break;

                        default:
                            break;
                    }

                    testCase_3.Interval = 10;
                    StateStep = 6;
                    break;

                case 6:
                    if (response_rx == true)
                    {
                        response_rx = false;
                        StateStep = 7;
                        testCase_3.Interval = 10;
                    }
                    else
                    {
                        testCase_3.Interval = 10;
                    }
                    break;

                case 7:
                    if (audioSource == 0)
                    {
                        switch (HFdevice)
                        {
                            case 0:
                                sendMessage(SelectHFDevice1_msg);
                                Device_Status[6] = 0x01;
                                HFdevice = 1;
                                break;

                            case 1:
                                sendMessage(SelectHFDevice2_msg);
                                Device_Status[6] = 0x02;
                                HFdevice = 2;
                                break;

                            case 2:
                                sendMessage(SelectHFDevice3_msg);
                                Device_Status[6] = 0x03;
                                HFdevice = 3;
                                break;

                            case 3:
                                sendMessage(SelectHFDevice4_msg);
                                Device_Status[6] = 0x04;
                                HFdevice = 4;
                                break;

                            case 4:
                                sendMessage(SelectHFDevice5_msg);
                                Device_Status[6] = 0x05;
                                HFdevice = 5;
                                break;

                            case 5:
                                sendMessage(SelectHFDevice0_msg);
                                Device_Status[6] = 0x00;
                                HFdevice = 0;
                                break;

                            default:
                                break;
                        }
                    }


                    testCase_3.Interval = 30;
                    StateStep = 8;
                    break;

                case 8:
                    if (response_rx == true)
                    {
                        response_rx = false;
                        StateStep = 9;
                        testCase_3.Interval = 10;
                    }
                    else
                    {
                        testCase_3.Interval = 10;
                    }
                    break;

                default:
                    break;

            }
        }

        /*
           Case 0101
           - send beep 
           - send another Beep notif, before previous beep finishes
       */
        private void testCase_5_TimerEvent(object sender, EventArgs e)
        {
            string beep_50_ms = "0101000704400000296001";
            switch (StateStep)
            {
                case 0:
                    sendMessage(beep_50_ms);
                    StateStep = 1;
                    break;
                case 1:
                    sendMessage(beep_50_ms);
                    StateStep = 2;
                    break;
                case 2:
                    sendMessage(beep_50_ms);
                    StateStep = 3;
                    break;/*
                case 3:
                    sendMessage(beep_50_ms);
                    StateStep = 4;
                    break;
                case 4:
                    sendMessage(beep_50_ms);
                    StateStep = 5;
                    break;*/
                default:
                    break;
            }
        }

        /*
           Case 0110
           - send extended_beep notification
           - send another extended_beep notif, before previous beep finishes and with different 
             on time and off time settings
       */
        private void testCase_6_TimerEvent(object sender, EventArgs e)
        {
            string base_ext_beep_notif = "01010009044000002961050101";// 61 op_code, 05 cycles, 01 period on time, 01 period off time
            string ext_beep_notif_mod_2 = "01010009044000002961070203";
            string ext_beep_notif_mod_3 = "01010009044000002961080302";

            //Timer setup interval for 1 second
            switch (StateStep)
            {
                case 0:
                    sendMessage(base_ext_beep_notif);
                    testCase_6.Interval = 1000;
                    StateStep = 1;
                    break;
                //Increase cycles, on duration and off duration
                case 1:
                    sendMessage(ext_beep_notif_mod_2);
                    StateStep = 2;
                    break;
                //Increase cycles, on duration and off duration
                case 2:
                    sendMessage(ext_beep_notif_mod_3);
                    StateStep = 3;
                    StateStep = InitialStep;
                    break;
                default:
                    InitialStep = 0;
                    testCase_6.Interval = 2000;
                    break;

            }
        }
        /*
           Case 0111
           - send beep_notification
           - send extended_beep_notif, before previous beep finishes
           - repeat
       */
        private void testCase_7_TimerEvent(object sender, EventArgs e)
        {
            string beep_250_ms = "0101000704400000296002"; //beep_250ms
            string base_ext_beep_notif = "01010009044000002961050101";// 61 op_code, 05 cycles, 01 period on time, 01 period off time

            switch (StateStep)
            {
                case 0:
                    sendMessage(beep_250_ms);
                    StateStep = 1;
                    InitialStep = 0;
                    testCase_7.Interval = 200;
                    break;
                case 1:
                    sendMessage(beep_250_ms);
                    testCase_7.Interval = 200;
                    StateStep = 2;
                    break;
                case 2:
                    sendMessage(base_ext_beep_notif);
                    testCase_7.Interval = 800;
                    StateStep = 3;
                    break;
                case 3:
                    sendMessage(base_ext_beep_notif);
                    testCase_7.Interval = 800;
                    StateStep = 4;
                    break;
                case 4:
                    sendMessage(beep_250_ms);
                    testCase_7.Interval = 200;
                    StateStep = 5;
                    break;
                case 5:
                    sendMessage(base_ext_beep_notif);
                    testCase_7.Interval = 800;
                    StateStep = 6;
                    break;
                default:
                    break;
            }
        }
        /*
           Case 1000
           - send beep_notification
           - send another beep notification, before previous beep finishes
           - send IPA beep 
           - repeat
       */
        private void testCase_8_TimerEvent(object sender, EventArgs e)
        {
            string beep_250_ms = "0101000704400000296002"; //beep_250ms
            string IPABeep = "0101000904400000296C500101";// IPA beep 10 cycles

            switch (StateStep)
            {
                case 0:
                    sendMessage(beep_250_ms);
                    StateStep = 1;
                    InitialStep = 0;
                    testCase_8.Interval = 200;
                    break;
                case 1:
                    sendMessage(beep_250_ms);
                    testCase_8.Interval = 200;
                    StateStep = 2;
                    break;
                case 2:
                    sendMessage(IPABeep);
                    testCase_8.Interval = 1000;
                    StateStep = 3;
                    break;
                default:
                    break;
            }
        }
        /*
           Case 1001
           - send extended_beep_notification
           - send another beep notification, before previous beep finishes
           - send IPA beep 
           - repeat
       */
        private void testCase_9_TimerEvent(object sender, EventArgs e)
        {
            string base_ext_beep_notif_1 = "01010009044000002961050101";// 61 op_code, 05 cycles, 01 period on time, 01 period off time
            string base_ext_beep_notif_2 = "01010009044000002961070202";
            string IPABeep = "0101000904400000296C500101";// IPA beep 10 cycles

            switch (StateStep)
            {
                //send extended beep command
                case 0:
                    sendMessage(base_ext_beep_notif_1);
                    StateStep = 1;
                    InitialStep = 0;
                    testCase_9.Interval = 300;
                    break;
                //send another extended beep command
                case 1:
                    sendMessage(base_ext_beep_notif_2);
                    testCase_9.Interval = 100;
                    StateStep = 2;
                    break;
                //send IPA beep command
                case 2:
                    sendMessage(IPABeep);
                    testCase_9.Interval = 3000;
                    StateStep = 3;
                    break;
                default:
                    break;
            }
        }
        /*
           Case 1010
           - send beep_notification
           - send extended_beep notification, before previous beep finishes
           - send IPA beep 
           - repeat
       */
        private void testCase_10_TimerEvent(object sender, EventArgs e)
        {
            string beep_notif_250ms = "0101000704400000296002";
            string base_ext_beep_notif = "01010009044000002961070101";// 61 op_code, 05 cycles, 01 period on time, 01 period off time
            string IPABeep = "0101000904400000296C500101";// IPA beep 10 cycles

            switch (StateStep)
            {
                //send extended beep command
                case 0:
                    sendMessage(beep_notif_250ms);
                    StateStep = 1;
                    InitialStep = 0;
                    testCase_10.Interval = 100;
                    break;
                //send another extended beep command
                case 1:
                    sendMessage(base_ext_beep_notif);
                    testCase_10.Interval = 100;
                    StateStep = 2;
                    break;
                //send IPA beep command
                case 2:
                    sendMessage(IPABeep);
                    testCase_10.Interval = 3000;
                    StateStep = 3;
                    break;
                default:
                    break;
            }
        }
    }
}
