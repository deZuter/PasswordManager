using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Classes
{
    public class Config
    {
        public string databaseDirectory { get; set; }
        public string lastDbName { get; set; }
        public string pathWithName 
        {
            get 
            {
                return databaseDirectory + "\\" + lastDbName;
            }
        }
    }
}
