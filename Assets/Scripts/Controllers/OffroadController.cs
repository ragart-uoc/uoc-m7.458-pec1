using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

namespace PEC1.Controllers
{
    /// <summary>
    /// Class <c>OffroadController</c> reduces the max speed of the car when it is offroad.
    /// </summary>
    public class OffroadController : MonoBehaviour
    {
        /// <value>Property <c>wheelColliders</c> represents the wheel colliders of the car.</value>
        public WheelCollider[] wheelColliders;
        
        /// <value>Property <c>offroadSpeedFactor</c> represents the speed factor when the car is offroad.</value>
        public float offroadSpeedFactor = 0.85f;
        
        /// <value>Property <c>m_CarController</c> represents the car controller.</value>
        private CarController m_CarController;
        
        /// <value>Property <c>m_OriginalMaxSpeed</c> represents the original max speed of the car.</value>
        private float m_OriginalMaxSpeed;
        
        /// <summary>
        /// Method <c>Start</c> is called before the first frame update.
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
