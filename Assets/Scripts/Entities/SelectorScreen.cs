using UnityEngine;

namespace PEC1.Entities
{
    /// <summary>
    /// Class <c>SelectorScreen</c> represents a selector screen.
    /// </summary>
    public class SelectorScreen
    {
        /// <value>Property <c>Name</c> represents the name of the screen.</value>
        public string Name { get; set; }
        
        /// <value>Property <c>SceneName</c> represents the name of the scene.</value>
        public string SceneName { get; set; }
        
        /// <value>Property <c>ScreenObject</c> represents the screen object.</value>
        public GameObject ScreenObject { get; set; }
    }
}
