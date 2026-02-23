using System;
using System.Windows.Forms;
using Clinic_BD.Forms.Auth;

namespace Clinic_BD
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            Application.Run(new LoginForm());
        }
    }
}