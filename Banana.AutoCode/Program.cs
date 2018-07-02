using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Banana.AutoCode
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if ((Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName)).GetUpperBound(0) == 0)
            {
                Application.Run(new MainForm());
            }
        }
    }
}
