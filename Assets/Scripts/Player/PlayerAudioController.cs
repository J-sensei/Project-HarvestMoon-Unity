using Environment;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class PlayerAudioController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private AudioSource playerAudioSource;

        [Header("Jump & Fall audio")]
        [SerializeField] private AudioClip jump;
        [SerializeField] private AudioClip fallHitGround;

        [Header("Footstep audio")]
        [SerializeField] private GroundType groundStanding = GroundType.Default;
        [SerializeField] private FootstepAudio[] footstepAudios;
        private FootstepAudio _currentFootstep;

        public AudioClip Jump { get { return jump; } }
        public AudioClip FallHitGround { get { return fallHitGround; } }

        private void Awake()
        {
            _currentFootstep = footstepAudios[0];
        }

        /// <summary>
        /// Play a footstep audio (Used in animation event)
        /// </summary>
        public void PlayFootstep()
        {
            if (_currentFootstep.footsteps.Length > 0)
            {
                playerAudioSource.PlayOneShot(_currentFootstep.footsteps[Random.Range(0, _currentFootstep.footsteps.Length)]);
            }
        }

        /// <summary>
        /// Update footstep audio based on the ground type that player standing
        /// </summary>
        /// <param name="type"></param>
        public void UpdateGroundType(GroundType type)
        {
            if (groundStanding == type) return; // No need to do anything if the type is same

            groundStanding = type;
            FootstepAudio targetAudio = footstepAudios.Where(x => x.type == groundStanding).FirstOrDefault();
            if(targetAudio.footsteps != null)
            {
                _currentFootstep = targetAudio;
            }
        }

        /// <summary>
        /// Player audio using player audio source
        /// </summary>
        /// <param name="clip"></param>
        public void PlayAudio(AudioClip clip)
        {
            playerAudioSource.PlayOneShot(clip);
        }
    }
}
