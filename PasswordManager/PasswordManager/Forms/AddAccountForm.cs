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
        Dictionary<string, string> properties;

        public AddAccountForm(MasterKey key)
        {
            this.key = key;
            account = null;
            this.properties = new Dictionary<string, string>();
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            txtPassword.UseSystemPasswordChar = true;
            // Установка шрифта кнопки, поддерживающего символы Unicode
            btnShowPassword.Font = new Font("Segoe UI Emoji", 8);
            // Установка текста кнопки на символ глаза
            btnShowPassword.Text = "\U0001F441";

        }
        public AddAccountForm(AccountEntry account, MasterKey key)
        {
            this.properties = new Dictionary<string, string>();
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            txtPassword.UseSystemPasswordChar = true;
            // Установка шрифта кнопки, поддерживающего символы Unicode
            btnShowPassword.Font = new Font("Segoe UI Emoji", 8);
            // Установка текста кнопки на символ глаза
            btnShowPassword.Text = "\U0001F441";

            txtAccountName.Text = account.accountName;
            txtLogin.Text = account.login;
            txtPassword.Text = "Измените это поле, чтобы поменять пароль";
            this.account = account;
            this.key = key;
            foreach (var property in account.properties) 
            {
                // Создание новых элементов TextBox
                TextBox keyTextBox = new TextBox();
                TextBox valueTextBox = new TextBox();

                // Установка положения и размеров TextBox
                keyTextBox.Location = new Point(1, pProperties.Controls.Count * 10); // Расположение по вертикали, учитывая уже имеющиеся элементы
                valueTextBox.Location = new Point(101, pProperties.Controls.Count * 10); // Расположение по вертикали, учитывая уже имеющиеся элементы
                keyTextBox.Size = new Size(100, 20);
                valueTextBox.Size = new Size(190, 20);

                keyTextBox.Text = property.Key;
                valueTextBox.Text = property.Value;
                // Добавление TextBox в Panel
                pProperties.Controls.Add(keyTextBox);
                pProperties.Controls.Add(valueTextBox);
            }
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
            if (txtPassword.TextLength == 0 || txtAccountName.TextLength == 0) 
            {
                MessageBox.Show("Поле Пароль и Название аккаунта должны быть заполнены", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < pProperties.Controls.Count; i += 2)
            {
                if (pProperties.Controls[i] is TextBox keyTextBox && pProperties.Controls[i + 1] is TextBox valueTextBox)
                {
                    string key = keyTextBox.Text;
                    string value = valueTextBox.Text;
                    properties.Add(key, value);
                }
            }

            if (account != null)
            {
                if (txtPassword.Text == "Измените это поле, чтобы поменять пароль")
                {
                    account = new AccountEntry(txtAccountName.Text, txtLogin.Text, account.password, properties);
                    DialogResult = DialogResult.OK;
                    return;
                }
            }
            account = new AccountEntry(txtAccountName.Text, txtLogin.Text, txtPassword.Text, key, properties);
            //groupName = txtGroupName.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
