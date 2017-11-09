using SerialPortInterface;
using SLIPProtocol;
using System;

namespace BlueBoxProtocol
{

    public class MessageTX_FRAME_CMD
    {
        public byte[] SrcAddress = new byte[2];
        public byte[] DestAddress = new byte[2];
        public byte[] Data;

        private MessageTX_FRAME_CMD()
        {

        }

        public byte[] GetSrcAddress
        {
            get { return SrcAddress; }
        }
        
        public byte[] GetDestAddress
        {
            get { return DestAddress; }
        }

        public byte[] GetData
        {
            get { return Data; }
        }
        
        public static MessageTX_FRAME_CMD getObject(BlueBoxMessage message, byte[] src_address)
        {
            MessageTX_FRAME_CMD result_object = null;

            if (message != null)
            {
                if (BlueBoxMessage.TX_FRAME_CMD == message.MessageType)
                {
                    result_object = new MessageTX_FRAME_CMD();
                    result_object.decodeMessage(message);
                    result_object.SrcAddress = src_address;
                }
            }
            return result_object;

        }
        
        private void decodeMessage(BlueBoxMessage message)
        {
            /* Get source - desitnation address */
            DestAddress[0] = message.MessageData[0];
            DestAddress[1] = message.MessageData[1];
            /* Get data */
            Data = new byte[message.MessageLength - 2];
            Array.Copy(message.MessageData, 2, Data, 0, message.MessageLength - 2);
        }

        override
        public string ToString()
        {
            string result;

            if (SrcAddress == null)
            {
                result = "Src XX XX ";
            }
            else
            {
                result = "Src " + ByteUtilities.ByteToHex(SrcAddress);
            }
            result += " - Dst " + ByteUtilities.ByteToHex(DestAddress);
            result += " - Data " + ByteUtilities.ByteToHex(Data);
            return result;
        }
    }

    public class MessageRX_FRAME_IND
    {
        public byte[] SrcAddress = new byte[2];
        public byte[] DestAddress = new byte[2];
        public byte[] Data;

        private MessageRX_FRAME_IND()
        {

        }

        public byte[] GetSrcAddress
        {
            get { return SrcAddress; }
        }

        public byte[] GetDestAddress
        {
            get { return DestAddress; }
        }

        public byte[] GetData
        {
            get { return Data; }
        }

        public static MessageRX_FRAME_IND getObject(BlueBoxMessage message, byte[] dest_address)
        {
            MessageRX_FRAME_IND result_object = null;

            if (message != null)
            {
                if (BlueBoxMessage.RX_FRAME_IND == message.MessageType)
                {
                    result_object = new MessageRX_FRAME_IND();
                    result_object.decodeMessage(message);
                    result_object.DestAddress = dest_address;
                }
            }
            return result_object;

        }

        private void decodeMessage(BlueBoxMessage message)
        {
            /* Get source - desitnation address */
            SrcAddress[0] = message.MessageData[0];
            SrcAddress[1] = message.MessageData[1];
            /* Get data */
            Data = new byte[message.MessageLength - 2];
            Array.Copy(message.MessageData, 2, Data, 0, message.MessageLength - 2);
        }

        override
        public string ToString()
        {
            string result;

            result = "Src " + ByteUtilities.ByteToHex(SrcAddress);
            if (DestAddress == null)
            {
                result += " - Dst XX XX ";
            }
            else
            {
                result += " - Dst " + ByteUtilities.ByteToHex(DestAddress);
            }
            result += " - Data " + ByteUtilities.ByteToHex(Data);
            return result;
        }
    }

    public class MessageINTF_CNTRL_CMD
    {
        public byte []Address = new byte[2];

        private MessageINTF_CNTRL_CMD()
        {

        }

        public byte[] GetAddress
        {
            get { return Address; }
        }

        public static MessageINTF_CNTRL_CMD getObject(BlueBoxMessage  message)
        {
            MessageINTF_CNTRL_CMD result_object = null;

            if (message != null)
            {
                if (BlueBoxMessage.INTF_CNTRL_CMD == message.MessageType)
                {
                    result_object = new MessageINTF_CNTRL_CMD();
                    result_object.decodeMessage(message);
                }
            }
            return result_object;

        }

        private void decodeMessage(BlueBoxMessage message)
        {
            Address[0] = message.MessageData[1];
            Address[1] = message.MessageData[2];
        }

