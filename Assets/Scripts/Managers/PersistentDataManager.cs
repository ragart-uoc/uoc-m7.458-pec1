using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEC1.Managers
{
    public static class PersistentDataManager
    {
        public static void SaveBestLap(string json)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestLap.json";
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, json);
        }
        
        public static string LoadBestLap()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestLap.json";
            var path = Path.Combine(Application.persistentDataPath, fileName);
            return File.Exists(path) == false ? string.Empty : File.ReadAllText(path);
        }
    }
}
