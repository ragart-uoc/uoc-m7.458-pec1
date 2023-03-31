using UnityEngine;
using TMPro;

namespace PEC1
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
        
        /// <summary>
        /// Method <c>ShowMessage</c> shows a message on the screen.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="duration">The duration of the message.</param>
        public void ShowMessage(string message, float duration)
        {
            messageText.text = message;
            Invoke(nameof(HideMessage), duration);
        }
        
        /// <summary>
        /// Method <c>HideMessage</c> hides the message on the screen.
        /// </summary>
        private void HideMessage()
        {
            messageText.text = string.Empty;
        }
    }
}
