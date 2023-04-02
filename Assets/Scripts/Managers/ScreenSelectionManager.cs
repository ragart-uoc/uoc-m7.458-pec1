using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PEC1.Entities;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>ScreenSelectionManager</c> manages the screen selection.
    /// </summary>
    public class ScreenSelectionManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static ScreenSelectionManager _instance;

        /// <value>Property <c>screenContainer</c> represents the screen container.</value>
        public GameObject screenContainer;

        /// <value>Property <c>screens</c> represents the list of screens.</value>
        private List<SelectorScreen> m_Screens;
        
        /// <value>Property <c>currentScreen</c> represents the current screen.</value>
        private int m_CurrentScreen;

        /// <value>Property <c>trackSelectionText</c> represents the track selection text.</value>
        public TextMeshProUGUI trackSelectionText;
        
        /// <value>Property <c>lapSelectionText</c> represents the lap selection text.</value>
        public TextMeshProUGUI lapSelectionText;
        
        /// <value>Property <c>m_GameManager</c> represents the GameManager instance.</value>
        private GameManager m_GameManager;

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
            
            // Get the GameManager instance
            m_GameManager = FindObjectOfType<GameManager>();
        }

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Get the screens
            m_Screens = new List<SelectorScreen>();
            foreach (Transform child in screenContainer.transform)
            {
                child.gameObject.SetActive(false);
                var screen = new SelectorScreen
                {
                    Name = Regex.Replace(child.name.Replace("Track-", ""), "(\\B[A-Z])", " $1"),
                    SceneName = child.name,
                    ScreenObject = child.gameObject
                };
                m_Screens.Add(screen);
            }
            
            // Set the first screen as active
            SetScreen(0);
            
            // Set the lap selection text
            lapSelectionText.text = $"Laps: {m_GameManager.GetLaps()}";
        }

        /// <summary>
        /// Method <c>SwitchTrack</c> switches the track.
        /// </summary>
        public void SwitchTrack()
        {
            m_Screens[m_CurrentScreen].ScreenObject.SetActive(false);
            var nextScreen = (m_CurrentScreen + 1) % m_Screens.Count;
            SetScreen(nextScreen);
        }
        
        /// <summary>
        /// Method <c>SetScreen</c> sets the screen.
        /// </summary>
        /// <param name="screenIndex">The screen index.</param>
        private void SetScreen(int screenIndex)
        {
            m_CurrentScreen = screenIndex;
            m_Screens[m_CurrentScreen].ScreenObject.SetActive(true);
            trackSelectionText.text = $"Track: {m_Screens[m_CurrentScreen].Name}";
            m_GameManager.SetSceneName(m_Screens[m_CurrentScreen].SceneName);
        }
        
        /// <summary>
        /// Method <c>SwitchLaps</c> switches the laps.
        /// </summary>
        public void SwitchLaps()
        {
            var nextLap = (m_GameManager.GetLaps() + 1) % 10;
            if (nextLap == 0) nextLap = 1;
            lapSelectionText.text = $"Laps: {nextLap}";
            m_GameManager.SetLaps(nextLap);
        }
        
        /// <summary>
        /// Method <c>NextScreen</c> loads the next screen.
        /// </summary>
        public void NextScreen()
        {
            SceneManager.LoadScene("CarSelection");
        }
        
        /// <summary>
        /// Method <c>PreviousScreen</c> loads the previous screen.
        /// </summary>
        public void PreviousScreen()
        {
            m_GameManager.GoToMainMenu();
        }
    }
}
