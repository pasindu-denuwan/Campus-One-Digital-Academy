using System;
using System.Windows.Forms;
using CampusOneDigitalAcademy.Forms;

namespace CampusOneDigitalAcademy
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the Campus One Digital Academy application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WelcomeForm());
        }
    }
}
