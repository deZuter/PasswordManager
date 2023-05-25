using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Classes
{
    public class MasterKey
    {
        private byte[] _key;
        public int _keyLen;
        private const int blockSize = 16;
        public MasterKey(string key)
        {
            if (key.Length == 0) 
            {
                this._keyLen = 0;
                this._key = null;
                return;
            }
            // Проверяем длину ключа
            if (key.Length % blockSize != 0)
            {
                int paddingSize = blockSize - (key.Length % blockSize);
                _key = new byte[key.Length + paddingSize];

                // Копируем оригинальный ключ в выровненный массив
                Array.Copy(Encoding.UTF8.GetBytes(key), _key, key.Length);
            }
            else
            {
                // Длина ключа уже кратна 16 байтам, не требуется выравнивание
                _key = Encoding.UTF8.GetBytes(key);
            }
            _keyLen = key.Length;
            ProtectedMemory.Protect(_key, MemoryProtectionScope.SameLogon);
        }
        public void unprotectKey()
        {
            ProtectedMemory.Unprotect(_key, MemoryProtectionScope.SameLogon);
        }
        public void protectKey() 
        {
            ProtectedMemory.Protect(_key, MemoryProtectionScope.SameLogon);
        }
        public byte[] getKey() 
        {
            return _key;
        }
    }
}
