using System;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using Newtonsoft.Json;

using System.Timers;
using Timer = System.Timers.Timer;
using Newtonsoft.Json.Linq;

namespace PasswordManager
{
    public class Password
    {
        byte[] password_masterKey;  //Единственное, что изначально необходимо для создания пароля.

        byte[] encryptedPassword;
        byte[] password_salt;
        byte[] password_IV;

        Password(string password, string masterKey) 
        {
            setPassword(password, masterKey);
        }

        Password(JObject jsonPassword) 
        {
            if (jsonPassword == null)
            {
                throw new ArgumentNullException(nameof(jsonPassword));
            }
            var ivToken = jsonPassword["IV"];
            if (ivToken == null || ivToken.Type != JTokenType.String)
            {
                throw new ArgumentException("Invalid IV in encrypted password JSON.");
            }
            password_IV = Convert.FromBase64String(ivToken.Value<string>());

            var saltToken = jsonPassword["Salt"];
            if (saltToken == null || saltToken.Type != JTokenType.String)
            {
                throw new ArgumentException("Invalid salt in encrypted password JSON.");
            }
            password_salt = Convert.FromBase64String(saltToken.Value<string>());

            var encryptedPasswordToken = jsonPassword["EncryptedPassword"];
            if (encryptedPasswordToken == null || encryptedPasswordToken.Type != JTokenType.String)
            {
                throw new ArgumentException("Invalid encrypted password in encrypted password JSON.");
            }
            encryptedPassword = Convert.FromBase64String(encryptedPasswordToken.Value<string>());
            ProtectPasswordValuesInOperationalMemory();
        }

        private void ProtectPasswordValuesInOperationalMemory() 
        {
            ProtectedMemory.Protect(encryptedPassword, MemoryProtectionScope.SameLogon);    //вся информация о пароле, находящаяся в оперативной памяти
            ProtectedMemory.Protect(password_salt, MemoryProtectionScope.SameLogon);        //находится в относительной безопасности благодаря технологии
            ProtectedMemory.Protect(password_IV, MemoryProtectionScope.SameLogon);          //Microsoft ProtectedMemory
            ProtectedMemory.Protect(password_masterKey, MemoryProtectionScope.SameLogon);

        }
        private void UnprotectPasswordValuesInOperationalMemory()
        {
            ProtectedMemory.Unprotect(encryptedPassword, MemoryProtectionScope.SameLogon);    
            ProtectedMemory.Unprotect(password_salt, MemoryProtectionScope.SameLogon);        
            ProtectedMemory.Unprotect(password_IV, MemoryProtectionScope.SameLogon);          
            ProtectedMemory.Unprotect(password_masterKey, MemoryProtectionScope.SameLogon);
        }

        public void setPassword(string password, string masterKey)
        {
            if (password == null || password.Length == 0 || masterKey == null || masterKey.Length == 0)
            {
                throw new ArgumentNullException();
            }
            using (var aes = new AesManaged())
            {
                byte[] salt = new byte[8];

                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(salt);                                 //Здесь генерируется соль для пароля
                }

                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                var keyGenerator = new Rfc2898DeriveBytes(masterKey, salt, 150);
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);   //Здесь генерируется Key для пароля
                aes.IV = keyGenerator.GetBytes(aes.BlockSize / 8);  //Здесь генерируется IV для пароля

                // Шифрование пароля
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);   
                    encryptedPassword = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);  //тут шифруется сам пароль с помощью сгенерированных значений
                    password_salt = salt;
                    password_IV = aes.IV;
                    password_masterKey = Encoding.UTF8.GetBytes(masterKey);
                    ProtectPasswordValuesInOperationalMemory();
                    Array.Clear(aes.Key, 0, aes.Key.Length);
                    Array.Clear(aes.IV, 0, aes.IV.Length);
                    Array.Clear(salt, 0, salt.Length);
                }
            }
        }
        private string getPassword()
        {
            if (encryptedPassword == null || encryptedPassword.Length == 0)//проверка, если что-то из этого нулёвое, то расшифровки не получится
                throw new ArgumentNullException(nameof(encryptedPassword));
            if (password_masterKey == null || password_masterKey.Length == 0)
                throw new ArgumentNullException(nameof(password_masterKey));
            if (password_salt == null || password_salt.Length == 0)
                throw new ArgumentNullException(nameof(password_salt));
            if (password_IV == null || password_IV.Length == 0)
                throw new ArgumentNullException(nameof(password_IV));

            UnprotectPasswordValuesInOperationalMemory();

            using (var aes = new AesManaged())
            {
                aes.KeySize = password_masterKey.Length * 8;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                var keyGenerator = new Rfc2898DeriveBytes(password_masterKey, password_salt, 150);
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);
                aes.IV = password_IV;

                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedPassword, 0, encryptedPassword.Length);
                    aes.Clear();

                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

        // Копируем пароль в буфер обмена и запускаем таймер
        public void CopyPasswordToClipboard(Timer clearClipboardTimer)
        {
            Clipboard.SetText(getPassword());
            clearClipboardTimer.Elapsed += ClearClipboardTimer_Elapsed;
            clearClipboardTimer.AutoReset = false;
            clearClipboardTimer.Start();
        }

        // Очищаем буфер обмена после истечения времени таймера
        private void ClearClipboardTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Clipboard.Clear();
        }

    }
}
