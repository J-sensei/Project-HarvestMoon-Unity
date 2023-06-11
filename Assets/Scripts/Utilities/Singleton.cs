using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Singleton template to enable singleton behavior of a class
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [Header("Singleton")]
        [Tooltip("Do not destroy the target object when loading a new scene")]
        [SerializeField] private bool dontDestroyOnLoad;

        /// <summary>
        /// Instance of the singleton object
        /// </summary>
        private static T instance;
        /// <summary>
        /// Instance of the singleton object
        /// </summary>
        public static T Instance { get { return instance; } }

        private void Awake()
        {
            // If instance is null, assigned reference to it
            if (instance == null)
            {
                instance = this as T;

                // Check if don't destroy on load is enable, meaning the object will take to next scene
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
                AwakeSingleton(); // Trigger the singleton awake function
            }
            else
            {
                // Destroy the object if the instance is already available
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Events to trigger when create an instance, this is called in awake function
        /// </summary>
        protected abstract void AwakeSingleton();
    }
}
