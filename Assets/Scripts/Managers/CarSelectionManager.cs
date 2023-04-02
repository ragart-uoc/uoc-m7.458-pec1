using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PEC1.Entities;
using UnityStandardAssets.Vehicles.Car;

namespace PEC1.Managers
{
    /// <summary>
    /// Class <c>CarSelectionManager</c> manages the car selection.
    /// </summary>
    public class CarSelectionManager : MonoBehaviour
    {
        /// <value>Property <c>_instance</c> represents the singleton instance of the class.</value>
        private static CarSelectionManager _instance;

        /// <value>Property <c>cars</c> represents the list of car continers.</value>
        public List<GameObject> cars;
        
        /// <value>Property <c>m_cars</c> represents the list of cars.</value>
        private List<SelectorCar> m_Cars;
        
        /// <value>Property <c>m_CurrentCar</c> represents the current car.</value>
        private int m_CurrentCar;
        
        /// <value>Property <c>carContainer</c> represents the car container.</value>
        public GameObject carContainer;

        /// <value>Property <c>carSelectionText</c> represents the car selection text.</value>
        public TextMeshProUGUI carSelectionText;
        
        /// <value>Property <c>m_GameManager</c> represents the GameManager instance.</value>
        private GameManager m_GameManager;

        /// <summary>
        /// Method <c>Awake</c> is called when the script instance is being loaded.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            _instance = this;
            
            // Get the GameManager instance
            m_GameManager = FindObjectOfType<GameManager>();
        }

        /// <summary>
        /// Method <c>Start</c> is called on the frame when a script is enabled just before any of the Update methods are called the first time.
        /// </summary>
        private void Start()
        {
            // Get the screens
            m_Cars = new List<SelectorCar>();
            foreach (GameObject carPrefab in cars)
            {
                var car = new SelectorCar
                {
                    Name = Regex.Replace(carPrefab.name.Replace("Car-", ""), "(\\B[A-Z])", " $1"),
                    CarPrefab = carPrefab
                };
                m_Cars.Add(car);
            }
            
            // Set the first car as active
            SetCar(0);
        }

        /// <summary>
        /// Method <c>SwitchCar</c> switches the car.
        /// </summary>
        public void SwitchCar()
        {
            var nextScreen = (m_CurrentCar + 1) % m_Cars.Count;
            SetCar(nextScreen);
        }
        
        /// <summary>
        /// Method <c>SetCar</c> sets the car.
        /// </summary>
        /// <param name="carIndex">The car index.</param>
        private void SetCar(int carIndex)
        {
            foreach (Transform child in carContainer.transform)
            {
                Destroy(child.gameObject);
            }
            m_CurrentCar = carIndex;
            carSelectionText.text = $"Car: {m_Cars[m_CurrentCar].Name}";
            m_GameManager.SetCarPrefab(m_Cars[m_CurrentCar].CarPrefab);
            var carInstance = Instantiate(m_Cars[m_CurrentCar].CarPrefab, carContainer.transform);
            carInstance.GetComponent<CarController>().enabled = false;
            carInstance.GetComponent<CarUserControl>().enabled = false;
            carInstance.GetComponent<CarAudio>().enabled = false;
            carInstance.GetComponent<Rigidbody>().useGravity = false;
        }
        
        /// <summary>
        /// Method <c>NextScreen</c> loads the next screen.
        /// </summary>
        public void NextScreen()
        {
            m_GameManager.LoadRaceScene();
        }
        
        /// <summary>
        /// Method <c>PreviousScreen</c> loads the previous screen.
        /// </summary>
        public void PreviousScreen()
        {
            SceneManager.LoadScene("ScreenSelection");
        }
    }
}
