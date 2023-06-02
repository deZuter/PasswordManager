using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PasswordManager.Classes
{
    public class FileManager
    {
        private const string configFilePath = "passwdConfig.cfg";
        public static GroupEntry LoadDb(string filePath, MasterKey key)
        {
            var jObject = FileEncryptor.DecryptJsonFile(filePath, key);

            var groupEntry = new JsonGroupEntryParser().JObjectToGroupEntry(jObject);
            return groupEntry;
        }
        public static void SaveDbToFile(GroupEntry groupEntry, string filePath, MasterKey key)
        {
            var json = new JsonGroupEntryParser().GroupEntryToJObject(groupEntry);
            FileEncryptor.EncryptAndSaveToFile(json, key, filePath);
        }
        public static Config LoadOrCreateConfig()
        {
            if (File.Exists(configFilePath))
            {
                // Файл конфигурации уже существует, считываем его содержимое
                string configFileContent = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Config>(configFileContent);
            }
            else
            {
                string selectedPath = null;
                // Создание экземпляра диалогового окна выбора пути до директории
                using (var folderDialog = new FolderBrowserDialog())
                {
                    // Установка заголовка окна
                    folderDialog.Description = "Выберите директорию для сохранения баз данных";

                    // Отображение диалогового окна и проверка результата
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Получение выбранного пути до директории
                        selectedPath = folderDialog.SelectedPath;

                    }
                }
                Config config = new Config();
                config.databaseDirectory = selectedPath;
                string configJson = JsonConvert.SerializeObject(config);
                File.WriteAllText(configFilePath, configJson);
                return config;
            }
        }
        public static void SaveConfigFile(Config config)
        {
            string configJson = JsonConvert.SerializeObject(config);
            File.WriteAllText(configFilePath, configJson);
        }
    }
}

