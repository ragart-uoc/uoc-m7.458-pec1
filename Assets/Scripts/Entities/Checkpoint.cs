using PEC1.Managers;
using UnityEngine;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>Checkpoint</c> is used to detect when the player passes through a checkpoint.
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        /// <value>Property <c>m_RaceManager</c> represents the RaceManager instance.</value>
        private RaceManager m_RaceManager;
        
        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when a GameObject collides with another GameObject.
        /// </summary>
        /// <param name="col">The other Collider involved in this collision.</param>
        private void OnTriggerEnter(Collider col)
        {
            if (col.CompareTag("Player"))
            {
                m_RaceManager.PlayerThroughCheckpoint(this); 
            }
        }
        
        /// <summary>
        /// Method <c>SetRaceManager</c> sets the RaceManager instance.
        /// </summary>
        /// <param name="raceManager">The RaceManager instance.</param>
        public void SetRaceManager(RaceManager raceManager)
        {
            m_RaceManager = raceManager;
        }
    }
}
