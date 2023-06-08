using Environment;
using UnityEngine;

namespace Player
{
    public class PlayerGroundChecker : GroundChecker
    {
        [Header("Player Properties")]
        [SerializeField] private PlayerAudioController audioController;

        private void Start()
        {
            if(audioController == null)
            {
                audioController = GetComponent<PlayerAudioController>();
            }

            // Listen to the ground check event
            OnGroundCheck.AddListener(audioController.UpdateGroundType);
        }
    }
}
