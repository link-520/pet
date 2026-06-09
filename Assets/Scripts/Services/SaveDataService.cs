using System;
using System.IO;
using LifeRPG.Data;
using UnityEngine;

namespace LifeRPG.Services
{
    /// <summary>
    /// 本地存档服务。负责把 PlayerData 保存到 Unity 的持久化目录。
    /// </summary>
    public class SaveDataService
    {
        private const string SaveFileName = "player_data.json";

        public string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        public PlayerData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.Log($"未找到玩家存档，将使用默认数据。存档路径：{SavePath}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(SavePath);
                PlayerData data = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"已读取玩家存档：{SavePath}");
                return data;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"读取玩家存档失败，将使用默认数据。路径：{SavePath}\n{exception}");
                return null;
            }
        }

        public void Save(PlayerData playerData)
        {
            if (playerData == null)
            {
                return;
            }

            try
            {
                string directory = Path.GetDirectoryName(SavePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                string json = JsonUtility.ToJson(playerData, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"已保存玩家存档：{SavePath}");
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"保存玩家存档失败。路径：{SavePath}\n{exception}");
            }
        }

        public void DeleteSave()
        {
            if (!File.Exists(SavePath))
            {
                return;
            }

            File.Delete(SavePath);
            Debug.Log($"已删除玩家存档：{SavePath}");
        }
    }
}
