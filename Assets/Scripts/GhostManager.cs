using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class GhostManager : MonoBehaviour
{
    public float timeBetweenSamples = 0.25f;
    public GhostLapData bestLapScO;              // Scriptable object that will contain the ghost data
    public GameObject carToRecord;              
    public GameObject carToPlay;                

    // RECORD VARIABLES
    private bool m_ShouldRecord;
    private float m_TotalRecordedTime;
    private float m_CurrenttimeBetweenSamples;

    // REPLAY VARIABLES
    private bool m_ShouldPlay;
    private float m_TotalPlayedTime;
    private float m_CurrenttimeBetweenPlaySamples;
    private int m_CurrentSampleToPlay;

    // POSITIONS/ROTATIONS
    private Vector3 m_LastSamplePosition = Vector3.zero;
    private Quaternion m_LastSampleRotation = Quaternion.identity;
    private Vector3 m_NextPosition;
    private Quaternion m_NextRotation;



    #region RECORD GHOST DATA
    void StartRecording()
    {
        Debug.Log("START RECORDING");
        m_ShouldRecord = true;
        m_ShouldPlay = false;

        // Seteamos los valores iniciales
        m_TotalRecordedTime = 0;
        m_CurrenttimeBetweenSamples = 0;

        // Limpiamos el scriptable object
        bestLapScO.Reset();
    }

    void StopRecording()
    {
        Debug.Log("STOP RECORDING");
        m_ShouldRecord = false;
    }
    #endregion

    #region PLAY GHOST DATA
    void StartPlaying()
    {
        Debug.Log("START PLAYING");
        m_ShouldPlay = true;
        m_ShouldRecord = false;

        // Seteamos los valores iniciales
        m_TotalPlayedTime = 0;
        m_CurrentSampleToPlay = 0;
        m_CurrenttimeBetweenPlaySamples = 0;

        // Desactivamos el control del coche
        carToPlay.GetComponent<CarController>().enabled = false;
        carToPlay.GetComponent<CarUserControl>().enabled = false;
    }

    void StopPlaying()
    {
        Debug.Log("STOP PLAYING");
        m_ShouldPlay = false;

        // Devolvemos el control al coche por si fuera necesario (opcional)
        carToPlay.GetComponent<CarController>().enabled = true;
        carToPlay.GetComponent<CarUserControl>().enabled = true;

    }
    #endregion

    private void Update()
    {
        HandleTestActionInputs();

        if (m_ShouldRecord)
        {
            // A cada frame incrementamos el tiempo transcurrido 
            m_TotalRecordedTime += Time.deltaTime;
            m_CurrenttimeBetweenSamples += Time.deltaTime;

            // Si el tiempo transcurrido es mayor que el tiempo de muestreo
            if (m_CurrenttimeBetweenSamples >= timeBetweenSamples)
            {
                // Guardamos la información para el fantasma
                bestLapScO.AddNewData(carToRecord.transform);
                // Dejamos el tiempo extra entre una muestra y otra
                m_CurrenttimeBetweenSamples -= timeBetweenSamples;
            }
        }
        else if (m_ShouldPlay)
        {
            // A cada frame incrementamos el tiempo transcurrido 
            m_TotalPlayedTime += Time.deltaTime;
            m_CurrenttimeBetweenPlaySamples += Time.deltaTime;

            // Si el tiempo transcurrido es mayor que el tiempo de muestreo
            if (m_CurrenttimeBetweenPlaySamples >= timeBetweenSamples)
            {
                // De cara a interpolar de una manera fluida la posición del coche entre una muestra y otra,
                // guardamos la posición y la rotación de la anterior muestra
                m_LastSamplePosition = m_NextPosition;
                m_LastSampleRotation = m_NextRotation;

                // Cogemos los datos del scriptable object
                bestLapScO.GetDataAt(m_CurrentSampleToPlay, out m_NextPosition, out m_NextRotation);

                // Dejamos el tiempo extra entre una muestra y otra
                m_CurrenttimeBetweenPlaySamples -= timeBetweenSamples;

                // Incrementamos el contador de muestras
                m_CurrentSampleToPlay++;
            }

            // De cara a crear una interpolación suave entre la posición y rotación entre una muestra y la otra, 
            // calculamos a nivel de tiempo entre muestras el porcentaje en el que nos encontramos
            float percentageBetweenFrames = m_CurrenttimeBetweenPlaySamples / timeBetweenSamples;
            Debug.Log(percentageBetweenFrames);

            // Aplicamos un lerp entre las posiciones y rotaciones de la muestra anterior y la siguiente según el procentaje actual.
            carToPlay.transform.position = Vector3.Slerp(m_LastSamplePosition, m_NextPosition, percentageBetweenFrames);
            carToPlay.transform.rotation = Quaternion.Slerp(m_LastSampleRotation, m_NextRotation, percentageBetweenFrames);
        }
    }


    void HandleTestActionInputs()
    {
        // START/STOP RECORDING
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (m_ShouldRecord)
                StopRecording();
            else
                StartRecording();
        }

        // PLAY RECORDED LAP
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (m_ShouldPlay)
                StopPlaying();
            else
                StartPlaying();
        }

        // RESET
        if (Input.GetKeyDown(KeyCode.Delete))
            bestLapScO.Reset();
    }

}
