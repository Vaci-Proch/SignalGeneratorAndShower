using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace SignalShower.SignalGenerator
{
    public class SignalGenerator
    {
        #region Private variables
        private WaveCalculator CalculateFor = WavesCalculator.TriangleWave;

        private double Amplitude = 6.0;
        private double WaveLength = 10000000;
        private DateTime Start = DateTime.MinValue;

        const int PORT_NO = 3000;
        const string SERVER_IP = "127.0.0.1";
        #endregion

        #region Setters
        public void SetWave(WaveCalculator NewWaveCalculator) => CalculateFor = NewWaveCalculator;
        public void SetAmplitude(double amplitude) => Amplitude = amplitude;
        public void SetWaveLength(double waveLength) => WaveLength = waveLength;
        #endregion

        public void ResetTime() => Start = DateTime.UtcNow;
        public double GetSignal()
        {
            if (Start.Equals(DateTime.MinValue)) Start = DateTime.UtcNow;
            return CalculateFor(DateTime.UtcNow.Ticks - Start.Ticks, Amplitude, WaveLength);
        }

        /// <summary>
        /// Wait for reciever to connect and send data to it. If reciever is lost, then wait for another one.
        /// </summary>
        public void GeneratorStart()
        {
            Console.WriteLine("Generator started");
            while (true)
            {
                TransmitData();
            }
        }

        private void TransmitData()
        {
            IPAddress localAdd = IPAddress.Parse(SERVER_IP);
            TcpListener listener = new TcpListener(localAdd, PORT_NO);
            Console.WriteLine("Server start");
            listener.Start();
            Console.WriteLine("Client linked");
            TcpClient client = listener.AcceptTcpClient();
            NetworkStream nwStream = client.GetStream();
            int dataLong = 9;
            byte[] dataToTransmit = new byte[dataLong];
            while (client.Connected)
            {
                var data = GetSignal().ToString();
                // Check if number is not integer and if it is then change it to double.
                if (!data.Contains(','))
                {
                    data += ",0";
                }
                for (int i = 7; i >= 0; i--)
                {
                    // Set data length to 8 (longer or shorter one).
                    dataToTransmit[i] = data.Length > i ? (byte)data[i] : (byte)'0';
                }
                // Add endpoint to data for better parsing from buffer on reciever site.
                dataToTransmit[8] = (byte)';';
                nwStream.Write(dataToTransmit, 0, dataLong);
                Thread.Sleep(10);
            }
            nwStream.Dispose();
        }
    }

    public delegate double WaveCalculator(long time, double amplitude, double waveLength);

    public static class WavesCalculator
    {
        public static WaveCalculator SineWave = (long time, double amplitude, double waveLength) => amplitude * Math.Sin(2 * Math.PI * time / waveLength);
        public static WaveCalculator SquareWave = (long time, double amplitude, double waveLength) => (time % waveLength < (0.5 * waveLength)) ? amplitude : -amplitude;
        public static WaveCalculator TriangleWave = (long time, double amplitude, double waveLength) => 2 * amplitude / Math.PI * Math.Asin(Math.Sin(2 * Math.PI * time / waveLength));
    }

}

