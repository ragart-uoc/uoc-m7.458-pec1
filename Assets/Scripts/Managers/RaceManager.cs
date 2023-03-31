using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PEC1.Entities;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>RaceManager</c> contains contains the methods and properties needed for the race.
    /// </summary>
    public class RaceManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static RaceManager _instance;

        /// <value>Property <c>player</c> represents the player GameObject.</value>
        public GameObject player;
        
        /// <value>Property <c>checkpointContainer</c> represents the object containing the track checkpoints.</value>
        public Transform checkpointContainer;
        
        /// <value>Property <c>ghostManager</c> represents the GhostManager instance.</value>
        public GhostManager ghostManager;

        /// <value>Property <c>uiManager</c> represents the UIManager instance.</value>
        public UIManager uiManager;

        /// <value>Property <c>player</c> represents the player GameObject.</value>
        private float m_TotalTime;
        
        /// <value>Property <c>m_Laps</c> represents the list of laps.</value>
        private List<Lap> m_Laps;

        /// <value>Property <c>m_CurrentLap</c> represents the current lap.</value>
        private Lap m_CurrentLap;
        
        /// <value>Property <c>m_GameManager</c> represents the GameManager instance.</value>
        private GameManager m_GameManager;
        
        /// <value>Property <c>checkpoints</c> represents the list of checkpoints.</value>
        private List<Checkpoint> m_Checkpoints;
        
        /// <value>Property <c>m_NextCheckpointIndex</c> represents the index of the next checkpoint.</value>
        private int m_NextCheckpointIndex;

        /// <value>Property <c>m_RaceActive</c> shows if the race is active.</value>
        private bool m_RaceActive;
        
        /// <value>Property <c>m_RecordLap</c> shows if the lap needs to be recorded.</value>
        private bool m_RecordLap;
        
        /// <value>Property <c>m_IsRecording</c> shows if the lap is being recorded.</value>
        [SerializeField]
        private float sampleTime = 0.25f;

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

            // Get the checkpoints
            m_Checkpoints = new List<Checkpoint>();
            foreach (Transform checkpointTransform in checkpointContainer)
            {
                var checkpoint = checkpointTransform.GetComponent<Checkpoint>();
                    checkpoint.SetRaceManager(this);
                m_Checkpoints.Add(checkpoint);
            }
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, , if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Increase the race total time
            if (m_RaceActive)
            {
                m_TotalTime += Time.deltaTime;
            }
            
            // Record the lap
            if (m_RecordLap)
            {
                // Increase the time
                m_CurrentLap.LapTime += Time.deltaTime;
                m_CurrentLap.CurrentTimeBetweenSamples += Time.deltaTime;
                
                // Check if the time between samples is greater than the sample time
                if (!(m_CurrentLap.CurrentTimeBetweenSamples >= sampleTime)) return;
                
                // Save the data
                m_CurrentLap.AddNewData(player.transform);
                // Keep the extra time between samples
                m_CurrentLap.CurrentTimeBetweenSamples -= sampleTime;
            }
        }

        /// <summary>
        /// Method <c>NextLap</c> is called when the player passes through the goal line and the race is not over.
        /// </summary>
        private void NextLap()
        {
            // Instantiate the lap list and the first lap if it is the first one
            if (m_CurrentLap == null)
            {
                m_Laps = new List<Lap>();
                m_CurrentLap = new Lap(1);
            }
            else
            {
                // Get the lap with the best time
                var bestLap = m_Laps[0];
                foreach (var lap in m_Laps.Where(lap => lap.LapTime < bestLap.LapTime))
                {
                    bestLap = lap;
                }
                // TODO: Print the new record message
                
                // Instantiate the new lap
                var lapNumber = m_CurrentLap.LapNumber + 1;
                m_CurrentLap = new Lap(lapNumber);

                // Play the ghost
                ghostManager.StartPlaying(bestLap, sampleTime);
            }
            // Start recording the lap
            m_RecordLap = true;
            // Print the lap message
            var lapMessage = m_CurrentLap.LapNumber == m_GameManager.GetLaps() ? "Last lap!" : $"Lap {m_CurrentLap.LapNumber}";
            uiManager.ShowMessage(lapMessage, 2f);
        }

        /// <summary>
        /// Method <c>RaceOver</c> is called when the player passes through the goal line in the last lap.
        /// </summary>
        private void RaceOver()
        {
            m_RaceActive = false;
            uiManager.ShowMessage("Race complete", 2f);
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
                    // Stop recording the lap and the ghost, in case they're enabled
                    m_RecordLap = false;
                    ghostManager.StopPlaying();
                
                    // Add the finished lap to the list
                    m_Laps.Add(m_CurrentLap);
                    
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
    }
}