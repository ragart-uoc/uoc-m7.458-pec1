using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>PersistentDataManager</c> manages the persistent data.
    /// </summary>
    public static class PersistentDataManager
    {
        /// <summary>
        /// Method <c>SaveBestRace</c> saves the best race.
        /// </summary>
        /// <param name="json">The JSON string of the best race.</param>
        /// <param name="lapNumber">The number of laps of the best race.</param>
        public static void SaveBestRace(string json, int lapNumber)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestRace_{lapNumber}laps.json";
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, json);
        }
        
        /// <summary>
        /// Method <c>LoadBestRace</c> loads the best race.
        /// </summary>
        /// <param name="lapNumber">The number of laps of the best race.</param>
        /// <returns>The JSON string of the best race.</returns>
        public static string LoadBestRace(int lapNumber)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestRace_{lapNumber}laps";
            var path = Path.Combine(Application.persistentDataPath, fileName + ".json");
            if (File.Exists(path))
                return File.ReadAllText(path);
            // If file doesn't exist, try with Unity resources
            path = $"SavedData/{fileName}";
            var textAsset = Resources.Load<TextAsset>(path);
            return textAsset == null ? string.Empty : textAsset.text;
            
        }
        
        /// <summary>
        /// Method <c>SaveBestLap</c> saves the best lap.
        /// </summary>
        /// <param name="json">The JSON string of the best lap.</param>
        public static void SaveBestLap(string json)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestLap.json";
            var path = Path.Combine(Application.persistentDataPath, fileName);
            File.WriteAllText(path, json);
        }
        
        /// <summary>
        /// Method <c>LoadBestLap</c> loads the best lap.
        /// </summary>
        /// <returns>The JSON string of the best lap.</returns>
        public static string LoadBestLap()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            var fileName = $"{sceneName}_BestLap";
            var path = Path.Combine(Application.persistentDataPath, fileName + ".json");
            if (File.Exists(path))
                return File.ReadAllText(path);
            // If file doesn't exist, try with Unity resources
            path = $"SavedData/{fileName}";
            var textAsset = Resources.Load<TextAsset>(path);
            return textAsset == null ? string.Empty : textAsset.text;
        }
    }
}
