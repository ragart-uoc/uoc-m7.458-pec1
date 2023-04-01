using UnityEngine;
using PEC1.Entities;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>GhostManager</c> contains the methods and properties needed for ghost management.
    /// </summary>
    public class GhostManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static GhostManager _instance;
        
        /// <value>Property <c>carToPlay</c> represents the car to play.</value>
        public GameObject carToPlay;
        
        /// <value>Property <c>carToRecord</c> represents the car to play.</value>
        private Lap m_LapToPlay;

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
                // A cada frame incrementamos el tiempo transcurrido
                m_CurrentTimeBetweenSamples += Time.deltaTime;

                // Si el tiempo transcurrido es mayor que el tiempo de muestreo
                if (m_CurrentTimeBetweenSamples >= m_SampleTime)
                {
                    // De cara a interpolar de una manera fluida la posición del coche entre una muestra y otra,
                    // guardamos la posición y la rotación de la anterior muestra
                    m_LastSamplePosition = m_NextPosition;
                    m_LastSampleRotation = m_NextRotation;

                    // Cogemos los datos del scriptable object
                    if (!m_LapToPlay.GetDataAt(m_CurrentSampleToPlay, out m_NextPosition, out m_NextRotation))
                    {
                        StopPlaying();
                    }

                        // Dejamos el tiempo extra entre una muestra y otra
                    m_CurrentTimeBetweenSamples -= m_SampleTime;

                    // Incrementamos el contador de muestras
                    m_CurrentSampleToPlay++;
                }

                // De cara a crear una interpolación suave entre la posición y rotación entre una muestra y la otra, 
                // calculamos a nivel de tiempo entre muestras el porcentaje en el que nos encontramos
                var percentageBetweenFrames = m_CurrentTimeBetweenSamples / m_SampleTime;

                // Aplicamos un lerp entre las posiciones y rotaciones de la muestra anterior y la siguiente según el procentaje actual.
                carToPlay.transform.position =
                    Vector3.Slerp(m_LastSamplePosition, m_NextPosition, percentageBetweenFrames);
                carToPlay.transform.rotation =
                    Quaternion.Slerp(m_LastSampleRotation, m_NextRotation, percentageBetweenFrames);
            }
        }

        /// <summary>
        /// Method <c>StartPlaying</c> starts playing a lap.
        /// </summary>
        public void StartPlaying(Lap lap, float timeBetweenSamples)
        {
            m_PlayLap = true;

            // Set initial values
            m_LapToPlay = lap;
            m_CurrentSampleToPlay = 0;
            m_CurrentTimeBetweenSamples = 0;
            m_SampleTime = timeBetweenSamples;

            // Enable the ghost car
            carToPlay.SetActive(true);
        }

        /// <summary>
        /// Method <c>StopPlaying</c> stops playing a lap.
        /// </summary>
        public void StopPlaying()
        {
            m_PlayLap = false;

            // Disable the ghost car
            carToPlay.SetActive(false);
        }
    }
}
