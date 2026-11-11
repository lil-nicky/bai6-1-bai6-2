
using System;
using System.Windows.Forms;
using HRMPayrollSystem.GUI;

namespace HRMPayrollSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
