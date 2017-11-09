using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialPortInterface
{
    class SerialPortHdlr
    {
        public delegate void ReportIncomingData(byte[] data);
        /// <summary>
        /// Serial Port properties
        /// </summary>
        private string BaudRate = string.Empty;
        private string Parity = string.Empty;
        private string StopBits = string.Empty;
        private string DataBits = string.Empty;
        private string PortName = string.Empty;
        private ReportIncomingData Callback = null;
        private SerialPort ComPort = new SerialPort();

        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="par">Desired Parity</param>
        /// <param name="sBits">Desired StopBits</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="name">Desired PortName</param>
        public SerialPortHdlr(string baud, string parity, string stop_bits, string data_bits, string port_name, ReportIncomingData callback)
        {
            BaudRate = baud;
            Parity = parity;
            StopBits = stop_bits;
            DataBits = data_bits;
            PortName = port_name;
            ComPort.DataReceived += new SerialDataReceivedEventHandler(comPortDataReceived);
            Callback = callback;
        }

        /// <summary>
        /// Get Available ports at PC
        /// </summary>
        /// <returns></returns>
        public static string[] getAvailableSerialPorts()
        {
            string[] available_coms = null;
            available_coms = SerialPort.GetPortNames();
            return available_coms;
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="msg"></param>
        public void WriteData(byte[] msg)
        {
            try
            {
                // send the message to the port
                ComPort.Write(msg, 0, msg.Length);
            }
            catch (FormatException ex)
            {
                // display error message
                /* TODO handle this */
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool OpenPort()
        {
            bool return_value = false;
            try
            {
                // Check if port is already open
                if (ComPort.IsOpen == true)
                {
                    ComPort.Close();
                }

                // Set serial port properties
                ComPort.BaudRate = int.Parse(BaudRate);    //BaudRate
                ComPort.DataBits = int.Parse(DataBits);    //DataBits
                ComPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), StopBits);    //StopBits
                ComPort.Parity = (Parity)Enum.Parse(typeof(Parity), Parity);    //Parity
                ComPort.PortName = PortName;   //PortName
                ComPort.Open();
                // /* TODO notify port is open */
                return_value = true;
            }
            catch (Exception ex)
            {
                /* TODO notify error */
                return_value = false;
            }
            return return_value;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClosePort()
        {
            try
            {
                if (ComPort.IsOpen == true)
                {
                    ComPort.Close();
                }
            }
            catch (Exception ex)
            {
                /* TODO notify error */
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isPortOpen()
        {
            return ComPort.IsOpen;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public void SetPortNameValues(object obj)
        {

            foreach (string str in SerialPort.GetPortNames())
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }

        /// <summary>
        /// Callback method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytes = ComPort.BytesToRead;

            if (bytes > 0)
            {
                byte[] comBuffer = new byte[bytes];
                // read the data and store it
                ComPort.Read(comBuffer, 0, bytes);
                Callback(comBuffer);
            }
        }
    }
}