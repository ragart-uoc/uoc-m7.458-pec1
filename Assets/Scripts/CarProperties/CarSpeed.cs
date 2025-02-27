using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace PEC1.CarProperties
{
    /// <summary>
    /// Class <c>CarSpeed</c> controls the max speed of the car.
    /// </summary>
    public class CarSpeed : MonoBehaviour
    {
        /// <value>Property <c>wheelColliders</c> represents the wheel colliders of the car.</value>
        public WheelCollider[] wheelColliders;
        
        /// <value>Property <c>damageSpeedFactor</c> represents the speed factor when the car is damaged.</value>
        public float damageSpeedFactor = 0.5f; 
        
        /// <value>Property <c>offroadSpeedFactor</c> represents the speed factor when the car is offroad.</value>
        public float offroadSpeedFactor = 0.85f;
        
        /// <value>Property <c>m_CarController</c> represents the car controller.</value>
        private CarController m_CarController;
        
        /// <value>Property <c>m_OriginalMaxSpeed</c> represents the original max speed of the car.</value>
        private float m_OriginalMaxSpeed;

        /// <value>Property <c>m_OriginalWasDamaged</c> represents the original damaged state of the car.</value>
        private bool m_OriginalWasDamaged;
        
        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Get the car controller
            m_CarController = GetComponent<CarController>();
            // Store the original max speed
            m_OriginalMaxSpeed = m_CarController.MaxSpeed;
        }

        /// <summary>
        /// Method <c>Update</c> is called once per frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            // Check if the car is damaged
            if (m_OriginalWasDamaged == false)
            {
                if (m_CarController.isDamaged)
                {
                    // Reduce the original max speed
                    m_OriginalMaxSpeed *= damageSpeedFactor;
                    // Store the original damaged state
                    m_OriginalWasDamaged = m_CarController.isDamaged;
                }
            }
            // Check the number of wheels offroad
            var wheelsOffroad = 0;
            foreach (var wheel in wheelColliders)
            {
                wheel.GetGroundHit(out var hit);
                if (hit.collider == null)
                    continue;
                if (!hit.collider.CompareTag("Road"))
                    wheelsOffroad++;
            }
            // Reduce the max speed depending on the number of wheels offroad
            var proportionalReduction = wheelsOffroad == 0 ? 0f : wheelsOffroad / 4f;
            var speedReduction = offroadSpeedFactor * proportionalReduction;
            m_CarController.MaxSpeed = m_OriginalMaxSpeed * (1 - speedReduction);
        }
    }
}
