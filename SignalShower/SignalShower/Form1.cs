using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SignalShower
{
    public partial class Form1 : Form
    {
        #region Class variables
        const int PORT_NO = 3000;
        const string SERVER_IP = "127.0.0.1";
        TcpClient Client;
        NetworkStream NwStream;

        Pen Pen = new Pen(Color.Black, 3);
        Graphics FormGraphics;

        List<double> ValuesToPrint;

        int MaxNumberOfValues = 300;
        double StepSize;
        double MaxHeight = 0;
        Point LeftUpCorner;
        Point RightDownCorner;

        SignalGenerator.SignalGenerator Generator;
        bool ConnectedToDataPort = false;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }
        public Form1(SignalGenerator.SignalGenerator generator)
        {
            Generator = generator;
            InitializeComponent();
            ConnectedToDataPort = StartDataReader();
            timer1.Start();
            FormGraphics = CreateGraphics();
            LeftUpCorner = new Point(this.Size.Width / 21, this.Size.Width / 21);
            RightDownCorner = new Point(this.Size.Width / 21 * 19, this.Size.Height - (this.Size.Width / 21));
            ValuesToPrint = new List<double>();
            StepSize = (double)(RightDownCorner.X - LeftUpCorner.X) / MaxNumberOfValues;
        }

        public void Tick()
        {
            if (ConnectedToDataPort)
            {
                ReadAndStoreData();
                UpdateGraphics();
            }
            else
            {
                ConnectedToDataPort = StartDataReader();
            }
        }


        private bool StartDataReader()
        {
            try
            {
                Client = new TcpClient(SERVER_IP, PORT_NO);
                NwStream = Client.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private void ReadAndStoreData()
        {
            byte[] bytesToRead = new byte[Client.ReceiveBufferSize];
            NwStream.Read(bytesToRead, 0, Client.ReceiveBufferSize);
            string[] result = ParseInputData(bytesToRead);
            foreach (var item in result)
            {
                double converted;
                var numberIsCorrect = double.TryParse(item, out converted);
                if (double.TryParse(item, out converted))
                {
                    ValuesToPrint.Insert(0, converted);
                    MaxHeight = Math.Max(MaxHeight, Math.Abs(converted));
                }
                else
                {
                    Console.WriteLine(item);
                }
            }
            // Remove values that are no longer needed.
            if (ValuesToPrint.Count > MaxNumberOfValues)
            {
                ValuesToPrint.RemoveRange(MaxNumberOfValues - 1, ValuesToPrint.Count - MaxNumberOfValues + 1);
            }
        }

        /// <summary>
        /// Split data by ';', that is used by sender to separate values
        /// </summary>
        /// <returns>returns all string values that are not longer then 8 chars</returns>
        private string[] ParseInputData(byte[] bytesReaded)
        {
            var result = new List<string>();
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytesReaded.Length; i++)
            {
                builder.Append((char)bytesReaded[i]);
            }
            var inputData = builder.ToString().Split(';');
            foreach (var item in inputData)
            {
                if (item.Length <= 8)
                {
                    result.Add(item);
                }
            }
            return result.ToArray();
        }

        private void UpdateGraphics()
        {
            Point[] pointsToDraw = new Point[ValuesToPrint.Count];
            int MiddleY = (RightDownCorner.Y + LeftUpCorner.Y) / 2;
            for (int i = 0; i < ValuesToPrint.Count; i++)
            {
                pointsToDraw[i] = new Point((int)(StepSize * i + LeftUpCorner.X), MiddleY + (int)((MiddleY - LeftUpCorner.Y) * (ValuesToPrint[i] / MaxHeight)));
            }
            FormGraphics.Clear(this.BackColor);
            Font font = new Font("Arial", 16);
            Brush brush = new SolidBrush(Color.Black);
            FormGraphics.DrawString(MaxHeight.ToString(), font, brush, RightDownCorner.X, LeftUpCorner.Y);
            FormGraphics.DrawString("0", font, brush, RightDownCorner.X, MiddleY - font.Height / 2);
            FormGraphics.DrawLine(Pen, LeftUpCorner.X, MiddleY, RightDownCorner.X, MiddleY);
            if (pointsToDraw.Length > 1)
                FormGraphics.DrawCurve(Pen, pointsToDraw);
        }
        private void ClearData()
        {
            MaxHeight = 0;
            ValuesToPrint = new List<double>();
        }


        #region Form functions
        private void timer1_Tick(object sender, EventArgs e)
        {
            Tick();
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Generator.SetWave(SignalGenerator.WavesCalculator.SineWave);
            ClearData();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Generator.SetWave(SignalGenerator.WavesCalculator.SquareWave);
            ClearData();

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            Generator.SetWave(SignalGenerator.WavesCalculator.TriangleWave);
            ClearData();

        }
        #endregion
    }
}
