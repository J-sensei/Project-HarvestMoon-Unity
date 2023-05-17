using UnityEngine;

namespace Player
{
    public class PlayerParticleController : MonoBehaviour
    {
        [SerializeField] private Transform leftFoot;
        [SerializeField] private Transform rightFoot;
        [SerializeField] private ParticleSystem walkSmoke;
        [SerializeField] private ParticleSystem landSmoke;

        public void PlaySmokeLeftFoot()
        {
            Instantiate(walkSmoke, leftFoot.position, Quaternion.identity);
        }

        public void PlaySmokeRightFoot()
        {
            Instantiate(walkSmoke, rightFoot.position, Quaternion.identity);
        }

        public void PlayLandSmoke()
        {
            Instantiate(landSmoke, transform.position, Quaternion.identity);
        }
    }
}
