using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager
{
    public class FileEncryption : IDisposable
    {
        public JObject LoadEncryptedFile(string filePath, byte[] key, int keylen) 
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(key, 0, keylen - 1);
            return DecryptJsonFile(filePath, segment.Array);
        }

        public static void EncryptAndSaveToFile(JObject jsonObj, string masterKey, string filePath)
        {
            // Сериализуем JObject в строку JSON
            string jsonStr = jsonObj.ToString();

            // Создаем новый экземпляр класса AES, используя мастер-ключ в качестве ключа
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(masterKey);
                aes.IV = new byte[16]; // Начальное значение IV должно быть случайным

                // Создаем объект Encryptor для AES, используя ключ и IV
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Создаем поток для записи зашифрованных данных в файл
                using (FileStream fsEncrypt = new FileStream(filePath, FileMode.Create))
                {
                    // Записываем начальное значение IV в файл
                    fsEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    // Создаем поток для шифрования данных и записи в выходной поток
                    using (CryptoStream csEncrypt = new CryptoStream(fsEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Преобразуем строку JSON в байтовый массив и записываем его в выходной поток
                        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
                        csEncrypt.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                }
            }
        }
        public static JObject DecryptJsonFile(string filePath, byte[] masterKey)
        {
            byte[] encryptedData = File.ReadAllBytes(filePath);
            byte[] iv = new byte[8];
            byte[] encryptedContent = new byte[encryptedData.Length - iv.Length];

            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (var aes = Aes.Create())
            {
                aes.Key = masterKey;
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor())
                using (var memoryStream = new MemoryStream(encryptedContent))
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (var streamReader = new StreamReader(cryptoStream))
                {
                    string decryptedJson = streamReader.ReadToEnd();
                    JObject jsonObject = JObject.Parse(decryptedJson);
                    return jsonObject;
                }
            }
        }

        public void Dispose()
        {
            return;
        }
    }
}
