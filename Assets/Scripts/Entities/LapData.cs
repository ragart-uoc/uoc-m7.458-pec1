using System;
using System.Collections.Generic;
using UnityEngine;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>LapData</c> contains the data of a lap.
    /// </summary>
    [Serializable]
    public class LapData
    {
        /// <value>Property <c>lapNumber</c> represents the lap number.</value>
        public float lapTime;
        
        /// <value>Property <c>carPositions</c> represents the list of positions of the lap.</value>
        public List<Vector3> carPositions;
        
        /// <value>Property <c>carRotations</c> represents the list of rotations of the lap.</value>
        public List<Quaternion> carRotations;
        
        /// <summary>
        /// Method <c>LapData</c> is the constructor of the class.
        /// </summary>
        /// <param name="lapTime">The lap time.</param>
        /// <param name="carPositions">The list of positions of the lap.</param>
        /// <param name="carRotations">The list of rotations of the lap.</param>
        public LapData(float lapTime, List<Vector3> carPositions, List<Quaternion> carRotations)
        {
            this.lapTime = lapTime;
            this.carPositions = carPositions;
            this.carRotations = carRotations;
        }
        
        /// <summary>
        /// Method <c>ToJson</c> converts the object to a JSON string.
        /// </summary>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        
        /// <summary>
        /// Method <c>FromJson</c> converts a JSON string to an object.
        /// </summary>
        public static LapData FromJson(string json)
        {
            return JsonUtility.FromJson<LapData>(json);
        }
    }
}
