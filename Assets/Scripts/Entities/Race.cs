using System.Collections.Generic;
using System.Linq;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>Race</c> contains the data of a race.
    /// </summary>
    public class Race
    {
        /// <value>Property <c>TrackName</c> represents the name of the track.</value>
        public string TrackName { get; set; }

        /// <value>Property <c>LapNumber</c> represents the number of laps.</value>
        public int LapNumber { get; set; }

        /// <value>Property <c>RaceTime</c> represents the race time.</value>
        public float RaceTime { get; set; } 
        
        /// <value>Property <c>m_Laps</c> represents the list of laps.</value>
        private List<Lap> m_Laps;

        /// <summary>
        /// Method <c>Race</c> is the constructor of the class.
        /// </summary>
        public Race()
        {
            m_Laps = new List<Lap>();
        }
        
        /// <summary>
        /// Method <c>AddLap</c> adds a lap to the list.
        /// </summary>
        /// <param name="lap">The lap to add.</param>
        public void AddLap(Lap lap)
        {
            m_Laps.Add(lap);
        }
        
        /// <summary>
        /// Method <c>GetLaps</c> returns the list of laps.
        /// </summary>
        /// <returns>The list of laps.</returns>
        public List<Lap> GetLaps()
        {
            return m_Laps;
        }
        
        /// <summary>
        /// Method <c>exportData</c> exports the data of the race.
        /// </summary>
        /// <returns>The data of the race.</returns>
        public RaceData ExportData()
        {
            var lapData = m_Laps.Select(lap => lap.ExportData()).ToList();
            return new RaceData(TrackName, LapNumber, RaceTime, lapData);
        }
        
        /// <summary>
        /// Method <c>ImportData</c> imports the data of the race.
        /// </summary>
        /// <param name="raceData">The data of the race.</param>
        public void ImportData(RaceData raceData)
        {
            TrackName = raceData.trackName;
            LapNumber = raceData.lapNumber;
            RaceTime = raceData.raceTime;
            for (var i = 0; i < raceData.laps.Count; i++)
            {
                var lap = new Lap(i + 1);
                lap.ImportData(raceData.laps[i]);
                m_Laps.Add(lap);
            }
        }
    }
}
