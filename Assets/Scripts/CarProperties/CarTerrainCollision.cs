using UnityEngine;
using PEC1.Managers;
using UnityStandardAssets.Vehicles.Car;

namespace PEC1.CarProperties
{
    /// <summary>
    /// Class <c>CarTerrainCollision</c> controls the car's collision with the terrain.
    /// </summary>
    public class CarTerrainCollision : MonoBehaviour
    {
        /// <value>Property <c>car</c> represents the car.</value>
        public CarController carController;
        
        /// <value>Property <c>particleDamageFlare</c> represents the particle damage flare.</value>
        public GameObject particleDamageFlare;
        
        /// <value>Property <c>m_RaceManager</c> represents the RaceManager instance.</value>
        private RaceManager m_RaceManager;

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            m_RaceManager = FindObjectOfType<RaceManager>();
        }
        
        /// <summary>
        /// Method <c>OnTriggerEnter</c> is called when the Collider other enters the trigger.
        /// </summary>
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                particleDamageFlare.SetActive(true);
                carController.isDamaged = true;
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                m_RaceManager.ReturnToLastCheckpoint();
            }
        }
    }
}
