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
    public class AccountEntry
    {
        ///<Summary>
        ///Конструктор класса, который принимает на вход пароль, логин (дополнительные поля добавляются по обращению к индексу)
        ///</Summary>
        AccountEntry(string login, string password, string masterKey)
        {
            this.login = login;
            this.password = new Password(password, masterKey);
            password = null;
            masterKey = null;
            Properties = new Dictionary<string, string>();
        }
        
        ///<Summary>
        ///Логин приложения (мб почта или еще что нибудь)
        ///</Summary>
        public string login;

        ///<Summary>
        ///Сам пароль, который внутри уже сам шифруется у себя
        ///</Summary>
        private Password password;

        ///<Summary>
        ///Название аккаунта
        ///</Summary>
        public string accountName;
        ///<Summary>
        ///Словарь хранит в себе динамически расширяемые значения полей
        ///</Summary>
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
    }
}
