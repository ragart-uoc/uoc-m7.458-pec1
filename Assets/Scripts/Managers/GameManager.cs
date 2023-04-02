using UnityEngine;
using UnityEngine.SceneManagement;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>GameManager</c> controls the flow of the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static GameManager _instance;
        
        /// <value>Property <c>defaultLapNumber</c> represents the default number of laps.</value>
        [SerializeField]
        private int defaultLapNumber = 3;
        
        /// <value>Property <c>lapNumber</c> represents the chosen number of laps.</value>
        [SerializeField]
        private int lapNumber;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
            
            // Set the number of laps
            lapNumber = defaultLapNumber;
        }
        
        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Disable the mouse entirely
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        /// <summary>
        /// Method <c>SetLaps</c> sets the number of laps.
        /// </summary>
        /// <param name="laps">The number of laps.</param>
        public void SetLaps(int laps)
        {
            lapNumber = laps;
        }
        
        /// <summary>
        /// Method <c>GetLaps</c> gets the number of laps.
        /// </summary>
        /// <returns>The number of laps.</returns>
        public int GetLaps()
        {
            return lapNumber;
        }

        /// <summary>
        /// Method <c>TooglePause</c> is used to pause the game.
        /// </summary>
        public void GoToMainMenu()
        {
            Destroy(gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Method <c>QuitGame</c> quits the game.
        /// </summary>
        public void ExitGame()
        {
            Application.Quit();
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
