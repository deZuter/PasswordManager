using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class PasswordListEntry
    {
        PasswordListEntry()
        {
            Properties = new Dictionary<string, string>();
        }

        Password password;

        //хранение прочих значений (типа url и пометок)
        public Dictionary<string, string> Properties { get; set; }
        public string this[string key]
        {
            get
            {
                string value;
                Properties.TryGetValue(key, out value);
                return value;
            }
            set
            {
                Properties[key] = value;
            }
        }
    }
}
