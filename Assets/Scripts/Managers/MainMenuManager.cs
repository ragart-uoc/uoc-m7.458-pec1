using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>MainMenuManager</c> manages the main menu.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static MainMenuManager _instance;
        
        /// <value>Property <c>mainMenuFirstSelectedButton</c> represents the main menu first selected button.</value>
        public Button mainMenuFirstSelectedButton;
        
        /// <value>Property <c>controlsPanel</c> represents the controls panel.</value>
        public GameObject controlsPanel;
        
        /// <value>Property <c>controlsFirstSelectedButton</c> represents the controls first selected button.</value>
        public Button controlsFirstSelectedButton;
        
        /// <value>Property <c>creditsPanel</c> represents the credits panel.</value>
        public GameObject creditsPanel;
        
        /// <value>Property <c>creditsFirstSelectedButton</c> represents the credits first selected button.</value>
        public Button creditsFirstSelectedButton;
        
        /// <value>Property <c>m_GameManager</c> represents the GameManager instance.</value>
        private GameManager m_GameManager;
        
        /// <value>Property <c>m_AudioSource</c> represents the AudioSource component.</value>
        private AudioSource m_AudioSource;

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
            mainMenuFirstSelectedButton.Select();
            
            // Get the AudioSource component
            m_AudioSource = Camera.main.GetComponent<AudioSource>();
        }
        
        /// <summary>
        /// Method <c>StartGame</c> is used to start the game.
        /// </summary>
        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }
        
        private IEnumerator StartGameCoroutine()
        {
            var audioClip = Resources.Load<AudioClip>("Sounds/yeehaw");
            m_AudioSource.PlayOneShot(audioClip);
            yield return new WaitWhile(() => m_AudioSource.isPlaying);
            SceneManager.LoadScene("ScreenSelection");
        }
        
        /// <summary>
        /// Method <c>ToggleCredits</c> is used to toggle the credits.
        /// </summary>
        public void ToggleCredits()
        {
            creditsPanel.SetActive(!creditsPanel.activeSelf);
            if (creditsPanel.activeSelf)
            {
                creditsFirstSelectedButton.Select();
            }
            else
            {
                mainMenuFirstSelectedButton.Select();
            }
        }
        
        /// <summary>
        /// Method <c>ToggleControls</c> is used to toggle the controls.
        /// </summary>
        public void ToggleControls()
        {
            controlsPanel.SetActive(!controlsPanel.activeSelf);
            if (controlsPanel.activeSelf)
            {
                controlsFirstSelectedButton.Select();
            }
            else
            {
                mainMenuFirstSelectedButton.Select();
            }
        }
        
        /// <summary>
        /// Method <c>ExitGame</c> is used to exit the game.
        /// </summary>
        public void ExitGame()
        {
            m_GameManager.ExitGame();
        }
    }
}
