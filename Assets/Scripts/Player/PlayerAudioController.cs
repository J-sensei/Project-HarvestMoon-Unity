using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private List<AudioClip> footsteps = new();
    [SerializeField] private AudioClip jump;
    [SerializeField] private AudioClip fallHitGround;

    public AudioClip Jump { get { return jump; } }
    public AudioClip FallHitGround { get { return fallHitGround; } }

    public void PlayFootstep()
    {
        if(footsteps.Count > 0)
        {
            playerAudioSource.PlayOneShot(footsteps[Random.Range(0, footsteps.Count - 1)]);
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        playerAudioSource.PlayOneShot(clip);
    }
}
