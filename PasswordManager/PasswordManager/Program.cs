using PasswordManager.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла какая-то ошибка. Свяжитесь с разработчиком dvlob@yandex.ru и напишите: " + ex.Message, "Ой!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
