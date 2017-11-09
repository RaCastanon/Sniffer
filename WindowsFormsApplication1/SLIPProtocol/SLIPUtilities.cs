using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLIPProtocol
{
    public enum SLIP_ESC_CHARS
    {
        SLIP_FLAG = 0xC0,
        SLIP_ESC = 0xDB,
        SLIP_ESC_FLAG = 0xDC,
        SLIP_ESC_ESC = 0xDD
    }

    public class SLIPUtilities
    {
        /* Debugger commands */
        public static int EQTOOL_TARGET_RESET = (1);
        public static int EQTOOL_TARGET_PING = (2);
        public static int EQTOOL_TARGET_MEM_READ = (3);
        public static int EQTOOL_TARGET_MEM_WRITE = (4);
        public static int EQTOOL_TARGET_FLASH_WRITE = (5);
        public static int EQTOOL_TARGET_CYCLES = (6);
        public static int EQTOOL_TARGET_PID_READ = (8);

        /* Debugger command responses */
        public static int EQTOOL_TARGET_RESET_RESPONSE = (1001);
        public static int EQTOOL_TARGET_PING_RESPONSE = (1002);
        public static int EQTOOL_TARGET_MEM_READ_RESPONSE = (1003);
        public static int EQTOOL_TARGET_MEM_WRITE_RESPONSE = (1004);
        public static int EQTOOL_TARGET_FLASH_WRITE_RESPONSE = (1005);
        public static int EQTOOL_TARGET_CYCLES_RESPONSE = (1006);
        public static int EQTOOL_TARGET_PID_READ_RESPONSE = (1008);

        /* PID request responses*/
        public static int EQTOOL_RESPONSE_OK = 0;
        public static int EQTOOL_RESPONSE_FAIL = 1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Integer32ToByteArray(int data)
        {
            byte[] result = new byte[4];

            result[3] = (byte)data;
            result[2] = (byte)(((uint)data >> 8) & 0xFF);
            result[1] = (byte)(((uint)data >> 16) & 0xFF);
            result[0] = (byte)(((uint)data >> 24) & 0xFF);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int ByteArrayToInt32(byte []data, int offset)
        {
            int result = 0;

            result = ((data[offset + 0] << 24) | (data[offset + 1] << 16) | (data[offset + 2] << 8) | (data[offset + 3]));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] encodeMemRead(int address, int size)
        {
            return encodeMemRead(Integer32ToByteArray(address), size);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] encodeMemRead(byte []address, int size)
        {
            byte []mem_read_request = new byte[12];
            byte []mem_read_id = Integer32ToByteArray(EQTOOL_TARGET_MEM_READ);
            byte []mem_read_size = Integer32ToByteArray(size);
            System.Array.Copy(mem_read_id, 0, mem_read_request, 0, 4);
            System.Array.Copy(address, 0, mem_read_request, 4, 4);
            System.Array.Copy(mem_read_size, 0, mem_read_request, 8, 4);

            return mem_read_request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"> Must be size + 8 to be considered as valid - without checksum and headers </param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] decodeMemReadResponse(byte[] message, int size)
        {
            if ((8 + size) != message.Length)
            {
                throw new Exception("Invalid message size while decoding MEM READ!");
            }
            
            byte[] mem_read_response = new byte[4];
            byte[] mem_read__status = new byte[4];
            byte[] mem_read_data = new byte[size];

            System.Array.Copy(message, 0, mem_read_response, 0, 4);
            System.Array.Copy(message, 4, mem_read__status, 0, 4);
            System.Array.Copy(message, 8, mem_read_data, 0, size);

            if ((ByteArrayToInt32(mem_read_response, 0) != EQTOOL_TARGET_MEM_READ_RESPONSE) ||
                (ByteArrayToInt32(mem_read__status, 0) != EQTOOL_RESPONSE_OK))
            {
                mem_read_data = null;
            }

            return mem_read_data;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static byte[] encodePIDRequest(int pid)
        {
            return encodePIDRequest(Integer32ToByteArray(pid));
        }

        /// <summary>
        /// Get PID message
        /// </summary>
        /// <param name="pid">4 bytes PID to request</param>
        /// <returns></returns>
        public static byte[] encodePIDRequest(byte []pid)
        {
            if ((pid != null) && (pid.Length != 4))
            {
                return null;
            }
            byte []pid_request = new byte[8];
            /* PID request ID */
            pid_request[0] = 0x00;
            pid_request[1] = 0x00;
            pid_request[2] = 0x00;
            pid_request[3] = 0x08;

            /* PID to read */
            System.Array.Copy(pid, 0, pid_request, 4, 4);

            return pid_request;
        }

        /// <summary>
        /// Get PID message from 
        /// </summary>
        /// <param name="message">12 bytes message - without checksum and headers </param>
        /// <returns>Data section</returns>
        public static byte[] decodePIDResponse(byte []message)
        {
            if (message.Length != 12)
            {
                throw new Exception("Invalid message size while decoding PID READ!");
            }

            byte[] pid_response = new byte[4];
            byte[] pid_status = new byte[4];
            byte[] pid_data = new byte[4];

            System.Array.Copy(message, 0, pid_response, 0, 4);
            System.Array.Copy(message, 4, pid_status, 0, 4);
            System.Array.Copy(message, 8, pid_data, 0, 4);

            if ((ByteArrayToInt32(pid_response, 0) != EQTOOL_TARGET_PID_READ_RESPONSE) ||
                (ByteArrayToInt32(pid_status, 0) != EQTOOL_RESPONSE_OK))
            {
                pid_data = null;
            }

            return pid_data;
        }

        public static string PadElementsInLines(List<string[]> lines, int padding = 1)
        {
            // Calculate maximum numbers for each element accross all lines
            var numElements = lines[0].Length;
            var maxValues = new int[numElements];
            for (int i = 0; i < numElements; i++)
            {
                maxValues[i] = lines.Max(x => x[i].Length) + padding;
            }

            var sb = new StringBuilder();
            // Build the output
            bool isFirst = true;
            foreach (var line in lines)
            {
                if (!isFirst)
                {
                    sb.AppendLine();
                }
                isFirst = false;

                for (int i = 0; i < line.Length; i++)
                {
                    var value = line[i];
                    // Append the value with padding of the maximum length of any value for this element
                    sb.Append(value.PadRight(maxValues[i]));
                }
            }
            return sb.ToString();
        }
    }

    public class SLIPDecoder
    {
        enum SLIPDecoderStates
        {
            RX_FRAME_HUNT,
            RX_FRAME_START,
            RX_FRAME_NORMAL,
            RX_FRAME_ESC,
            RX_FRAME_END
        }

        private static byte SLIP_FLAG = 0xC0;
        private static byte SLIP_ESC = 0xDB;
        private static byte SLIP_ESC_FLAG = 0xDC;
        private static byte SLIP_ESC_ESC = 0xDD;
        private static int SLIP_MIN_DATA = 1;
        private static int SLIP_MAX_DATA = 300;

        private byte[] SlipReceived = new byte[1024];
        private byte checksum = 0x00;
        private int position = 0;
        private int message_size = 0;
        private SLIPDecoderStates CurrentState = SLIPDecoderStates.RX_FRAME_HUNT;
        private bool DecodeComplete = false;

        /// <summary>
        /// 
        /// </summary>
        public SLIPDecoder()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDecodeComplete
        {
            get { return DecodeComplete; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] getMessage()
        {
            byte[] message = null;
            if (DecodeComplete)
            {
                message = new byte[message_size];
                System.Array.Copy(SlipReceived, 0, message, 0, message_size);
            }

            return message;
        }

        public SLIPMessage getSlipMessage()
        {
            SLIPMessage message;

            message = new SLIPMessage(getMessage());
            /* Validy if getMessage is valid or if it is null */
            return message;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="received_data"></param>
        /// <param name="array_position"></param>
        /// <param name="received_data_size"></param>
        /// <returns></returns>
        public int processIncomingMessage(byte[] received_data, int array_position, int received_data_size)
        {
            int num_bytes = received_data_size;
            byte rx_byte;

            if (received_data_size <= 0)
            {
                return 0;
            }

            DecodeComplete = false;

            while (num_bytes-- > 0)
            {
                rx_byte = received_data[array_position++];

                switch (CurrentState)
                {
                    case SLIPDecoderStates.RX_FRAME_HUNT:
                        if (rx_byte == SLIP_FLAG)
                        {
                            CurrentState = SLIPDecoderStates.RX_FRAME_START;
                            DecodeComplete = false;
                        }
                        break;
                    case SLIPDecoderStates.RX_FRAME_START:
                        switch (rx_byte)
                        {
                            case (byte)SLIP_ESC_CHARS.SLIP_FLAG:
                                break;
                            case (byte)SLIP_ESC_CHARS.SLIP_ESC:
                                position = 0;
                                checksum = 0x00;
                                CurrentState = SLIPDecoderStates.RX_FRAME_ESC;
                                break;
                            default:
                                position = 0;
                                checksum = 0x00;
                                CurrentState = SLIPDecoderStates.RX_FRAME_NORMAL;
                                SlipReceived[position++] = rx_byte;
                                checksum += rx_byte;
                                break;
                        }
                        break;
                    case SLIPDecoderStates.RX_FRAME_NORMAL:
                        switch (rx_byte)
                        {
                            case (byte)SLIP_ESC_CHARS.SLIP_FLAG:
                                CurrentState = SLIPDecoderStates.RX_FRAME_HUNT;

                                if (position < SLIP_MIN_DATA)
                                {
                                    position = 0;
                                    break;
                                }

                                if ((checksum & 0xFF) == 0)
                                {
                                    /* Checksum is correct */
                                    message_size = position - 1;
                                    DecodeComplete = true;
                                    //return num_bytes;
                                }
                                else
                                {
                                    position = 0;
                                    checksum = 0x00;
                                }
                                break;
                            case (byte)SLIP_ESC_CHARS.SLIP_ESC:
                                CurrentState = SLIPDecoderStates.RX_FRAME_ESC;
                                break;
                            default:
                                SlipReceived[position++] = rx_byte;
                                checksum += rx_byte;
                                break;
                        }
                        break;
                    case SLIPDecoderStates.RX_FRAME_ESC:
                        if (rx_byte == SLIP_ESC_ESC)
                        {
                            SlipReceived[position++] = SLIP_ESC;
                            checksum += SLIP_ESC;
                            CurrentState = SLIPDecoderStates.RX_FRAME_NORMAL;
                        }
                        else if (rx_byte == SLIP_ESC_FLAG)
                        {
                            SlipReceived[position++] = SLIP_FLAG;
                            checksum += SLIP_FLAG;
                            CurrentState = SLIPDecoderStates.RX_FRAME_NORMAL;
                        }
                        else
                        {
                            /* This is an error. Begin looking for a new frame. */
                            CurrentState = SLIPDecoderStates.RX_FRAME_HUNT;
                            position = 0;
                            checksum = 0x00;
                        }
                        break;
                    default:
                        CurrentState = SLIPDecoderStates.RX_FRAME_HUNT;
                        position = 0;
                        break;
                }

                /* End while if decode was completed */
                if (DecodeComplete)
                {
                    break;
                }
            }

            if (position >= SLIP_MAX_DATA)
            {
                /*
                ** Buffer full without end of frame!
                ** Discard current frame and begin looking for new frame
                */
                CurrentState = SLIPDecoderStates.RX_FRAME_HUNT;
                position = 0;
                checksum = 0x00;
            }
            return num_bytes;
        }
    }

    public class SLIPEncoder
    {
        private static byte SLIP_FLAG = 0xC0;
        private static byte SLIP_ESC = 0xDB;
        private static byte SLIP_ESC_FLAG = 0xDC;
        private static byte SLIP_ESC_ESC = 0xDD;
        private static int SLIP_MIN_DATA = 1;
        private static int SLIP_MAX_DATA = 300;
        private byte[] MessageResult = new byte[512];
        private bool EncodeSuccess = false;
        private int MessageSize = 0;

        /// <summary>
        /// 
        /// </summary>
        public SLIPEncoder()
        {

        }

        public bool IsEncodeComplete
        {
            get { return EncodeSuccess; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] getEncodeMessage()
        {
            byte []message = null;
            if (EncodeSuccess)
            {
                message = new byte[MessageSize];
                System.Array.Copy(MessageResult, 0, message, 0, MessageSize);
            }
            return message;
        }

        public bool encodeSLIPMessage(SLIPMessage message)
        {
            EncodeSuccess = false;
            MessageSize = 0;

            if (message == null)
            {
                return false;
            }

            byte[] slip_message = message.RawMessage;
            byte message_checksum = message.MessageChecksum;
            int position = 0;
            byte tx_byte;

            MessageResult[position++] = SLIP_FLAG;

            for (int i = 0; i < (slip_message.Length + 1); i++)
            {
                if (position > (SLIP_MAX_DATA - 2))
                {
                    return false;
                }

                if (i == slip_message.Length)
                {
                    tx_byte = message_checksum;
                }
                else
                {
                    tx_byte = slip_message[i];
                }
                
                if (tx_byte == SLIP_FLAG)
                {
                    MessageResult[position++] = SLIP_ESC;
                    tx_byte = SLIP_ESC_FLAG;
                }
                else if (tx_byte == SLIP_ESC)
                {
                    MessageResult[position++] = SLIP_ESC;
                    tx_byte = SLIP_ESC_ESC;
                }
                else
                {
                    // do nothing
                }
                MessageResult[position++] = tx_byte;
            }

            MessageResult[position++] = SLIP_FLAG;
            MessageSize = position;
            EncodeSuccess = true;

            return true;
        }

    }
}
