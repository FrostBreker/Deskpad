using System;
using System.Windows.Forms;

namespace DeskPad
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.

        public static MainForm MainForm;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new MainForm();

            Application.Run(MainForm);
        }
    }
}
