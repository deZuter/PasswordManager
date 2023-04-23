using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PasswordManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tbSearch_init();
            lwAccounts_init();
        }

        #region lwAccounts
        private void lwAccounts_init()
        {
            lwAccounts.Columns.Add("Название", 150);
            lwAccounts.Columns.Add("Логин", 150);
            lwAccounts.Columns.Add("Пароль", 150);
            lwAccounts.View = View.Details;
            lwAccounts.Scrollable = true;
            lwAccounts.HeaderStyle = ColumnHeaderStyle.Clickable;
        }
        #endregion

        #region tbSearch

        const string DEFUALT_SEARCH_TEXT = "Поиск...";
        private void tbSearch_init()
        {
            tbSearch.Text = DEFUALT_SEARCH_TEXT;
            tbSearch.ForeColor = SystemColors.GrayText;
        }

        private void tbSearch_GotFocus(object sender, EventArgs e)
        {
            // Очистить TextBox при получении фокуса
            if (tbSearch.Text == DEFUALT_SEARCH_TEXT)
            {
                tbSearch.Text = "";
                tbSearch.ForeColor = SystemColors.WindowText;
            }
        }
        private void tbSearch_LostFocus(object sender, EventArgs e)
        {
            // Восстановить текст по умолчанию, если TextBox пуст
            if (string.IsNullOrWhiteSpace(tbSearch.Text))
            {
                tbSearch.Text = DEFUALT_SEARCH_TEXT;
                tbSearch.ForeColor = SystemColors.GrayText;
            }
        }

        #endregion
    }
}
