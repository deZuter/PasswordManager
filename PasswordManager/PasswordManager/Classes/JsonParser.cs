using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Classes
{
    public class JsonGroupEntryParser : IDisposable
    {
        public JObject GroupEntryToJObject(GroupEntry groupEntry) 
        {
            if (groupEntry == null) 
            {
                return null;
            }
            string json = JsonConvert.SerializeObject(groupEntry);
            var jObject = JObject.Parse(json);
            return jObject;

        }
        public GroupEntry JObjectToGroupEntry(JObject jObject) 
        {
            if (jObject == null) 
            {
                return null;
            }
            var groupEntry = new GroupEntry(jObject);
            return groupEntry;
        }
        public void Dispose()
        {
            return;
        }

    }
}
