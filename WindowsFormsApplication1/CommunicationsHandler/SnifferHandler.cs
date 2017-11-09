using BlueBoxProtocol;
using MessageContainerData;
using SerialPortInterface;
using SLIPProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ViewsHandler;

namespace CommunicationsHandler
{
    class SnifferHandler : ViewInterface
    {
        private Thread ViewHandlerSnifferTxThread = null;
        private bool ThreadRunning = true;
        private SerialPortHdlr SnifferSerialPort = null;
        private Queue<MessageContainer> AVCLAN_MessagesToSend;
        private SLIPDecoder SLIPDecoderObject = new SLIPDecoder();
        private SLIPEncoder SLIPEncoderObject = new SLIPEncoder();
        private SendAVCLANMessage SendAVCLANMessageHandler;
        private CommunicationStatus AVCLANCommunicationStatus;
        private static AutoResetEvent SnifferTxMessageWaitSemaphore = new AutoResetEvent(false);

        /// <summary>
        /// 
        /// </summary>
        public SnifferHandler()
        {
            AVCLANCommunicationStatus = CommunicationStatus.DISABLE;
            AVCLAN_MessagesToSend = new Queue<MessageContainer>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void startSnifferHandler()
        {
            if (null == ViewHandlerSnifferTxThread)
            {
                ViewHandlerSnifferTxThread = new Thread(new ThreadStart(threadSnifferTxTask));
                ViewHandlerSnifferTxThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void stopSnifferHandler()
        {
            if (null != ViewHandlerSnifferTxThread)
            {
                ThreadRunning = false;
                SnifferTxMessageWaitSemaphore.Set();
            }
        }

        /// <summary>
        /// Open Serial Port
        /// </summary>
        /// <param name="com_port"></param>
        /// <returns></returns>
        public bool connectSnifferPort(string com_port)
        {
            bool connection_success = false;
            if (null == SnifferSerialPort)
            {
                SnifferSerialPort = new SerialPortHdlr("57600", "0", "1", "8", com_port,
                                                new SerialPortHdlr.ReportIncomingData(SnifferPortIncomingData));
                if (SnifferSerialPort.OpenPort())
                {
                    connection_success = true;
                }
                else
                {
                    SnifferSerialPort = null;
                }
            }
            return connection_success;
        }

        /// <summary>
        /// Close serial port
        /// </summary>
        /// <returns></returns>
        public bool closeSnifferPort()
        {
            bool disconnecion_success = false;

            if ((null != SnifferSerialPort) && SnifferSerialPort.isPortOpen())
            {
                SnifferSerialPort.ClosePort();
                SnifferSerialPort = null;
                disconnecion_success = true;
            }
            return disconnecion_success;
        }

        /// <summary>
        /// Main thread
        /// </summary>
        private void threadSnifferTxTask()
        {
            MessageContainer pending_to_send;

            while (ThreadRunning)
            {
                try
                {
                    if (AVCLAN_MessagesToSend.Count() > 0)
                    {
                        pending_to_send = AVCLAN_MessagesToSend.Dequeue();

                        if ((SnifferSerialPort != null) && SnifferSerialPort.isPortOpen())
                        {
                            SLIPEncoderObject.encodeSLIPMessage(pending_to_send.MessageObject);
                            if (SLIPEncoderObject.IsEncodeComplete)
                            {
                                SnifferSerialPort.WriteData(SLIPEncoderObject.getEncodeMessage());
                            }
                        }
                    }
                    else
                    {
                        SnifferTxMessageWaitSemaphore.WaitOne();
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
        private void sendAVCLANMessage(MessageContainer message)
        {
            if (null != message)
            {
                AVCLAN_MessagesToSend.Enqueue(message);
                SnifferTxMessageWaitSemaphore.Set();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void SnifferPortIncomingData(byte[] data)
        {
            int pending_bytes = data.Length;
            int array_position = 0;

            try
            {
                while (pending_bytes > 0)
                {
                    pending_bytes = SLIPDecoderObject.processIncomingMessage(data, array_position, pending_bytes);
                    /* check if message has been decoded and if there is a AVCLAN message handler */
                    if ((SLIPDecoderObject.IsDecodeComplete) && (null != SendAVCLANMessageHandler))
                    {
                        /* Enqueue message into AVCLAN handler */
                        SendAVCLANMessageHandler(new MessageContainer(SLIPDecoderObject.getSlipMessage(), MessageType.TX_TO_DEVICE_FROM_SNIFFER));
                    }

                    if (pending_bytes > 0)
                    {
                        array_position = data.Length - pending_bytes;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SnifferPortIncomingData error:" + ex.ToString());
            }
        }

        /************************************************************************************
         * ViewInterface methods implementation
         ************************************************************************************/

        public void processIncomingMessage(MessageContainer message)
        {
            if ((MessageType.TX_TO_DEVICE == message.IsMessageDirection) || 
                (MessageType.TX_TO_DEVICE_FROM_SNIFFER == message.IsMessageDirection))
            {
                /* Do nothing!!! */
            }
            else
            {
                if ((null != message))
                {
                    AVCLAN_MessagesToSend.Enqueue(message);
                    SnifferTxMessageWaitSemaphore.Set();
                }
            }
        }

        public void setCommunicationStatus(CommunicationStatus status)
        {
            AVCLANCommunicationStatus = status;
        }

        public void setViewStatus(ViewStatus status)
        {
            /* DO nothing */
        }

        public void setCommunicationDelegate(SendAVCLANMessage callback)
        {
            SendAVCLANMessageHandler = callback;
        }

        public ListenerType getListenerType()
        {
            return ListenerType.SNIFFER_LISTENER;
        }
    }
}
