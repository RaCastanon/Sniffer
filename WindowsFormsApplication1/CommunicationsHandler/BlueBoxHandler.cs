using BlueBoxProtocol;
using MessageContainerData;
using SerialPortInterface;
using SLIPProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ViewsHandler;

namespace CommunicationsHandler
{
    public class AVCLANHandler
    {
        private Thread ViewHandlerAVCLAN_RxThread = null;
        private Thread ViewHandlerAVCLAN_TxThread = null;
        private bool ThreadRunning = true;
        private Dictionary<string, ViewInterface> AVCLAN_Listeners;
        private SerialPortHdlr AVCLANSerialPort = null;
        private string CurrentAVCLAN_KeyView = "";
        private Queue<MessageContainer> AVCLAN_MessagesToSend;
        private Queue<MessageContainer> AVCLAN_ReceivedMessages;
        private SLIPDecoder SLIPDecoderObject = new SLIPDecoder();
        private SLIPEncoder SLIPEncoderObject = new SLIPEncoder();
        private static AutoResetEvent AVCLAN_RxMessageWaitSemaphore = new AutoResetEvent(false);
        private static AutoResetEvent AVCLAN_TxMessageWaitSemaphore = new AutoResetEvent(false);
        
        /// <summary>
        /// 
        /// </summary>
        public AVCLANHandler()
        {
            AVCLAN_Listeners = new Dictionary<string, ViewInterface>();
            AVCLAN_MessagesToSend = new Queue<MessageContainer>();
            AVCLAN_ReceivedMessages = new Queue<MessageContainer>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void startAVCLANHandler()
        {
            if (null == ViewHandlerAVCLAN_TxThread)
            {
                ViewHandlerAVCLAN_TxThread = new Thread(new ThreadStart(threadAVCLANTxTask));
                ViewHandlerAVCLAN_TxThread.Start();
            }

            if (null == ViewHandlerAVCLAN_RxThread)
            {
                ViewHandlerAVCLAN_RxThread = new Thread(new ThreadStart(threadAVCLANRxTask));
                ViewHandlerAVCLAN_RxThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void stopAVCLANHandler()
        {
            if ((null != ViewHandlerAVCLAN_TxThread) || (null != ViewHandlerAVCLAN_RxThread))
            {
                ThreadRunning = false;
                AVCLAN_TxMessageWaitSemaphore.Set();
                AVCLAN_RxMessageWaitSemaphore.Set();
            }
        }

        /// <summary>
        /// Add View
        /// </summary>
        /// <param name="key_view"></param>
        /// <param name="view"></param>
        public void addAVCLANListener(String key_view, ViewInterface listener)
        {
            if ((key_view != null) && (listener != null))
            {
                AVCLAN_Listeners.Add(key_view, listener);
                listener.setCommunicationDelegate(new SendAVCLANMessage(sendBlueBoxMessage));
            }
        }

        /// <summary>
        /// Enable view
        /// </summary>
        /// <param name="key_view"></param>
        public void setCurrentAVCLANListener(String key_view)
        {
            if ((!key_view.Equals(CurrentAVCLAN_KeyView)) && (AVCLAN_Listeners.ContainsKey(key_view)))
            {
                /* Disable current view */
                if (AVCLAN_Listeners.ContainsKey(CurrentAVCLAN_KeyView))
                {
                    AVCLAN_Listeners[CurrentAVCLAN_KeyView].setViewStatus(ViewStatus.DISABLE);
                }
                /* Enable current view */
                AVCLAN_Listeners[key_view].setViewStatus(ViewStatus.ENABLE);
                CurrentAVCLAN_KeyView = key_view;
            }
        }

        /// <summary>
        /// Open Serial Port
        /// </summary>
        /// <param name="com_port"></param>
        /// <returns></returns>
        public bool connectAVCLANPort(string com_port)
        {
            bool connection_success = false;
            if (null == AVCLANSerialPort)
            {
                AVCLANSerialPort = new SerialPortHdlr("57600", "0", "1", "8", com_port,
                                                new SerialPortHdlr.ReportIncomingData(AVCLANPortIncomingData));
                if (AVCLANSerialPort.OpenPort())
                {
                    connection_success = true;
                    foreach (var view in AVCLAN_Listeners)
                    {
                        view.Value.setCommunicationStatus(CommunicationStatus.ENABLE);
                    }
                }
                else
                {
                    AVCLANSerialPort = null;
                }
            }
            return connection_success;
        }

        /// <summary>
        /// Close serial port
        /// </summary>
        /// <returns></returns>
        public bool closeAVCLANPort()
        {
            bool disconnecion_success = false;
            if ((null != AVCLANSerialPort) && AVCLANSerialPort.isPortOpen())
            {
                AVCLANSerialPort.ClosePort();
                AVCLANSerialPort = null;
                disconnecion_success = true;
                foreach (var view in AVCLAN_Listeners)
                {
                    view.Value.setCommunicationStatus(CommunicationStatus.DISABLE);
                }
            }
            return disconnecion_success;
        }

        /// <summary>
        /// Main thread
        /// </summary>
        private void threadAVCLANTxTask()
        {
            MessageContainer pending_to_send;
            
            while (ThreadRunning)
            {
                try
                {
                    if (AVCLAN_MessagesToSend.Count() > 0)
                    {
                        pending_to_send = AVCLAN_MessagesToSend.Dequeue();
                        
                        if (AVCLANSerialPort.isPortOpen())
                        {
                            SLIPEncoderObject.encodeSLIPMessage(pending_to_send.MessageObject);

                            if (SLIPEncoderObject.IsEncodeComplete)
                            {
                                AVCLANSerialPort.WriteData(SLIPEncoderObject.getEncodeMessage());
                                /* Notify listeners message */
                                notifyAVCLANListeners(pending_to_send);
                            }
                        }
                    }
                    else
                    {
                        AVCLAN_TxMessageWaitSemaphore.WaitOne();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="callback"></param>
        public void sendBlueBoxMessage(MessageContainer message)
        {
            if (null != message)
            {
                AVCLAN_MessagesToSend.Enqueue(message);
                AVCLAN_TxMessageWaitSemaphore.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void threadAVCLANRxTask()
        {
            MessageContainer message_received;

            while (ThreadRunning)
            {
                if (AVCLAN_ReceivedMessages.Count() > 0)
                {
                    message_received = AVCLAN_ReceivedMessages.Dequeue();
                    notifyAVCLANListeners(message_received);
                }
                else
                {
                    AVCLAN_RxMessageWaitSemaphore.WaitOne();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void AVCLANPortIncomingData(byte[] data)
        {
            int pending_bytes = data.Length;
            int array_position = 0;

            try
            {
                while (pending_bytes > 0)
                {
                    pending_bytes = SLIPDecoderObject.processIncomingMessage(data, array_position, pending_bytes);

                    if (SLIPDecoderObject.IsDecodeComplete)
                    {
                        AVCLAN_ReceivedMessages.Enqueue(new MessageContainer(SLIPDecoderObject.getSlipMessage(), MessageType.RX_FROM_DEVICE));
                        AVCLAN_RxMessageWaitSemaphore.Set();
                    }

                    if (pending_bytes > 0)
                    {
                        array_position = data.Length - pending_bytes;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("AVCLANPortIncomingData error:" + ex.ToString());
            }
        }

        private void notifyAVCLANListeners(MessageContainer message)
        {
            foreach (var view in AVCLAN_Listeners)
            {
                try
                {
                    view.Value.processIncomingMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
