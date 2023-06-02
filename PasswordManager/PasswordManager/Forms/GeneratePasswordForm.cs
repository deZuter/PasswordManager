using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager.Forms
{
    public partial class GeneratePasswordForm : Form
    {

        public GeneratePasswordForm()
        {
            InitializeComponent();
        }
        public class PasswordGenerator
        {
            private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
            private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            private const string NumericChars = "0123456789";
            private const string SpecialChars = "!@#$%^&*()-=_+[]{}|;:,.<>?";
            public static string GeneratePassword(int length, bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial)
            {
                string charSet = "";

                if (includeLowercase)
                    charSet += LowercaseChars;

                if (includeUppercase)
                    charSet += UppercaseChars;

                if (includeNumeric)
                    charSet += NumericChars;

                if (includeSpecial)
                    charSet += SpecialChars;

                StringBuilder password = new StringBuilder();
                Random random = new Random();

                for (int i = 0; i < length; i++)
                {
                    int randomIndex = random.Next(0, charSet.Length);
                    password.Append(charSet[randomIndex]);
                }

                return password.ToString();
            }
        }
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            int generatedPasswordLen = 0;
            try
            {
                generatedPasswordLen = int.Parse(tbLength.Text);
            }
            catch
            {
                MessageBox.Show("Вы написали в поле \"Длина\" что-то, кроме цифр", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (generatedPasswordLen < 0 || generatedPasswordLen > 128) 
            {
                MessageBox.Show("Не стоит генерировать длину, больше 128", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tbPassword.Text = PasswordGenerator.GeneratePassword(generatedPasswordLen, cbLowerCase.Checked, cbUpperCase.Checked, cbNumerals.Checked, cbSymblos.Checked);           
        }
    }
}
