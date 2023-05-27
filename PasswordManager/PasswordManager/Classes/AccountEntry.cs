using Newtonsoft.Json;
using PasswordManager.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PasswordManager
{
    ///<Summary>
    ///Класс представляет собой запись в листе паролей, содержащая в себе сам пароль, логин и прочие поля (расширяется динамически)
    ///</Summary>
    public class AccountEntry : IDisposable
    {
        ///<Summary>
        ///Конструктор класса, который принимает на вход пароль, логин (дополнительные поля добавляются по обращению к индексу)
        ///</Summary>
        public AccountEntry(string accountName, string login, string password, MasterKey key)
        {
            this.accountName = accountName;
            this.login = login;
            this.password = new Password(password, key);
            password = null;
            this.Properties = new Dictionary<string, string>();
        }

        ///<Summary>
        ///Конструктор класса для Json
        ///</Summary>
        [JsonConstructor]
        public AccountEntry(string accountName, string login, string password)
        {
            this.accountName = accountName;
            this.login = login;
            this.password = JsonConvert.DeserializeObject<Password>(password); ;
            this.Properties = new Dictionary<string, string>();
        }
        ///<Summary>
        ///Название аккаунта
        ///</Summary>
        public string accountName;

        ///<Summary>
        ///Логин приложения (мб почта или еще что нибудь)
        ///</Summary>
        public string login;

        ///<Summary>
        ///Сам пароль, который внутри уже сам шифруется у себя
        ///</Summary>
        [JsonIgnore]
        public Password password;

        ///<Summary>
        ///Необходимо для верного парсинга
        ///</Summary>
        [JsonProperty("Password")]
        private string SerializedPassword
        {
            get { return password.getJsonPassword(); }
        }
        ///<Summary>
        ///Словарь хранит в себе динамически расширяемые значения полей
        ///</Summary>
        [JsonProperty]
        private Dictionary<string, string> Properties { get; set; }

        ///<Summary>
        ///Обращение к полям записи (не получится обратиться к паролю
        ///</Summary>
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

        public void Dispose()
        {
            password.Dispose();
        }
    }
}
