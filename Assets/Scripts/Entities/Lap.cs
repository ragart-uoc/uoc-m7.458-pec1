using System.Collections.Generic;
using UnityEngine;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>GhostLapData</c> contains the data of a lap.
    /// </summary>
    public class Lap
    {
        /// <value>Property <c>LapNumber</c> represents the lap number.</value>
        public int LapNumber { get; set; }

        /// <value>Property <c>m_LapTime</c> represents the lap time.</value>
        public float LapTime { get; set; }
        
        /// <value>Property <c>m_CurrentTimeBetweenSamples</c> represents the current time between samples.</value>
        public float CurrentTimeBetweenSamples { get; set; }
        
        /// <value>Property <c>m_CarPositions</c> represents the list of positions of the lap.</value>
        private List<Vector3> m_CarPositions;
        
        /// <value>Property <c>m_CarRotations</c> represents the list of rotations of the lap.</value>
        private List<Quaternion> m_CarRotations;
        
        /// <summary>
        /// Method <c>Lap</c> is the constructor of the class.
        /// </summary>
        /// <param name="lapNumber">The lap number.</param>
        public Lap(int lapNumber)
        {
            LapNumber = lapNumber;
            LapTime = 0;
            m_CarPositions = new List<Vector3>();
            m_CarRotations = new List<Quaternion>();
        }

        /// <summary>
        /// Method <c>AddNewData</c> adds a new position and rotation to the lists.
        /// </summary>
        /// <param name="transform">The transform with the position and rotation information.</param>
        public void AddNewData(Transform transform)
        {
            m_CarPositions.Add(transform.position);
            m_CarRotations.Add(transform.rotation);
        }

        /// <summary>
        /// Method <c>GetDataAt</c> gets the position and rotation at the given sample.
        /// </summary>
        /// <param name="sample">The sample number.</param>
        /// <param name="position">The position at the given sample.</param>
        /// <param name="rotation">The rotation at the given sample.</param>
        /// <returns>True if the sample is in the list, false otherwise.</returns>
        public bool GetDataAt(int sample, out Vector3 position, out Quaternion rotation)
        {
            // If not in array
            if (sample >= m_CarPositions.Count)
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
                return false;
            }
            position = m_CarPositions[sample];
            rotation = m_CarRotations[sample];
            return true;
        }
    }
}
