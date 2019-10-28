using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SignalShower
{
    static class Program
    {
        /// <summary>
        /// Hlavní vstupní bod aplikace.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SignalGenerator.SignalGenerator signalGenerator = new SignalGenerator.SignalGenerator();
            Thread Generator = new Thread(() => signalGenerator.GeneratorStart());
            Generator.Start();
            var form = new Form1(signalGenerator);
            Application.Run(form);
            Generator.Abort();
        }
    }
}
