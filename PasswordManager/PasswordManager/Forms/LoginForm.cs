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
    public partial class LoginForm : Form
    {
        public string key;
        private string _fileName;
        public string FileName {
            get 
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                txtFileName.Text = value;
            }
        }
        public LoginForm(string fileName = null, bool showNewDbButton = true)
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            if (showNewDbButton == false)
            {
                btnNewDB.Enabled = false;
            }
            if (fileName != null)
            {
                this.FileName = fileName;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtKey.TextLength == 0 || txtFileName.TextLength == 0) 
            {
                return;
            }
            this.key = txtKey.Text;
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnNewDB_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            // Создание экземпляра диалогового окна выбора пути до директории
            using (var fileDialog = new OpenFileDialog())
            {
                DialogResult result = fileDialog.ShowDialog();
                // Отображение диалогового окна и проверка результата
                if (result == DialogResult.OK)
                {
                    // Получение выбранного пути до директории
                    this.FileName = fileDialog.SafeFileName.ToString();
                }
            }
        }
    }
}
