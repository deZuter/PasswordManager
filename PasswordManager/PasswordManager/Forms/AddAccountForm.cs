using PasswordManager.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager.Forms
{
    public partial class AddAccountForm : Form
    {
        public AccountEntry account;
        MasterKey key;

        public AddAccountForm(MasterKey key)
        {
            this.key = key;
            account = null;
            InitializeComponent();
            txtPassword.UseSystemPasswordChar = true;
            // Установка шрифта кнопки, поддерживающего символы Unicode
            btnShowPassword.Font = new Font("Segoe UI Emoji", 8);
            // Установка текста кнопки на символ глаза
            btnShowPassword.Text = "\U0001F441";

        }

        private void btnAddProperty_Click(object sender, EventArgs e)
        {
            // Создание новых элементов TextBox
            TextBox keyTextBox = new TextBox();
            TextBox valueTextBox = new TextBox();

            // Установка положения и размеров TextBox
            keyTextBox.Location = new Point(1, pProperties.Controls.Count * 10); // Расположение по вертикали, учитывая уже имеющиеся элементы
            valueTextBox.Location = new Point(101, pProperties.Controls.Count * 10); // Расположение по вертикали, учитывая уже имеющиеся элементы
            keyTextBox.Size = new Size(100, 20);
            valueTextBox.Size = new Size(190, 20);

            // Добавление TextBox в Panel
            pProperties.Controls.Add(keyTextBox);
            pProperties.Controls.Add(valueTextBox);
        }

        private void btnShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = false;

        }

        private void btnShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.UseSystemPasswordChar = true;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            account = new AccountEntry(txtAccountName.Text, txtLogin.Text, txtPassword.Text, key);
            //groupName = txtGroupName.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
