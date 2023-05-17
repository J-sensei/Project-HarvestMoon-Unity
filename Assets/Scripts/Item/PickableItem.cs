using StateMachine.Player;
using UnityEngine;

namespace Item
{
    /// <summary>
    /// Item that can be pick and lift by the player
    /// </summary>
    public class PickableItem : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Set selected item
                PlayerStateMachine player = other.GetComponent<PlayerStateMachine>();
                player.SelectedItem = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // Unset selected item
                PlayerStateMachine player = other.GetComponent<PlayerStateMachine>();
                player.SelectedItem = null;
            }
        }
    }
}
