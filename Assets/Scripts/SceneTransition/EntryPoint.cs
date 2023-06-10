using UnityEngine;
using Utilities;

namespace SceneTransition
{
    /// <summary>
    /// Point where scene switching will happen when playing step inside
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] SceneLocation location;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagCollection.PLAYER_TAG))
            {
                SceneTransitionManager.Instance.SwitchScene(location);
            }
        }
    }

}