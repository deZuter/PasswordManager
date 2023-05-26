using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PasswordManager
{
    public class GroupEntry
    {
        public List<GroupEntry> groupList;

        public List<AccountEntry> accountEntries;

        public string name;
        private GroupEntry() 
        { }
        public GroupEntry(string name) 
        {
            this.name = name;
            this.groupList = new List<GroupEntry>();
            this.accountEntries = new List<AccountEntry>();
        }

        public GroupEntry(JObject json) 
        {
            try 
            {
                GroupEntry groupEntry = new GroupEntry();
                groupEntry = JsonConvert.DeserializeObject<GroupEntry>(json.ToString());
                this.groupList = groupEntry.groupList;
                this.accountEntries = groupEntry.accountEntries;
                this.name = groupEntry.name;
            }
            catch
            {
                MessageBox.Show("Ошибка при десереализации JSON-a Возможно, файл поврежден", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void addAccountEntry(AccountEntry accountEntry) 
        {
            accountEntries.Add(accountEntry);
        }

        public List<AccountEntry> GetAccountEntries() 
        {
            if (accountEntries.Count != 0)
                return accountEntries;
            else return null;
        }

        public string getName()
        {
            return name;
        }
    }
}
