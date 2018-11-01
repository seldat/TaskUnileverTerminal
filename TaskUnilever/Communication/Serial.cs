﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SeldatMRMS.Communication
{
    class Serial
    {
        public Serial(String portName)
        {
            _portName = portName;
        }
        public Serial()
        {
        }
        public SerialPort _serialPort = new SerialPort();
        private int _baudRate = 9600;
        private int _dataBits = 8;
        private Handshake _handshake = Handshake.None;
        private Parity _parity = Parity.None;
        private string _portName = "COM1";
        private StopBits _stopBits = StopBits.One;

        public delegate void SerialMessenger(string st);
        public SerialMessenger serialMessenger;
        

        /// <summary> 
        /// Holds data received until we get a terminator. 
        /// </summary> 
        private string tString = string.Empty;
        /// <summary> 
        /// End of transmition byte in this case EOT (ASCII 4). 
        /// </summary> 
        private byte _terminator = 0x4;

        public int BaudRate { get { return _baudRate; } set { _baudRate = value; } }
        public int DataBits { get { return _dataBits; } set { _dataBits = value; } }
        public Handshake Handshake { get { return _handshake; } set { _handshake = value; } }
        public Parity Parity { get { return _parity; } set { _parity = value; } }
        public string PortName { get { return _portName; } set { _portName = value; } }
        public void Open()
        {

            _serialPort.BaudRate = _baudRate;
            _serialPort.DataBits = _dataBits;
            _serialPort.Handshake = _handshake;
            _serialPort.Parity = _parity;
            _serialPort.PortName = _portName;
            _serialPort.StopBits = _stopBits;
            _serialPort.Open();

            //MessageBox.Show("Comport connected :" + _serialPort.IsOpen);
            Task.Run(() => {
                while (_serialPort.IsOpen)
                {
                    try
                    {
                        String st = _serialPort.ReadLine();
                        //Console.WriteLine(st.Split('\r')[0]);
                        ///  byte[] data = Encoding.UTF8.GetBytes(st);
                        serialMessenger(st.Split('\r')[0]);
                    }
                    catch { }
                }

            });

        }
        public void Close()
        {
            if(_serialPort!=null)
            {
                if (_serialPort.IsOpen)
                    _serialPort.Close();
            }
        }
        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //Initialize a buffer to hold the received data 
            byte[] buffer = new byte[5];

            //There is no accurate method for checking how many bytes are read 
            //unless you check the return from the Read method 
            int bytesRead = _serialPort.Read(buffer, 0, 4);
            foreach (byte b in buffer)
            {
                Console.WriteLine(b);
            }
            _serialPort.DiscardInBuffer();
            //serialMessenger(buffer);
            //String str=_serialPort.ReadLine();
            //For the example assume the data we are received is ASCII data. 
            /* tString += Encoding.ASCII.GetString(buffer, 0, bytesRead);
             //Check if string contains the terminator  
             if (tString.IndexOf((char)_terminator) > -1)
             {
                 //If tString does contain terminator we cannot assume that it is the last character received 
                 string workingString = tString.Substring(0, tString.IndexOf((char)_terminator));
                 //Remove the data up to the terminator from tString 
                 tString = tString.Substring(tString.IndexOf((char)_terminator));
                 //Do something with workingString 
                 Console.WriteLine(workingString);
             }*/
        }
    }
}
