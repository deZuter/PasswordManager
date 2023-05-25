using System;
using System.Text;
using System.Security.Cryptography;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Timers;
using Timer = System.Timers.Timer;
using System.Security.Cryptography.Xml;
using PasswordManager.Classes;
using System.Threading;

namespace PasswordManager
{
    public class Password
    {
        byte[] password_masterKey;
        int password_masterKey_len;
        byte[] encryptedPassword;
        int encryptedPassword_len;
        byte[] password_salt;
        int password_salt_len;
        byte[] password_IV;
        int password_IV_len;
        const int blockSize = 16;

        public Password(string password, MasterKey key) 
        {
            setPassword(password, key);
        }
        public Password(JObject jsonPassword) 
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

            var encryptedPasswordToken = jsonPassword["Data"];
            if (encryptedPasswordToken == null || encryptedPasswordToken.Type != JTokenType.String)
            {
                throw new ArgumentException("Invalid encrypted password in encrypted password JSON.");
            }
            encryptedPassword = Convert.FromBase64String(encryptedPasswordToken.Value<string>());
            ProtectPasswordValuesInOperationalMemory();
        }
        private byte[] AlignToBlockSize(byte[] data)
        {
            int paddingSize = blockSize - (data.Length % blockSize);
            byte[] alignedData = new byte[data.Length + paddingSize];
            Array.Copy(data, alignedData, data.Length);
            return alignedData;
        }
        private byte[] RestoreOriginalLength(byte[] data, int originalLength)
        {
            if (data.Length == originalLength)
            {
                return data; // Если длина уже соответствует исходной, возвращаем массив без изменений
            }

            byte[] restoredData = new byte[originalLength];
            Array.Copy(data, restoredData, originalLength);
            return restoredData;
        }
        private void ProtectPasswordValuesInOperationalMemory() 
        {
            encryptedPassword = AlignToBlockSize(encryptedPassword);
            password_salt = AlignToBlockSize(password_salt);
            password_IV = AlignToBlockSize(password_IV);
            password_masterKey = AlignToBlockSize(password_masterKey);
            ProtectedMemory.Protect(encryptedPassword, MemoryProtectionScope.SameLogon);
            ProtectedMemory.Protect(password_salt, MemoryProtectionScope.SameLogon);     
            ProtectedMemory.Protect(password_IV, MemoryProtectionScope.SameLogon);      
            ProtectedMemory.Protect(password_masterKey, MemoryProtectionScope.SameLogon);
        }
        private void UnprotectPasswordValuesInOperationalMemory()
        {
            ProtectedMemory.Unprotect(encryptedPassword, MemoryProtectionScope.SameLogon);    
            ProtectedMemory.Unprotect(password_salt, MemoryProtectionScope.SameLogon);        
            ProtectedMemory.Unprotect(password_IV, MemoryProtectionScope.SameLogon);          
            ProtectedMemory.Unprotect(password_masterKey, MemoryProtectionScope.SameLogon);
            encryptedPassword = RestoreOriginalLength(encryptedPassword, encryptedPassword_len);
            password_salt = RestoreOriginalLength(password_salt, password_salt_len);
            password_IV = RestoreOriginalLength(password_IV, password_IV_len);
            password_masterKey = RestoreOriginalLength(password_masterKey, password_masterKey_len);
        }

        private void setPassword(string password, MasterKey key)
        {
            if (password == null || password.Length == 0 || key == null || key._keyLen == 0)
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
                key.unprotectKey();
                var keyGenerator = new Rfc2898DeriveBytes(key.getKey(), salt, 1000);
                key.protectKey();
                aes.Key = keyGenerator.GetBytes(aes.KeySize / 8);   //Здесь генерируется Key для пароля
                aes.IV = keyGenerator.GetBytes(aes.BlockSize / 8);  //Здесь генерируется IV для пароля

                // Шифрование пароля
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);   
                    encryptedPassword = encryptor.TransformFinalBlock(passwordBytes, 0, passwordBytes.Length);  //тут шифруется сам пароль с помощью сгенерированных значений
                    encryptedPassword_len = encryptedPassword.Length;
                    password_salt = salt;
                    password_salt_len = password_salt.Length;
                    password_IV = aes.IV;
                    password_IV_len = password_IV.Length;
                    password_masterKey = aes.Key;
                    password_masterKey_len = password_masterKey.Length;
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

            using (var aes = new AesManaged())
            {
                aes.KeySize = password_masterKey.Length * 8;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.ISO10126;

                aes.Key = password_masterKey;
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
        private Thread clipboardThread;
        public void CopyPasswordToClipboard(int interval)
        {
            clipboardThread = new Thread(() =>
            {
                Timer clearClipboardTimer = new Timer();
                clearClipboardTimer.Interval = interval;
                UnprotectPasswordValuesInOperationalMemory();
                Clipboard.SetText(getPassword());
                ProtectPasswordValuesInOperationalMemory();
                Thread.Sleep(interval);
                Clipboard.Clear();
                clipboardThread.Abort();
            });
            clipboardThread.SetApartmentState(ApartmentState.STA);
            clipboardThread.Start();

        }
        public JObject getJsonPassword() 
        {
            UnprotectPasswordValuesInOperationalMemory();
            JObject passwordJson = new JObject();
            passwordJson["IV"] = Convert.ToBase64String(password_IV);
            passwordJson["Salt"] = Convert.ToBase64String(password_salt);
            passwordJson["Data"] = Convert.ToBase64String(encryptedPassword);
            return passwordJson;
        }
    }
}
