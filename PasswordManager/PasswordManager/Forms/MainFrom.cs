using Newtonsoft.Json.Linq;
using PasswordManager.Classes;
using PasswordManager.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PasswordManager
{
    public partial class MainForm : Form
    {
        MasterKey key;
        Config config;
        public MainForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            MainForm_Init();
            config = FileManager.LoadOrCreateConfig();
            if (!ShowLoginForm(config.lastDbName))
            {
                this.Close();
                return;
            }
            try
            {
                var newRoot = FileManager.LoadDb(config.pathWithName, key);
                rootEntry = newRoot;
                treeViewManager.Override(rootEntry);
                listViewManager.setNewGroup(rootEntry);
            }
            catch
            {
                var dialogResult = MessageBox.Show("Ошибка при открытии базы паролей\nЖелаете создать новую базу паролей?",
                    "Ошибка",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.OK)
                {
                    if (!ShowNewDbForm())
                    {
                        this.Close();
                    }
                }
                else this.Close();
            }


        }
        private bool ShowLoginForm(string fileName = null, bool showNewDbButton = true)
        {
            using (LoginForm loginForm = new LoginForm(fileName, showNewDbButton))
            {
                var dialogResult = loginForm.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    this.key = new MasterKey(loginForm.key);
                    config.lastDbName = loginForm.FileName;
                }
                else if (dialogResult == DialogResult.Yes)  //из-за неимением большего используем Yes, а так то результат NewDb
                {
                    if (!ShowNewDbForm())
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
                return true;
            }
        }
        private bool ShowNewDbForm()
        {
            using (NewDbForm newDbForm = new NewDbForm())
            {
                var dialogResult = newDbForm.ShowDialog();
                if (dialogResult != DialogResult.OK)
                {
                    return false;
                }
                this.key = newDbForm.key;
                config.lastDbName = newDbForm.dbName;
                return true;
            }
        }

        private void MainForm_Init()
        {
            tbSearch_init();
            lwAccounts_init();
            twGroups_init();
        }

        #region lwAccounts
        private void lwAccounts_init()
        {
            listViewManager = new ListViewManager(lwAccounts, tslblAccountsCount);
        }

        ListViewManager listViewManager;
        public class ListViewManager
        {
            private ListView _lwAccounts;
            private GroupEntry _groupEntry;
            private ToolStripLabel _lblAccountsCount;
            public ListViewManager(ListView listView, ToolStripLabel lblAccountsCount)
            {
                this._lwAccounts = listView;
                _lwAccounts.Columns.Add("Название", 150);
                _lwAccounts.Columns.Add("Логин", 150);
                _lwAccounts.Columns.Add("Пароль", 50);
                _lwAccounts.View = View.Details;
                _lwAccounts.Scrollable = true;
                _lwAccounts.HeaderStyle = ColumnHeaderStyle.Clickable;
                _lblAccountsCount = lblAccountsCount;
            }
            public void AddAccountEntry(AccountEntry account)
            {
                _groupEntry.addAccountEntry(account);
                PopulateListView();
            }
            public void RemoveSelectedAccountEntry()
            {
                if (_lwAccounts.SelectedItems.Count != 0)
                {

                    var accountEntry = _lwAccounts.SelectedItems[0].Tag as AccountEntry;
                    _groupEntry.removeAccountEntry(accountEntry);
                }
                PopulateListView();
            }
            public void setNewGroup(GroupEntry group)
            {
                this._groupEntry = group;
                PopulateListView();
            }
            public void PopulateListView()
            {
                List<AccountEntry> accountEntries = _groupEntry.GetAccountEntries();
                // Очистка ListView перед добавлением новых данных
                _lwAccounts.Items.Clear();

                if (accountEntries == null)
                {
                    // Добавление сообщения о пустом списке в ListView
                    ListViewItem emptyItem = new ListViewItem("Записей не найдено");
                    _lwAccounts.Items.Add(emptyItem);
                    _lblAccountsCount.Text = "0 aккаунт(ов)";
                    return;
                }
                // Проход по списку AccountEntry и добавление данных в ListView
                foreach (AccountEntry entry in accountEntries)
                {
                    // Создание новой строки для ListView
                    ListViewItem item = new ListViewItem(entry.accountName);

                    // Добавление логина во вторую колонку
                    item.SubItems.Add(entry.login);

                    // Добавление пароля в третью колонку
                    item.SubItems.Add("********");

                    // Установка Tag в качестве ссылки на соответствующий объект AccountEntry
                    item.Tag = entry;

                    // Добавление строки в ListView
                    _lwAccounts.Items.Add(item);
                }
                _lblAccountsCount.Text = _lwAccounts.Items.Count.ToString() + " aккаунт(ов)";
            }
        }

        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            listViewManager.RemoveSelectedAccountEntry();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (AddAccountForm addAccountForm = new AddAccountForm(key))
            {
                if (addAccountForm.ShowDialog() == DialogResult.OK)
                {
                    listViewManager.AddAccountEntry(addAccountForm.account);
                }
            }
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
                listViewManager.PopulateListView();
            }
        }

        private void tbSearch_TextChanged(object sender, EventArgs e)
        {
            if (tbSearch.Text == "")
            {
                return;
            }
            string searchText = tbSearch.Text;
            lwAccounts.Items.Clear(); // Очистка ListView перед добавлением результатов поиска
            SearchNodes(twGroups.Nodes, searchText);
        }

        private void SearchNodes(TreeNodeCollection nodes, string searchText)
        {
            foreach (TreeNode node in nodes)
            {
                var groupEntry = node.Tag as GroupEntry;
                if (groupEntry.accountEntries != null && groupEntry.accountEntries.Count != 0)
                {
                    foreach (AccountEntry entry in groupEntry.accountEntries)
                    {
                        if (entry.accountName.Contains(searchText))
                        {
                            // Создание ListViewItem для отображения AccountEntry в ListView
                            ListViewItem item = new ListViewItem(entry.accountName);
                            item.SubItems.Add(entry.login);
                            item.SubItems.Add("********");
                            item.Tag = entry;
                            // Добавление ListViewItem в ListView
                            lwAccounts.Items.Add(item);
                        }
                    }
                }
                // Рекурсивный вызов функции для поиска в дочерних нодах
                if (node.Nodes.Count != 0) SearchNodes(node.Nodes, searchText);
            }
        }

        #endregion

        #region twGroups
        GroupEntry rootEntry;
        TreeViewManager treeViewManager;

        public class TreeViewManager
        {
            private TreeView _treeView;
            public TreeViewManager(TreeView treeView)
            {
                _treeView = treeView;
            }

            public void AddGroupEntry(GroupEntry groupEntry, TreeNode parentNode = null)
            {
                TreeNode node = new TreeNode(groupEntry.name);
                node.Tag = groupEntry;

                // Добавляем узел в дерево
                if (parentNode == null)
                    _treeView.Nodes.Add(node);
                else
                    parentNode.Nodes.Add(node);

                // Рекурсивно добавляем поддеревья
                if (groupEntry.groupList != null)
                {
                    foreach (var childEntry in groupEntry.groupList)
                    {
                        AddGroupEntry(childEntry, node);
                    }
                }
            }
            public void InsertTreeNode(GroupEntry newGroupEntry, TreeNode parentNode = null)
            {

                // Создаем новый узел и устанавливаем его свойство Tag
                TreeNode newNode = new TreeNode(newGroupEntry.name);
                newNode.Tag = newGroupEntry;

                if (parentNode == null)
                {
                    // Добавляем новый узел в коллекцию узлов родительского узла
                    _treeView.Nodes.Add(newNode);
                }
                else
                {
                    // Добавляем новый GroupEntry в коллекцию Children родительского GroupEntry
                    var parentEntry = parentNode.Tag as GroupEntry;
                    parentEntry.groupList.Add(newGroupEntry);
                    parentNode.Nodes.Add(newNode);
                }
                _treeView.ExpandAll();
            }
            public void RemoveSelectedGroupEntry()
            {
                TreeNode node = _treeView.SelectedNode;
                TreeNode rootNode = _treeView.Nodes[0];
                if (node == null)
                {
                    return;
                }
                if (rootNode == node)
                {
                }
                var entry = node.Tag as GroupEntry;
                entry.Dispose();
                node.Remove();
            }
            public void Override(GroupEntry groupEntry)
            {
                _treeView.Nodes.Clear();
                AddGroupEntry(groupEntry, null);
                _treeView.ExpandAll();
            }
        }
        private void twGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null)
            {
                return;
            }
            TreeNode selectedNode = twGroups.SelectedNode;
            if (selectedNode != null)
            {
                // Получение связанного с узлом объекта GroupEntry
                GroupEntry selectedGroup = selectedNode.Tag as GroupEntry;

                // Проверка, что связанный объект GroupEntry не является null
                if (selectedGroup != null)
                {
                    // Вызов метода PopulateListView с соответствующим списком AccountEntry
                    listViewManager.setNewGroup(selectedGroup);
                }
            }
        }
        private void twGroups_init()
        {
            rootEntry = new GroupEntry("Группы");
            treeViewManager = new TreeViewManager(twGroups);
            treeViewManager.InsertTreeNode(rootEntry);
        }
        private void btnPlus_Click(object sender, EventArgs e)
        {
            using (AddGroupForm inputDialog = new AddGroupForm())
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем выбранный узел и добавляем туда новую запись, с именем из диалогового окна
                    TreeNode selectedNode = twGroups.SelectedNode;
                    treeViewManager.InsertTreeNode(new GroupEntry(inputDialog.groupName),
                        (selectedNode != null) ? selectedNode : null);
                }
            }
        }
        private void btnMinus_Click(object sender, EventArgs e)
        {
            treeViewManager.RemoveSelectedGroupEntry();
        }
        #endregion
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ShowLoginForm(null, false))
            {
                return;
            }
            try
            {
                var newRoot = FileManager.LoadDb(config.pathWithName, key);
                rootEntry = newRoot;
                treeViewManager.Override(rootEntry);
                listViewManager.setNewGroup(rootEntry);
            }
            catch
            {
                MessageBox.Show("Неправильный мастер-ключ.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        private void lwAccounts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lwAccounts.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lwAccounts.SelectedItems[0];
                // Получаем объект из свойства Tag выбранного элемента
                var selectedObject = selectedItem.Tag as AccountEntry;
                if (selectedObject == null) 
                {
                    toolStripStatusLabel.Text = "Невозможно скопировать пароль. Нет записей";
                    return;
                }
                selectedObject.password.CopyPasswordToClipboard(3000);
                toolStripStatusLabel.Text = "Пароль от аккаунта " + selectedObject.accountName + " скопирован в буфер обмена на 3 секунды";
            }
        }
        //обработка горячих клавиш
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                FileManager.SaveDbToFile(rootEntry, config.pathWithName, key);
                toolStripStatusLabel.Text = "Сохранено!";
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            FileManager.SaveDbToFile(rootEntry, config.pathWithName, key);
            toolStripStatusLabel.Text = "Сохранено!";
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            DialogResult dialogResult = MessageBox.Show("Сохранить текущую базу паролей?", "Выход", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                FileManager.SaveConfigFile(config);
                FileManager.SaveDbToFile(rootEntry, config.pathWithName, key);
            }

        }
        private void СохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManager.SaveDbToFile(rootEntry, config.pathWithName, key);
            toolStripStatusLabel.Text = "Сохранено!";
        }
        private void новыйФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ShowNewDbForm()) 
            {
                return;
            }
            rootEntry = new GroupEntry("Группы");
            treeViewManager.Override(rootEntry);
            toolStripStatusLabel.Text = "Создана новая база паролей";
        }
        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutProgramForm form = new AboutProgramForm())
            {
                form.ShowDialog();
            }
        }
    }
}
