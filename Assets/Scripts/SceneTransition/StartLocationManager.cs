using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace SceneTransition
{
    /// <summary>
    /// Manage list of object should be spawn at when new scene is loaded
    /// </summary>
    public class StartLocationManager : Singleton<StartLocationManager>
    {
        [Tooltip("List of start points where player can spawn to when enter this scene, minimum required one to make it work")]
        [SerializeField] private List<StartPoint> startPoints = new List<StartPoint>();

        protected override void AwakeSingleton()
        {
            // TODO: Load objects from scene transition manager
        }

        public Transform GetTransform(SceneLocation from)
        {
            StartPoint point = startPoints.Find(x => x.enterFrom == from);
            return point.spawnPoint;
        }
    }
}
