using System;
using System.Collections.Generic;
using UnityEngine;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>RaceData</c> contains the data of a race.
    /// </summary>
    [Serializable]
    public class RaceData
    {
        /// <value>Property <c>trackName</c> represents the name of the track.</value>
        public string trackName;

        /// <value>Property <c>lapNumber</c> represents the number of laps.</value>
        public int lapNumber;

        /// <value>Property <c>raceTime</c> represents the race time.</value>
        public float raceTime; 
        
        /// <value>Property <c>laps</c> represents the list of laps.</value>
        public List<LapData> laps;
        
        /// <summary>
        /// Method <c>RaceData</c> is the constructor of the class.
        /// </summary>
        /// <param name="name">The name of the track.</param>
        /// <param name="number">The number of laps.</param>
        /// <param name="time">The race time.</param>
        /// <param name="list">The list of laps.</param>
        public RaceData(string name, int number, float time, List<LapData> list)
        {
            trackName = name;
            lapNumber = number;
            raceTime = time;
            laps = list;
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
        public static RaceData FromJson(string json)
        {
            return JsonUtility.FromJson<RaceData>(json);
        }
    }
}
