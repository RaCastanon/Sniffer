using SerialPortInterface;
using System;

namespace SLIPProtocol
{

    public class SLIPMessage
    {
        private static byte MESSAGE_HEADER = 0xC0;
        private byte[] SlipMessageData;
        private byte[] RawMessageData;
        private byte MessageChecksumResult;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SLIPMessage(byte[] message)
        {
            /* RAW message */
            RawMessageData = message;
            MessageChecksumResult = two_complement_checksum(message);
            SlipMessageData = getSLIPMessage(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public SLIPMessage(string message)
        {
            /* RAW message */
            RawMessageData = ByteUtilities.HexToByte(message);
            MessageChecksumResult = two_complement_checksum(RawMessageData);
            SlipMessageData = getSLIPMessage(RawMessageData);
        }
        
        public byte[] SlipMessage
        {
            get { return SlipMessageData; }
        }

        public byte[] RawMessage
        {
            get { return RawMessageData; }
        }

        public byte MessageChecksum
        {
            get { return MessageChecksumResult; }
        }

        public SLIPMessage SlipMessageObject
        {
            get { return this; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static byte[] getSLIPMessage(byte[] message)
        {
            byte[] new_message = new byte[message.Length + 3];

            new_message[0] = MESSAGE_HEADER;
            Array.Copy(message, 0, new_message, 1, message.Length);
            new_message[message.Length + 1] = two_complement_checksum(message);
            new_message[message.Length + 2] = MESSAGE_HEADER;
            return new_message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static byte two_complement_checksum(byte[] message)
        {
            byte checksum = 0x00;

            foreach (byte data in message)
            {
                checksum += (byte)((data ^ (byte)0xFF) + (byte)0x01);
            }
            return checksum;
        }

        override
        public string ToString()
        {
            return ByteUtilities.ByteToHex(SlipMessageData);
        }
    }
}
