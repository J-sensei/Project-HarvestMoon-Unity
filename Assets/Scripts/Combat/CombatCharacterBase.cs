using Entity;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;
using QuickOutline;
using Utilities;
using UnityEngine.EventSystems;
using UI.Combat;

namespace Combat
{
    public enum CombatCharacterType
    {
        Player,
        Enemy
    }

    public enum CombatCharacterState
    {
        Idle,
        Attack,
        Hurt,
    }

    [RequireComponent(typeof(CharacterStatusBase))]
    public class CombatCharacterBase : MonoBehaviour, IPointerClickHandler
    {
        [Header("Configuration")]
        [Tooltip("Type of the combat character in the scene")]
        [SerializeField] private CombatCharacterType type = CombatCharacterType.Player;
        [Tooltip("Time of tween required for this character run to target position")]
        [SerializeField] private float runDuration = 1.5f;
        [Tooltip("Rotation speed of the tween")]
        [SerializeField] private float rotateDuration = 0.25f;
        [Tooltip("Rotation value for the character to face front (y-axis)")]
        [SerializeField] private float frontRotation = 0f;
        [Tooltip("Distance gap move to the target position")]
        [SerializeField] private float distanceGap = 1f;

        [Header("Reference")]
        [SerializeField] private CharacterStatusBase characterStatus;
        [SerializeField] private CombatAnimationController animationController;
        [SerializeField] private Animator animator;

        [Header("Particles")]
        [Tooltip("Spawn the particle when hit the target")]
        [SerializeField] private ParticleSystem hitParticle;

        [Header("Die")]
        [Tooltip("Time to destroy the gameobject after declared as die, negative will not delete it")]
        [SerializeField] private float timeToDestroy = -1f;

        #region Animation Hash
        private const string IDLE = "Idle";
        private const string RUN = "Run";
        private const string ATTACK = "Attack";
        private const string HURT = "Hurt";
        private const string DIE = "Die";

        public int IdleAnimationHash { get; private set; }
        public int RunAnimationHash { get; private set; }
        public int AttackAnimationHash { get; private set; }
        public int HurtAnimationHash { get; private set; }
        public int DieAnimationHash { get; private set; }
        #endregion

        private Vector3 _originalPos;
        private bool _attacking = false;
        private bool _attackAnimationPlay = false;
        private bool _hurtAnimationPlay = false;
        private CombatCharacterState state = CombatCharacterState.Idle;
        [SerializeField] private Outline _outline;
        private bool _selecting = false;
        private bool _die = false;

        public bool Attacking { get { return _attacking; } }
        public CharacterStatusBase CharacterStatus { get { return characterStatus; } }
        public CombatCharacterType Type { get { return type; } }

        private CharacterStatusBase _attackTarget;
        public bool Die { get { return _die; } }

        private void Awake()
        {
            IdleAnimationHash = Animator.StringToHash(IDLE);
            RunAnimationHash = Animator.StringToHash(RUN);
            AttackAnimationHash = Animator.StringToHash(ATTACK);
            HurtAnimationHash = Animator.StringToHash(HURT);
            DieAnimationHash = Animator.StringToHash(DIE);

            _originalPos = transform.position;

            if (characterStatus == null) characterStatus = GetComponent<CharacterStatusBase>();
            if (animator == null) animator = GetComponent<Animator>();
            if(animationController == null)
            {
                animationController = GetComponent<CombatAnimationController>();
                if (animationController == null) animationController = GetComponentInChildren<CombatAnimationController>();
            }

            if(type == CombatCharacterType.Enemy)
            {
                if(_outline == null) _outline = GetComponent<Outline>();
                //StartCoroutine(OutlineHelper.InitializeOutline(_outline));
            }

            animationController.OnAttack += OnAttack;
            animationController.OnHurtFinish += HurtFinish;
            characterStatus.OnDamage += OnHurt;
            characterStatus.OnDie += OnDie;
        }

        public void Select()
        {
            if(_outline != null)
            {
                _outline.enabled = true;
                _selecting = true;
            }
        }

        public void Deselect()
        {
            if(_outline != null)
            {
                _outline.enabled = false;
                _selecting = false;
            }
        }

        private void OnDie()
        {
            _die = true;
            animator.SetBool(DieAnimationHash, true);
            if(timeToDestroy > 0f)
            {
                Destroy(gameObject, timeToDestroy);
            }
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
                    state = CombatCharacterState.Idle;
                    transform.DORotate(new Vector3(0f, frontRotation), rotateDuration);
                });
            }
        }

        public IEnumerator CheckAnimationCompleted(string currentStateName, Action onComplete)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName(currentStateName) &&
                    animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                yield return null;

            if (onComplete != null)
                onComplete();
        }

        private void OnDisable()
        {
            animationController.OnAttack -= OnAttack;
            animationController.OnAttack -= HurtFinish;
            characterStatus.OnDamage -= OnHurt;
            characterStatus.OnDie -= OnDie;
        }

        private void OnHurt()
        {
            _hurtAnimationPlay = true;
            animator.SetBool(HurtAnimationHash, true);
            state = CombatCharacterState.Hurt;
            //StartCoroutine(CheckAnimationCompleted(HURT, () =>
            //{
            //    animator.SetBool(HurtAnimationHash, false);
            //    _hurtAnimationPlay = false;
            //    state = CombatCharacterState.Idle;
            //}));
        }

        public void HurtFinish()
        {
            animator.SetBool(HurtAnimationHash, false);
            _hurtAnimationPlay = false;
            state = CombatCharacterState.Idle;
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
            state = CombatCharacterState.Attack;
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

        #region IPointerEvents
        public void OnPointerClick(PointerEventData eventData)
        {
            if(eventData.button == PointerEventData.InputButton.Left && type == CombatCharacterType.Enemy)
            {
                Debug.Log("Click: " + name);
                CombatManager.Instance.Select(this);
            }
        }
        #endregion
    }
}
