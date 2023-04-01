using System.Collections.Generic;
using UnityEngine;
using PEC1.Entities;
using UnityStandardAssets.Vehicles.Car;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>PlaybackManager</c> contains the methods and properties needed for car playback management.
    /// </summary>
    public class PlaybackManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static PlaybackManager _instance;
        
        /// <value>Property <c>m_CarToPlay</c> represents the car to play.</value>
        private GameObject m_CarToPlay;
        
        /// <value>Property <c>m_CarInitialActiveState</c> represents the car initial active state.</value>
        private bool m_CarInitialActiveState;
        
        /// <value>Property <c>m_CarRigidbody</c> represents the car rigidbody.</value>
        private Rigidbody m_CarRigidbody;
        
        /// <value>Property <c>m_CarController</c> represents the car controller.</value>
        private CarController m_CarController;
        
        /// <value>Property <c>m_CarUserControl</c> represents the car user control.</value>
        private CarUserControl m_CarUserControl;
        
        /// <value>Property <c>m_CarAudio</c> represents the car audio.</value>
        private CarAudio m_CarAudio;
        
        /// <value>Property <c>m_CarAudioSources</c> represents the car audio sources.</value>
        private AudioSource[] m_CarAudioSources;
        
        /// <value>Property <c>m_LapsToPlay</c> represents the laps to play.</value>
        private List<Lap> m_LapsToPlay;
        
        /// <value>Property <c>m_RepeatPlayback</c> represents if the playback needs to be repeated.</value>
        private bool m_RepeatPlayback;

        /// <value>Property <c>m_CompleteLap</c> represents the complete lap.</value>
        private Lap m_CompleteLap;

        /// <value>Property <c>m_PlayLap</c> shows if the lap needs to be played.</value>
        private bool m_PlayLap;
        
        /// <value>Property <c>m_CurrentTimeBetweenSamples</c> represents the current time between samples.</value>
        private float m_CurrentTimeBetweenSamples;
        
        /// <value>Property <c>m_SampleTime</c> represents the time between samples.</value>
        private float m_SampleTime;
        
        /// <value>Property <c>m_CurrentSampleToPlay</c> represents the current sample to play.</value>
        private int m_CurrentSampleToPlay;

        /// <value>Property <c>m_LastSamplePosition</c> represents the last sample position.</value>
        private Vector3 m_LastSamplePosition = Vector3.zero;
        
        /// <value>Property <c>m_LastSampleRotation</c> represents the last sample rotation.</value>
        private Quaternion m_LastSampleRotation = Quaternion.identity;
        
        /// <value>Property <c>m_NextPosition</c> represents the next position.</value>
        private Vector3 m_NextPosition;
        
        /// <value>Property <c>m_NextRotation</c> represents the next rotation.</value>
        private Quaternion m_NextRotation;

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
        }

        /// <summary>
        /// Method <c>Update</c> is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            if (m_PlayLap)
            {
                // Increase the time
                m_CurrentTimeBetweenSamples += Time.deltaTime;

                // If the time is greater than the sample time
                if (m_CurrentTimeBetweenSamples >= m_SampleTime)
                {
                    // Store the next position and rotation
                    m_LastSamplePosition = m_NextPosition;
                    m_LastSampleRotation = m_NextRotation;

                    // If the current sample is the last one
                    if (!m_CompleteLap.GetDataAt(m_CurrentSampleToPlay, out m_NextPosition, out m_NextRotation))
                    {
                        // If repeat is enabled, start again
                        if (m_RepeatPlayback)
                        {
                            // Si se ha acabado la última muestra y se ha marcado la opción de repetir, volvemos a empezar
                            m_CurrentSampleToPlay = 0;
                            m_CompleteLap.GetDataAt(m_CurrentSampleToPlay, out m_NextPosition, out m_NextRotation);
                        }
                        // If not, stop playing
                        else
                        {
                            StopPlaying();
                        }
                    }

                    // Deduct the sample time from the current time
                    m_CurrentTimeBetweenSamples -= m_SampleTime;

                    // Increase the current sample
                    m_CurrentSampleToPlay++;
                }

                // Calculate the percentage between the last sample and the next one
                var percentageBetweenFrames = m_CurrentTimeBetweenSamples / m_SampleTime;

                // Interpolate the position and rotation
                m_CarToPlay.transform.position =
                    Vector3.Slerp(m_LastSamplePosition, m_NextPosition, percentageBetweenFrames);
                m_CarToPlay.transform.rotation =
                    Quaternion.Slerp(m_LastSampleRotation, m_NextRotation, percentageBetweenFrames);
            }
        }

        /// <summary>
        /// Method <c>StartPlaying</c> starts playing a lap.
        /// </summary>
        public void StartPlaying(List<Lap> laps, float timeBetweenSamples, GameObject car, bool repeat = false)
        {
            // Set play lap to true
            m_PlayLap = true;

            // Set initial values
            m_LapsToPlay = laps;
            m_CurrentSampleToPlay = 0;
            m_CurrentTimeBetweenSamples = 0;
            m_SampleTime = timeBetweenSamples;
            m_CarToPlay = car;
            m_RepeatPlayback = repeat;

            // Save car initial active state
            m_CarInitialActiveState = m_CarToPlay.activeSelf;
            
            // Merge received laps into one
            MergeLaps();
            
            // Stop the car using velocity
            m_CarRigidbody = m_CarToPlay.GetComponent<Rigidbody>();
            m_CarRigidbody.velocity = Vector3.zero;
            m_CarRigidbody.angularVelocity = Vector3.zero;
            
            // Get the car into the first sample position
            m_CompleteLap.GetDataAt(0, out m_NextPosition, out m_NextRotation);
            m_CarToPlay.transform.position = m_NextPosition;
            m_CarToPlay.transform.rotation = m_NextRotation;
            
            // Disable the car particles
            var carParticles = m_CarToPlay.GetComponentsInChildren<ParticleSystem>();
            foreach (var carParticle in carParticles)
            {
                carParticle.Stop();
            }

            // Disable car input and audio
            m_CarController = m_CarToPlay.GetComponent<CarController>();
            m_CarUserControl = m_CarToPlay.TryGetComponent(out CarUserControl carUserControl) ? carUserControl : null;
            m_CarAudio = m_CarToPlay.TryGetComponent(out CarAudio carAudio) ? carAudio : null;
            m_CarController.enabled = false;
            if (m_CarUserControl != null) m_CarUserControl.enabled = false;
            if (m_CarAudio != null) m_CarAudio.enabled = false;
            
            // Stop and disable all children audiosources
            m_CarAudioSources = m_CarToPlay.GetComponentsInChildren<AudioSource>();
            foreach (var carAudioSource in m_CarAudioSources)
            {
                carAudioSource.Stop();
                carAudioSource.enabled = false;
            }

            // Enable the car
            m_CarToPlay.SetActive(true);
        }

        /// <summary>
        /// Method <c>StopPlaying</c> stops playing a lap.
        /// </summary>
        public void StopPlaying()
        {
            m_PlayLap = false;

            // Enable or disable the car depending on its initial state
            if (m_CarToPlay == null) return;
            m_CarToPlay.SetActive(m_CarInitialActiveState);
            
            // Enable car input and audio if the car is active
            if (!m_CarToPlay.activeSelf) return;
            m_CarController.enabled = true;
            if (m_CarUserControl != null) m_CarUserControl.enabled = true;
            if (m_CarAudio != null) m_CarAudio.enabled = true;
            foreach (var carAudioSource in m_CarAudioSources)
            {
                carAudioSource.enabled = true;
            }
        }

        /// <summary>
        /// Method <c>MergeLaps</c> merges the laps to play into one.
        /// </summary>
        private void MergeLaps()
        {
            m_CompleteLap = new Lap(0);
            foreach (var lap in m_LapsToPlay)
            {
                // Loop through all the samples of the lap until GetDataAt returns false
                for (var i = 0; lap.GetDataAt(i, out var position, out var rotation); i++)
                {
                    m_CompleteLap.AddNewData(position, rotation);
                }
            }
        }
    }
}
