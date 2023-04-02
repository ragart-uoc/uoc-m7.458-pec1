using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>UIManager</c> controls the UI of the game.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static UIManager _instance;

        /// <value>Property <c>messageText</c> represents the text that will be displayed on the screen.</value>
        public TextMeshProUGUI messageText;
        
        /// <value>Property <c>submessageText</c> represents the subtext that will be displayed on the screen.</value>
        public TextMeshProUGUI submessageText;
        
        /// <value>Property <c>trackNameText</c> represents the track name text.</value>
        public TextMeshProUGUI trackNameText;

        /// <value>Property <c>bestRaceTimeText</c> represents the best race time text.</value>
        public TextMeshProUGUI bestRaceTimeText;

        /// <value>Property <c>bestLapTimeText</c> represents the best lap time text.</value>
        public TextMeshProUGUI bestLapTimeText;
        
        /// <value>Property <c>raceTimeText</c> represents the total time text.</value>
        public TextMeshProUGUI raceTimeText;
        
        /// <value>Property <c>currentLapTimeText</c> represents the current lap time text.</value>
        public TextMeshProUGUI currentLapTimeText;

        /// <value>Property <c>lapContainer</c> represents the lap container.</value>
        public GameObject lapTimeContainer;
        
        /// <value>Property <c>lapTimePrefab</c> represents the lap time prefab.</value>
        public GameObject lapTimePrefab;

        /// <value>Property <c>pauseMenu</c> represents the pause menu.</value>
        public GameObject pauseMenu;
        
        /// <value>Property <c>firstSelectedButton</c> represents the first selected button.</value>
        public Button pauseFirstSelectedButton;
        
        /// <summary>
        /// Method <c>ShowMessage</c> shows a message on the screen.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration of the text.</param>
        public void ShowMessage(string message, float duration)
        {
            messageText.text = message;
            StartCoroutine(HideText(messageText, duration));
        }
        
        /// <summary>
        /// Method <c>ShowSubmessage</c> shows a submessage on the screen.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration of the text.</param>
        public void ShowSubmessage(string message, float duration)
        {
            submessageText.text = message;
            StartCoroutine(HideText(submessageText, duration));
        }
        
        /// <summary>
        /// Method <c>ShowTrackName</c> shows the track name on the screen.
        /// </summary>
        /// <param name="trackName">The track name to be shown.</param>
        /// <param name="duration">The duration of the text.</param>
        public void ShowTrackName(string trackName, float duration)
        {
            trackNameText.text = trackName;
            StartCoroutine(HideText(trackNameText, duration));
        }
        
        /// <summary>
        /// Method <c>HideText</c> hides any text.
        /// </summary>
        /// <param name="text">The text to be hidden.</param>
        /// <param name="duration">The duration of the text.</param>
        private IEnumerator HideText(TextMeshProUGUI text, float duration)
        {
            yield return new WaitForSeconds(duration);
            text.text = string.Empty;
        }

        /// <summary>
        /// Method <c>ShowBestRaceTime</c> updates the best race time text.
        /// </summary>
        /// <param name="time">The best race time.</param>
        public void ShowBestRaceTime(float time)
        {
            bestRaceTimeText.text = FloatToTime(time);
            bestRaceTimeText.transform.parent.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Method <c>ShowBestLapTime</c> updates the best lap time text.
        /// </summary>
        /// <param name="time">The best lap time.</param>
        public void ShowBestLapTime(float time)
        {
            bestLapTimeText.text = FloatToTime(time);
            bestLapTimeText.transform.parent.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Method <c>UpdateRaceTime</c> updates the race time text.
        /// </summary>
        /// <param name="time">The race time.</param>
        public void UpdateRaceTime(float time)
        {
            raceTimeText.text = FloatToTime(time);
        }
        
        /// <summary>
        /// Method <c>UpdateCurrentLapTime</c> updates the total time text.
        /// </summary>
        /// <param name="time">The total time.</param>
        public void UpdateCurrentLapTime(float time)
        {
            currentLapTimeText.text = FloatToTime(time);
        }
        
        /// <summary>
        /// Method <c>AddLapTime</c> adds a lap time to the lap time container.
        /// </summary>
        /// <param name="lapNumber">The lap number.</param>
        /// <param name="time">The lap time.</param>
        public void AddLapTime(int lapNumber, float time)
        {
            var lapTime = Instantiate(lapTimePrefab, lapTimeContainer.transform);
            lapTime.transform.Find("TimeText").GetComponent<TextMeshProUGUI>().text = $"Lap {lapNumber}";
            lapTime.transform.Find("TimeValue").GetComponent<TextMeshProUGUI>().text = FloatToTime(time);
        }
        
        /// <summary>
        /// Method <c>FloatToTime</c> converts a float to a time string.
        /// </summary>
        /// <param name="time">The time to be converted.</param>
        /// <returns>The time string.</returns>
        private string FloatToTime(float time)
        {
            var minutes = Mathf.FloorToInt(time / 60);
            var seconds = Mathf.FloorToInt(time % 60);
            var miliseconds = Mathf.FloorToInt((time * 100) % 100);

            // Format string with leading zeros
            return $"{minutes:00}:{seconds:00}:{miliseconds:0000}";
        }
        
        /// <summary>
        /// Method <c>TogglePauseMenu</c> toggles the pause menu.
        /// </summary>
        public void TogglePauseMenu()
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            if (pauseMenu.activeSelf)
                pauseFirstSelectedButton.Select();
        }
    }
}
