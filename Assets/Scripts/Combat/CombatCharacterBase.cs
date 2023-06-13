using Entity;
using UnityEngine;
using DG.Tweening;

namespace Combat
{
    public enum CombatCharacterType
    {
        Player,
        Enemy
    }

    [RequireComponent(typeof(CharacterStatusBase))]
    public class CombatCharacterBase : MonoBehaviour
    {
        [SerializeField] private CombatCharacterType type = CombatCharacterType.Player;
        [SerializeField] private CharacterStatusBase characterStatus;
        [SerializeField] private CombatAttackController attackController;
        [SerializeField] private Animator animator;
        [Tooltip("Time of tween required for this character run to target position")]
        [SerializeField] private float runDuration = 1.5f;
        [SerializeField] private float rotateDuration = 0.25f;
        [SerializeField] private float frontRotation = 0f;

        [SerializeField] private float distanceGap = 1f;

        [Header("Particles")]
        [SerializeField] private ParticleSystem hitParticle;
        #region Animation Hash
        private const string IDLE = "Idle";
        private const string RUN = "Run";
        private const string ATTACK = "Attack";

        public int IdleAnimationHash { get; private set; }
        public int RunAnimationHash { get; private set; }
        public int AttackAnimationHash { get; private set; }
        #endregion

        private Vector3 _originalPos;
        private bool _attacking = false;
        private bool _attackAnimationPlay = false;

        public bool Attacking { get { return _attacking; } }
        public CharacterStatusBase CharacterStatus { get { return characterStatus; } }
        public CombatCharacterType Type { get { return type; } }

        private CharacterStatusBase _attackTarget;

        private void Awake()
        {
            IdleAnimationHash = Animator.StringToHash(IDLE);
            RunAnimationHash = Animator.StringToHash(RUN);
            AttackAnimationHash = Animator.StringToHash(ATTACK);

            _originalPos = transform.position;

            if (characterStatus == null) characterStatus = GetComponent<CharacterStatusBase>();
            if (animator == null) animator = GetComponent<Animator>();
            if(attackController == null)
            {
                attackController = GetComponent<CombatAttackController>();
                if (attackController == null) attackController = GetComponentInChildren<CombatAttackController>();
            }

            attackController?.Initialize(OnAttack);
        }

        private void Update()
        {
            // Wait animation finish and then go back
            if(_attackAnimationPlay && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !animator.IsInTransition(0))
            {
                // Affect target character status

                _attackAnimationPlay = false;
                animator.SetBool(AttackAnimationHash, false); // Stop attack animation
                // Run back to orignal position
                // Rotate 180
                transform.DORotate(new Vector3(0f, frontRotation + 180), rotateDuration);
                transform.DOMove(_originalPos, runDuration).OnComplete(() => {
                    animator.SetBool(RunAnimationHash, false); // Stop run animation
                    _attacking = false;
                    transform.DORotate(new Vector3(0f, frontRotation), rotateDuration);
                });
            }
        }

        private void OnAttack()
        {
            if (_attackTarget != null)
            {
                characterStatus.Attack(_attackTarget);
                if (hitParticle != null)
                {
                    Instantiate(hitParticle, _attackTarget.transform.position, Quaternion.identity);
                }
                _attackTarget = null;
            }
            else
            {
                Debug.LogWarning("[Combat Character Base] Attack Target is null");
            }
        }

        public void Attack(CombatCharacterBase character)
        {
            _attacking = true;
            Vector3 pos = character.transform.position;
            pos.z -= distanceGap; // Distance

            _attackTarget = character.characterStatus; // Get the target status

            // Run
            animator.SetBool(RunAnimationHash, true); // Start run animation
            transform.DOMove(pos, runDuration).OnComplete(() =>
            {
                animator.SetBool(AttackAnimationHash, true); // Start attack animation
                _attackAnimationPlay = true;
            });
        }
    }
}
