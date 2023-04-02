using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using PEC1.Entities;
using UnityStandardAssets.Cameras;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>RaceManager</c> contains contains the methods and properties needed for the race.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static RaceManager _instance;

        /// <value>Property <c>playerContainer</c> represents the object containing the player.</value>
        public GameObject playerContainer;

        /// <value>Property <c>m_Player</c> represents the player GameObject.</value>
        private GameObject m_Player;
        
        /// <value>Property <c>m_PlayerRigidbody</c> represents the player Rigidbody.</value>
        private Rigidbody m_PlayerRigidbody;

        /// <value>Property <c>ghost</c> represents the ghost GameObject.</value>
        public GameObject ghost;
        
        /// <value>Property <c>checkpointContainer</c> represents the object containing the track checkpoints.</value>
        public Transform checkpointContainer;

        /// <value>Property <c>mainCameraRig</c> represents the main camera rig.</value>
        public GameObject mainCameraRig;
        
        /// <value>Property <c>replayCameraRig</c> represents the replay camera rig.</value>
        public GameObject replayCameraRig;

        /// <value>Property <c>playbackManager</c> represents the PlaybackManager instance.</value>
        public PlaybackManager playbackManager;

        /// <value>Property <c>uiManager</c> represents the UIManager instance.</value>
        public UIManager uiManager;

        /// <value>Property <c>m_Race</c> represents the current race.</value>
        private Race m_Race;

        /// <value>Property <c>m_BestRace</c> represents the best race.</value>
        private Race m_BestRace;

        /// <value>Property <c>m_CurrentLap</c> represents the current lap.</value>
        private Lap m_CurrentLap;

        /// <value>Property <c>m_BestLap</c> represents the best lap.</value>
        private Lap m_BestLap;
        
        /// <value>Property <c>m_TrackReadableName</c> represents the track readable name.</value>
        private string m_TrackReadableName;
        
        /// <value>Property <c>m_GameManager</c> represents the GameManager instance.</value>
        private GameManager m_GameManager;
        
        /// <value>Property <c>checkpoints</c> represents the list of checkpoints.</value>
        private List<Checkpoint> m_Checkpoints;
        
        /// <value>Property <c>m_NextCheckpointIndex</c> represents the index of the next checkpoint.</value>
        private int m_NextCheckpointIndex;
        
        /// <value>Property <c>m_LastCheckpointPosition</c> represents the position of the last checkpoint.</value>
        private Vector3 m_LastCheckpointPosition;
        
        /// <value>Property <c>m_LastCheckpointRotation</c> represents the rotation of the last checkpoint.</value>
        private Quaternion m_LastCheckpointRotation;

        /// <value>Property <c>m_RaceActive</c> shows if the race is active.</value>
        private bool m_RaceActive;
        
        /// <value>Property <c>m_RecordLap</c> shows if the lap needs to be recorded.</value>
        private bool m_RecordLap;
        
        /// <value>Property <c>m_IsRecording</c> shows if the lap is being recorded.</value>
        [SerializeField]
        private float sampleTime = 0.1f;
        
        /// <value>Property <c>m_PlayerInactivityTime</c> represents the time ellapsed since the player has been inactive.</value>
        private float m_PlayerInactivityTime;
        
        /// <value>Property <c>m_IsPaused</c> shows if the game is paused.</value>
        private bool m_IsPaused;

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
        /// Method <c>OnEnable</c> is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        /// <summary>
        /// Method <c>OnSceneLoaded</c> is called when a scene is loaded.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Instantiate the player
            m_Player = Instantiate(m_GameManager.GetCarPrefab(), playerContainer.transform.position, playerContainer.transform.rotation);
            m_Player.transform.parent = playerContainer.transform;
            mainCameraRig.GetComponent<AutoCam>().SetTarget(m_Player.transform);
            var cinemachineClearShot = replayCameraRig.transform.Find("CM ClearShot1").GetComponent<CinemachineClearShot>();
            cinemachineClearShot.LookAt = m_Player.transform;
            var replayCameras = cinemachineClearShot.GetComponentsInChildren<CinemachineVirtualCamera>();
            foreach (var replayCamera in replayCameras)
            {
                if (replayCamera.Follow != null
                    && replayCamera.Follow.transform.name == "PlayerContainer")
                {
                    replayCamera.Follow = m_Player.transform;
                }
                if (replayCamera.LookAt != null
                    && replayCamera.LookAt.transform.name == "PlayerContainer")
                {
                    replayCamera.LookAt = m_Player.transform;
                }
            }
            
            // Set the race object
            m_Race = new Race
            {
                TrackName = SceneManager.GetActiveScene().name,
                LapNumber = m_GameManager.GetLaps()
            };
            
            // Get the track readable name
            m_TrackReadableName = Regex.Replace(m_Race.TrackName.Replace("Track-", ""), "(\\B[A-Z])", " $1");
            
            // Show the track name
            uiManager.ShowTrackName(m_TrackReadableName, 3f);

            // Get the checkpoints
            m_Checkpoints = new List<Checkpoint>();
            foreach (Transform checkpointTransform in checkpointContainer)
            {
                var checkpoint = checkpointTransform.GetComponent<Checkpoint>();
                checkpoint.SetRaceManager(this);
                m_Checkpoints.Add(checkpoint);
            }
            
            // Get the player rigidbody
            m_PlayerRigidbody = m_Player.GetComponent<Rigidbody>();
            
            // Set the initial position
            m_LastCheckpointPosition = m_Player.transform.position;
            m_LastCheckpointRotation = m_Player.transform.rotation;
            
            // Check if there's a best race saved
            var savedBestRace = PersistentDataManager.LoadBestRace(m_Race.LapNumber);
            if (savedBestRace != String.Empty)
            {
                m_BestRace = new Race();
                m_BestRace.ImportData(RaceData.FromJson(savedBestRace));
                uiManager.ShowBestRaceTime(m_BestRace.RaceTime);
            }

            // Check if there's a best lap saved
            var savedBestLap = PersistentDataManager.LoadBestLap();
            if (savedBestLap != String.Empty)
            {
                m_BestLap = new Lap(0);
                m_BestLap.ImportData(LapData.FromJson(savedBestLap));
                uiManager.ShowBestLapTime(m_BestLap.LapTime);
            // If not, check the best race saved for the best lap time    
            } else if (m_BestRace != null)
            {
                var laps = m_BestRace.GetLaps();
                m_BestLap = laps[0];
                foreach (var lap in laps.Where(lap => lap.LapTime < m_BestLap.LapTime))
                {
                    m_BestLap = lap;
                }
                uiManager.ShowBestLapTime(m_BestLap.LapTime);
                // Write the lap data to a file
                var lapData = m_BestLap.ExportData().ToJson();
                PersistentDataManager.SaveBestLap(lapData);
            }
        }

        /// <summary>
        /// Method <c>OnDisable</c> is called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, , if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // If user presses Cancel key, toggle pause menu
            if (Input.GetButtonDown("Cancel"))
            {
                TogglePause();
            }
            
            // If user presses Fire1 key, return to last checkpoint
            if (Input.GetButtonDown("Fire1"))
            {
                ReturnToLastCheckpoint();
            }
            
            // Increase the race total time
            if (m_RaceActive)
            {
                m_Race.RaceTime += Time.deltaTime;
                uiManager.UpdateRaceTime(m_Race.RaceTime);
            }
            
            // Record the lap
            if (m_RecordLap)
            {
                // Increase the time
                m_CurrentLap.LapTime += Time.deltaTime;
                uiManager.UpdateCurrentLapTime(m_CurrentLap.LapTime);
                m_CurrentLap.CurrentTimeBetweenSamples += Time.deltaTime;
                
                // Check if the time between samples is greater than the sample time
                if (!(m_CurrentLap.CurrentTimeBetweenSamples >= sampleTime)) return;
                
                // Save the data
                m_CurrentLap.AddNewData(m_Player.transform.position, m_Player.transform.rotation);
                
                // Keep the extra time between samples
                m_CurrentLap.CurrentTimeBetweenSamples -= sampleTime;
            }
            
            // Print a message if the player is inactive
            if (m_PlayerRigidbody.velocity.magnitude < 0.1f)
            {
                m_PlayerInactivityTime += Time.deltaTime;
                if (m_PlayerInactivityTime > 5)
                {
                    uiManager.ShowSubmessage("Press Fire1 to return to the last checkpoint", 2f);
                    m_PlayerInactivityTime = 0;
                }
            }
            else
            {
                m_PlayerInactivityTime = 0;
            }
        }

        /// <summary>
        /// Method <c>NextLap</c> is called when the player passes through the goal line and the race is not over.
        /// </summary>
        private void NextLap()
        {
            // Instantiate the next lap
            m_CurrentLap = (m_CurrentLap == null) ? new Lap(1) : new Lap(m_CurrentLap.LapNumber + 1);

            // Start recording the lap
            m_RecordLap = true;
            
            // Print the lap message
            var lapMessage = m_CurrentLap.LapNumber == m_GameManager.GetLaps()
                ? "Last lap!"
                : $"Lap {m_CurrentLap.LapNumber}";
            uiManager.ShowMessage(lapMessage, 2f);
            
            // Play the ghost
            if (m_BestRace != null && m_CurrentLap.LapNumber == 1)
            {
                playbackManager.StartPlaying(m_BestRace.GetLaps(), sampleTime, ghost);
            }
            else if (m_BestRace == null && m_BestLap != null)
            {
                playbackManager.StopPlaying();
                playbackManager.StartPlaying(new List<Lap>() { m_BestLap }, sampleTime, ghost);
            }
        }

        /// <summary>
        /// Method <c>RaceOver</c> is called when the player passes through the goal line in the last lap.
        /// </summary>
        private void RaceOver()
        {
            // Stop the race
            m_RaceActive = false;
            
            // Stop any active playback
            playbackManager.StopPlaying();
            
            // Write the race data to a file
            var raceData = m_Race.ExportData().ToJson();
            PersistentDataManager.SaveBestRace(raceData, m_Race.LapNumber);
            
            // Print the race message
            uiManager.ShowMessage("Race complete", 2f);
            
            // Show the race over menu
            uiManager.ToggleRaceOverMenu();
            
            // Start the replay of the race
            playbackManager.StartPlaying(m_Race.GetLaps(), sampleTime, m_Player, true);
        }
        
        /// <summary>
        /// Method <c>PlayerThroughCheckpoint</c> is called when the player passes through a checkpoint.
        /// </summary>
        /// <param name="checkpoint">The checkpoint that the player has passed through.</param>
        public void PlayerThroughCheckpoint(Checkpoint checkpoint)
        {
            // Check if the player has passed through the right checkpoint
            if (m_Checkpoints.IndexOf(checkpoint) != m_NextCheckpointIndex)
            {
                uiManager.ShowMessage("Wrong checkpoint!", 2f);
                return;
            }
            
            // Disable the current checkpoint
            checkpoint.gameObject.SetActive(false);
            
            // Save the position of the last checkpoint
            m_LastCheckpointPosition = m_Player.transform.position;
            m_LastCheckpointRotation = m_Player.transform.rotation;
            
            // Check if the player has passed through the goal line
            if (checkpoint.gameObject.CompareTag("GoalLine"))
            {
                // Start the race if it is the first lap
                if (m_CurrentLap == null)
                {
                    m_RaceActive = true;
                }
                else
                {
                    // Stop recording the lap
                    m_RecordLap = false;
                
                    // Add the finished lap to the list
                    m_Race.AddLap(m_CurrentLap);
                    
                    // Print the lap time
                    uiManager.AddLapTime(m_CurrentLap.LapNumber, m_CurrentLap.LapTime);
                    
                    // Get the lap with the best time
                    m_BestLap ??= m_Race.GetLaps()[0];
                    if (m_BestLap.LapTime > m_CurrentLap.LapTime)
                        m_BestLap = m_CurrentLap;
                    
                    // Write the lap data to a file
                    var lapData = m_BestLap.ExportData().ToJson();
                    PersistentDataManager.SaveBestLap(lapData);
                    
                    // Update the best lap time
                    uiManager.ShowBestLapTime(m_BestLap.LapTime);
                    
                    // If the current lap is the best lap, show a message
                    if (m_CurrentLap == m_BestLap)
                    {
                        uiManager.ShowSubmessage("New record!", 2f);
                    }

                    // End the race if it is the last lap
                    if (m_CurrentLap.LapNumber == m_GameManager.GetLaps())
                    {
                        RaceOver();
                        return;
                    }
                }
                
                // Start the next lap
                NextLap();
            }
            
            // Increase the checkpoint index
            m_NextCheckpointIndex = (m_NextCheckpointIndex + 1) % m_Checkpoints.Count;
            
            // Enable the next checkpoint
            m_Checkpoints[m_NextCheckpointIndex].gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Method <c>ReturnToLastCheckpoint</c> returns the player to the last checkpoint.
        /// </summary>
        public void ReturnToLastCheckpoint()
        {
            // Stop the car
            m_PlayerRigidbody.velocity = Vector3.zero;
            m_PlayerRigidbody.angularVelocity = Vector3.zero;
            
            // Return the player to the last checkpoint
            m_Player.transform.position = m_LastCheckpointPosition;
            m_Player.transform.rotation = m_LastCheckpointRotation;
        }
        
        /// <summary>
        /// Method <c>TooglePause</c> is used to pause the game.
        /// </summary>
        public void TogglePause()
        {
            // Do nothing is race over menu is active
            if (uiManager.IsRaceOverMenuActive())
                return;
            m_IsPaused = !m_IsPaused;
            // Pause or resume time and audio
            Time.timeScale = m_IsPaused ? 0 : 1;
            AudioListener.pause = m_IsPaused;
            // Show or hide the pause menu
            uiManager.TogglePauseMenu();
        }

        /// <summary>
        /// Method <c>ToggleMinimap</c> is used to toggle the minimap.
        /// </summary>
        public void ToggleMinimap()
        {
            uiManager.ToggleMinimapCameraRig();
        }

        /// <summary>
        /// Method <c>WatchReplay</c> is used to show the replay of the race,
        /// </summary>
        public void WatchReplay()
        {
            uiManager.ToggleRaceOverMenu();
            mainCameraRig.SetActive(false);
            replayCameraRig.SetActive(true);
            playbackManager.StartPlaying(m_Race.GetLaps(), sampleTime, m_Player);
        }

        /// <summary>
        /// Method <c>GoToMainMenu</c> is used to go to the main menu.
        /// </summary>
        public void GoToMainMenu()
        {
            m_GameManager.GoToMainMenu();
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