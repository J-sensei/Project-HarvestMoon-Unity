using SceneTransition;
using StateMachine.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Enemy
{
    public enum EnemyBehaviorState
    {
        Idle,
        Patrol,
        Chase,
    }

    /// <summary>
    /// Only for wolf?
    /// </summary>
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private EnemyBehaviorState enemyState = EnemyBehaviorState.Idle;
        [Tooltip("How many second to toggle the state behavior")]
        [SerializeField] private float behaviorTime = 4.5f;
        [Tooltip("How many second to find the path if in chasing state")]
        [SerializeField] private float pathfindInterval = 1f;
        [Tooltip("How close distance between enemy and player to be detect and chase the player")]
        [SerializeField] private float detectRange = 10f;
        [SerializeField] private float undetectRange = 20f;
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float runSpeed = 2.5f;

        #region Animation Hash
        private const string IDLE = "Idle";
        private const string WALK = "Walk";
        private const string RUN = "Run";
        private const string ROAR = "Roar";

        public int IdleHash { get; private set; }
        public int WalkHash { get; private set; }
        public int RunHash { get; private set; }
        public int RoarHash { get; private set; }
        #endregion

        private Animator _animator;
        private NavMeshAgent _agent;
        private float _behaviorTimer;
        private bool _roar = false;
        private float _pathfindTimer = 0f;
        private bool _pause = false;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            IdleHash = Animator.StringToHash(IDLE);
            WalkHash = Animator.StringToHash(WALK);
            RunHash = Animator.StringToHash(RUN);
            RoarHash = Animator.StringToHash(ROAR);

            _behaviorTimer = behaviorTime;
        }

        private void Start()
        {
            ChangeBehavior(enemyState);
            _agent.speed = walkSpeed;
            _agent.isStopped = true;
        }

        public void Disable()
        {
            _agent.enabled = false;
            _animator.speed = 0;
            _pause = true;
        }

        public void Enable()
        {
            _agent.enabled = true;
            _animator.speed = 1f;
            _pause = false;
        }

        private void Update()
        {
            if (_pause) return;

            switch (enemyState)
            {
                case EnemyBehaviorState.Idle:
                    Idle();
                    break;
                case EnemyBehaviorState.Patrol:
                    Patrol();
                    break;
                case EnemyBehaviorState.Chase:
                    Chase();
                    break;
            }
        }

        private void Idle()
        {
            _behaviorTimer -= Time.deltaTime;
            if(_behaviorTimer <= 0f)
            {
                _behaviorTimer = behaviorTime;
                ChangeBehavior(EnemyBehaviorState.Patrol);
            }

            CheckPlayer();
        }

        private void Patrol()
        {
            _animator.SetBool(WalkHash, true); // Make sure wolf is walking
            // Walk Roar to finish
            if (_roar && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !_animator.IsInTransition(0))
            {
                Vector3 location = Enemy.RandomNavSphere(_agent.transform.position, 10f, -1);
                _agent.SetDestination(location);
                _agent.isStopped = false;
                _roar = false;
                _animator.SetBool(RoarHash, false);
            }

            if(_agent.hasPath && _agent.remainingDistance <= 0.25f && !_agent.isStopped)
            {
                _agent.isStopped = true;
                ChangeBehavior(EnemyBehaviorState.Idle);
            }

            CheckPlayer();
        }

        private void Chase()
        {
            _animator.SetBool(WalkHash, true);
            _animator.SetBool(RunHash, true);

            if(_pathfindTimer <= 0f)
            {
                _agent.isStopped = false;
                _agent.SetDestination(GameManager.Instance.Player.transform.position);
                _pathfindTimer = pathfindInterval;
            }

            _pathfindTimer -= Time.deltaTime;
            Unchase();
        }

        private void ChangeBehavior(EnemyBehaviorState state)
        {
            switch (state)
            {
                case EnemyBehaviorState.Idle:
                    _agent.speed = walkSpeed;
                    _behaviorTimer = behaviorTime;
                    _animator.SetBool(WalkHash, false);
                    _animator.SetBool(IdleHash, true);
                    _animator.SetBool(RunHash, false);
                    break;
                case EnemyBehaviorState.Patrol:
                    _agent.speed = walkSpeed;
                    _animator.SetBool(RoarHash, true);
                    _animator.SetBool(RunHash, false);
                    _roar = true;
                    break;
                case EnemyBehaviorState.Chase:
                    _animator.SetBool(RoarHash, false);
                    _agent.speed = runSpeed;
                    break;
            }

            enemyState = state;
        }

        /// <summary>
        /// Check if player is near the enemy
        /// </summary>
        private void CheckPlayer()
        {
            if(GameManager.Instance.Player != null)
            {
                Vector3 playerDirection = GameManager.Instance.Player.transform.position - transform.position;
                if (playerDirection.magnitude > detectRange) return;

                // Use dot product to check if player is in front of the enemy AI
                Vector3 agentDirection = transform.forward; // Forward will be a normalized Vector3
                playerDirection.Normalize();
                float dotProduct = Vector3.Dot(playerDirection, agentDirection); // Get the dot product

                // When player is infront of the enemy (0f - 1f), behind is (-1f - 0f)
                // In this case we want make sure player is infront of the enemy
                if (dotProduct > 0f)
                {
                    Debug.Log("Chase Player!");
                    ChangeBehavior(EnemyBehaviorState.Chase);
                }
            }
        }

        private void Unchase()
        {
            Vector3 playerDirection = GameManager.Instance.Player.transform.position - transform.position;
            if(playerDirection.magnitude > undetectRange)
            {
                _pathfindTimer = 0f;
                _agent.isStopped = true;
                ChangeBehavior(EnemyBehaviorState.Idle);
            }
        }

        public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

            randomDirection += origin;

            NavMeshHit navHit;

            NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

            return navHit.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out PlayerStateMachine player))
            {
                // TOOD: Enter Battle scene
                Debug.Log("Touch player");
                GameManager.Instance.EnterCombat();
            }
        }
    }
}
