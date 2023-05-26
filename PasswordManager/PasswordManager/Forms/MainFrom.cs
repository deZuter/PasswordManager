using Newtonsoft.Json.Linq;
using PasswordManager.Classes;
using PasswordManager.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace PasswordManager
{
    public partial class MainForm : Form
    {
        MasterKey key;
        Config config;
        public MainForm()
        {
            using (LoginForm inputDialog = new LoginForm())
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем выбранный узел и добавляем туда новую запись, с именем из диалогового окна
                    this.key = new MasterKey(inputDialog.key);
                }
                else 
                {
                    this.Close();
                }
            }
            config = FileManager.LoadOrCreateConfig();
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
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
                _lwAccounts.Columns.Add("Пароль", 150);
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
            public void setNewGroup(GroupEntry group)
            {
                this._groupEntry = group;
                PopulateListView();
            }
            private void PopulateListView()
            {
                List<AccountEntry> accountEntries = _groupEntry.GetAccountEntries();
                // Очистка ListView перед добавлением новых данных
                _lwAccounts.Items.Clear();

                if (accountEntries == null)
                {
                    // Добавление сообщения о пустом списке в ListView
                    ListViewItem emptyItem = new ListViewItem("No entries found");
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
                _lblAccountsCount.Text =_lwAccounts.Items.Count.ToString() + " aккаунт(ов)";
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
            }
            public void Override(GroupEntry groupEntry)
            {
                _treeView.Nodes.Clear();
                AddGroupEntry(groupEntry, null);
            }
            public TreeNode getSelectedNode() 
            {
                return _treeView.SelectedNode;
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
            rootEntry = new GroupEntry("root");
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
        #endregion
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

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                rootEntry = FileManager.LoadEncryptedFile(selectedFile, key);
                if (rootEntry == null) 
                {
                    return;
                }
                treeViewManager.Override(rootEntry);
                listViewManager.setNewGroup(rootEntry);
            }
        }

        private void lwAccounts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lwAccounts.SelectedItems.Count > 0)
            {
                    ListViewItem selectedItem = lwAccounts.SelectedItems[0];
                    // Получаем объект из свойства Tag выбранного элемента
                    var selectedObject = selectedItem.Tag as AccountEntry;
                    selectedObject.password.CopyPasswordToClipboard(3000);
            }
        }


        //обработка горячих клавиш
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)       // Ctrl-S Save
            {
                FileManager.SaveDbToFile(rootEntry, config.DatabaseDirectory + "\\root", key);
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            FileManager.SaveDbToFile(rootEntry, config.DatabaseDirectory + "\\root", key);
        }
    }
}