        override
        public string ToString()
        {
            return ByteUtilities.ByteToHex(Address);
        }
    }

    public class BlueBoxMessage
    {
        /**************************************************************
         * Message structure 
         * Header  AVC_INTF  Command  Length  Data  Checksum   Header
         * C0      01        XX       00 01   01    7C         C0 
         **************************************************************/

        /* AVC INTF */
        public static byte AVC_INTF = 0x01;         /* AVC_INTF 0x01 */
        /* Command TYPE*/
        public static byte TX_FRAME_CMD = 0x01;     /* Command 0x01 TX_FRAME_CMD */
        public static byte GET_STATUS_CMD = 0x02;   /* Command 0x02 GET_STATUS_CMD */
        public static byte INTF_CNTRL_CMD = 0x03;   /* Command 0x03 INTF_CNTRL_CMD */
        public static byte TX_FRAME_RSP = 0x81;     /* Command 0x81 TX_FRAME_RSP */
        public static byte GET_STATUS_RSP = 0x82;   /* Command 0x82 GET_STATUS_RSP */
        public static byte INTF_CNTRL_RSP = 0x83;   /* Command 0x83 INTF_CNTRL_RSP */
        public static byte RX_FRAME_IND = 0xF1;     /* Command 0xF1 RX_FRAME_IND */
        public static byte UNDEFINED = 0xFF;        /* Command 0xFF UNDEFINED */

        /* Decoding constants */
        private static int SLIPToAVCLAN_MinSize = 8;
        private static int SLIPToAVCLAN_Interface = 1;
        private static int SLIPToAVCLAN_Command = 2;
        private static int SLIPToAVCLAN_Length1 = 3;
        private static int SLIPToAVCLAN_Length2 = 4;
        private static int SLIPToAVCLAN_Data = 5;

        /* Class variables */
        private SLIPMessage MessageSLIPObject;
        private DateTime MessageTimeStamp;
        private string MessageTimeStampString;
        public byte MessageType = UNDEFINED;
        public int MessageLength = 0;
        public byte[] MessageData = new byte[0];
        public byte[] MessageDataAddress = new byte[2];
        
        public BlueBoxMessage(SLIPMessage message_object)
        {
            /* Message type*/
            MessageSLIPObject = message_object;
            /* Message timestamp */
            MessageTimeStamp = DateTime.Now;
            MessageTimeStampString = MessageTimeStamp.ToString("hh:mm:ss.fff");
            /* Bluebox Message decoding */
            decodeSLIPMessage();
        }

        public string TimeStampString
        {
            get { return MessageTimeStampString; }
        }

        public SLIPMessage SlipMessage
        {
            get { return MessageSLIPObject; }
        }

        override
        public string ToString()
        {
            return TimeStampString + " - " + ByteUtilities.ByteToHex(MessageData);
        }

        private void decodeSLIPMessage()
        {
            if (MessageSLIPObject.SlipMessage.Length >= SLIPToAVCLAN_MinSize)
            {
                if (AVC_INTF == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Interface])
                {
                    /* Current message type */
                    MessageType = MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command];

                    /* Message Length */
                    MessageLength = (MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Length1] << 8) |
                                                  (MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Length2]);

                    /* RX - TX messages related to AVCLAN */
                    if ((TX_FRAME_CMD == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command]) ||
                        (RX_FRAME_IND == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command]))
                    {
                        if (RX_FRAME_IND == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command])
                        {
                            MessageLength -= 4;
                        }
                    }
                    else if ((INTF_CNTRL_CMD == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command]) ||
                             (TX_FRAME_RSP == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command]) ||
                             (INTF_CNTRL_RSP == MessageSLIPObject.SlipMessage[SLIPToAVCLAN_Command]))
                    {
                        /* Process */
                    }
                    else
                    {
                        /* Do nothing!!! - Process GET_STATUS_CMD - GET_STATUS_RSP */
                    }

                    /* Get Message data */
                    if (MessageLength > 0)
                    {
                        MessageData = new byte[MessageLength];
                        Array.Copy(MessageSLIPObject.SlipMessage, SLIPToAVCLAN_Data,
                                   MessageData, 0, MessageLength);
                    }

                }
            }
        }
        
    }
}
