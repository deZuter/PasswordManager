using Newtonsoft.Json.Linq;
using PasswordManager.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TreeView = System.Windows.Forms.TreeView;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PasswordManager
{
    public partial class MainForm : Form
    {
        byte[] paddedKey;
        int blockSize = 16;
        int keyLen;

        public MainForm()
        {
            string key = null;
            using (LoginForm inputDialog = new LoginForm())
            {
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    // Получаем выбранный узел и добавляем туда новую запись, с именем из диалогового окна
                    key = inputDialog.key;
                }
                else 
                {
                    this.Close();
                }
            }
            // Проверяем длину ключа
            if (key.Length % blockSize != 0)
            {
                int paddingSize = blockSize - (key.Length % blockSize);
                paddedKey = new byte[key.Length + paddingSize];

                // Копируем оригинальный ключ в выровненный массив
                Array.Copy(Encoding.UTF8.GetBytes(key), paddedKey, key.Length);
            }
            else
            {
                // Длина ключа уже кратна 16 байтам, не требуется выравнивание
                paddedKey = Encoding.UTF8.GetBytes(key);
            }
            keyLen = key.Length;
            ProtectedMemory.Protect(paddedKey, MemoryProtectionScope.SameLogon);
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

        #region twGroups
        GroupEntry rootEntry;
        TreeViewManager treeViewManager;
        private void twGroups_init() 
        {
            rootEntry = new GroupEntry("root");
            treeViewManager = new TreeViewManager(twGroups);
            treeViewManager.InsertTreeNode(rootEntry);
        }
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
                    PopulateListView(selectedGroup.GetAccountEntries());
                }
            }
        }
        #endregion

        #region lwAccounts
        public void PopulateListView(List<AccountEntry> accountEntries)
        {
            // Очистка ListView перед добавлением новых данных
            lwAccounts.Items.Clear();

            if (accountEntries == null)
            {
                // Добавление сообщения о пустом списке в ListView
                ListViewItem emptyItem = new ListViewItem("No entries found");
                lwAccounts.Items.Add(emptyItem);
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
                lwAccounts.Items.Add(item);
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {

        }
        #endregion
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;
                JObject json = new JObject();
                using (var fileEncryptor = new FileEncryption()) 
                {
                    json = fileEncryptor.LoadEncryptedFile(selectedFile, paddedKey, keyLen);
                }
                rootEntry = json.ToObject<GroupEntry>();
                treeViewManager.Override(rootEntry);
            }
        }


    }
}
