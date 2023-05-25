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
        private List<AccountEntry> accountEntries;
        public string name;

        public GroupEntry(string name) 
        {
            this.name = name;
            this.groupList = new List<GroupEntry>();
            this.accountEntries = new List<AccountEntry>();
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
