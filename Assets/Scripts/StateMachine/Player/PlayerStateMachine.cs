using Item;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StateMachine.Player
{
    /// <summary>
    /// State machine of the player to control different behaviors
    /// </summary>
    public class PlayerStateMachine : MonoBehaviour
    {
        #region Animation
        private int _fallingAnimationHash;
        public int FallingAnimationHash { get { return _fallingAnimationHash; } }
        private int _walkingAnimationHash;
        /// <summary>
        /// Hash value for walkig animation
        /// </summary>
        public int WalkingAnimationHash { get { return _walkingAnimationHash; } }
        private int _runningAnimationHash;
        /// <summary>
        /// Hash value for running animation
        /// </summary>
        public int RunningAnimationHash { get { return _runningAnimationHash; } }
        private int _jumpingAnimationHash;
        /// <summary>
        /// Hash value for jumping animation
        /// </summary>
        public int JumpingAnimationHash { get { return _jumpingAnimationHash; } }
        #endregion

        /// <summary>
        /// Unity new input system (Class is genenerated by the InputAction)
        /// </summary>
        private InputControls _inputControls;
        /// <summary>
        /// Character controller reference to move the character
        /// </summary>
        private CharacterController _characterController;
        /// <summary>
        /// Character controller reference to move the character
        /// </summary>
        public CharacterController CharacterController { get { return _characterController; } }
        /// <summary>
        /// Animator reference
        /// </summary>
        private Animator _animator;
        /// <summary>
        /// Player animator reference
        /// </summary>
        public Animator Animator { get { return _animator; } }
        public PlayerAudioController AudioController { get; private set; }
        public PlayerParticleController ParticleController { get; private set; }
        /// <summary>
        /// Player interactor to have interaction with interactable objects such as farm and tools
        /// </summary>
        public PlayerInteractor PlayerInteractor { get; private set; }

        #region Move
        /// <summary>
        /// Player input values for the movement
        /// </summary>
        private Vector2 _moveInput;
        /// <summary>
        /// Current movement in Vector 3
        /// </summary>
        private Vector3 _currentMovement;
        /// <summary>
        /// Current movement of X-axis
        /// </summary>
        public float CurrentMovementX
        {
            get { return _currentMovement.x; }
            set { _currentMovement.x = value; }
        }
        /// <summary>
        /// Current movement of Y-axis
        /// </summary>
        public float CurrentMovementY 
        { 
            get { return _currentMovement.y; } 
            set { _currentMovement.y = value; }
        }
        /// <summary>
        /// Current movement of Z-axis
        /// </summary>
        public float CurrentMovementZ
        {
            get { return _currentMovement.z; }
            set { _currentMovement.z = value; }
        }

        /// <summary>
        /// Final movement vector3 that will apply to the player move
        /// </summary>
        private Vector3 _applyMovement;
        public float ApplyMovementX
        {
            get { return _applyMovement.x; }
            set { _applyMovement.x = value; }
        }
        public float ApplyMovementY
        {
            get { return _applyMovement.y; }
            set { _applyMovement.y = value; }
        }
        public float ApplyMovementZ
        {
            get { return _applyMovement.z; }
            set { _applyMovement.z = value; }
        }

        /// <summary>
        /// Is the movement input is pressed by the player
        /// </summary>
        private bool _moveInputPress;
        /// <summary>
        /// Is the movement input is pressed by the player
        /// </summary>
        public bool MoveInputPress { get { return _moveInputPress; } }
        /// <summary>
        /// Is the character running
        /// </summary>
        private bool _walkInputPress;
        /// <summary>
        /// Is the running input is press by the player
        /// </summary>
        public bool WalkInputPress { get { return _walkInputPress; } }
        #endregion

        #region Jump
        /// <summary>
        /// Jump input is press
        /// </summary>
        private bool _jumpInputPress = false;
        /// <summary>
        /// Jump input is press
        /// </summary>
        public bool JumpInputPress { get { return _jumpInputPress; } }
        /// <summary>
        /// Character is jumping
        /// </summary>
        private bool _isJumping;
        /// <summary>
        /// Character is jumping
        /// </summary>
        public bool IsJumping
        {
            get { return _isJumping; }
            set { _isJumping = value; }
        }
        /// <summary>
        /// Initial jump velocity when jump button is press
        /// </summary>
        private float _initialJumpVelocity;
        /// <summary>
        /// Initial jump velocity when jump button is press
        /// </summary>
        public float InitialJumpVelocity { get { return _initialJumpVelocity; } }

        /// <summary>
        /// Maximum jump height player can jump
        /// </summary>
        private float _maxJumpHeight = 2f;
        /// <summary>
        /// Maximum jump time player can hold the button (seconds)
        /// </summary>
        private float _maxJumpTime = 0.8f;
        /// <summary>
        /// After jump is finish, if playing is still holding jump button then this should set to true
        /// </summary>
        private bool _requireJumpAgain = false;
        /// <summary>
        /// After jump is finish, if playing is still holding jump button then this should set to true. To make sure player don't keep jumping by holding down the button
        /// </summary>
        public bool RequireJumpAgain 
        { 
            get { return _requireJumpAgain; } 
            set { _requireJumpAgain = value; }
        }
        #endregion

        // Placeholder
        // Player rotation factor per frame
        /// <summary>
        /// Rotation speed when moving
        /// </summary>
        private float _rotationFactor = 7.0f;
        /// <summary>
        /// Multiplier for running
        /// </summary>
        private float _runMultiplier = 3.0f;
        public float RunMultiplier { get { return _runMultiplier; } }

        #region Gravity
        /// <summary>
        /// Gravity force pulling down the player
        /// </summary>
        private float _gravity = -9.8f;
        /// <summary>
        /// Gravity force pulling down the player
        /// </summary>
        public float Gravity { get { return _gravity; } }
        public float JumpGravity { get; private set; }

        ///// <summary>
        ///// Gravity force pulling down the player when grounded
        ///// </summary>
        //private float _groundedGravity = -0.5f;
        ///// <summary>
        ///// Gravity force pulling down the player when grounded
        ///// </summary>
        //public float GroundedGravity { get { return _groundedGravity; } }

        /// <summary>
        /// Multiplier multiply the gravity when falling down
        /// </summary>
        private float _fallMultiplier = 2.0f;
        /// <summary>
        /// Multiplier multiply the gravity when falling down
        /// </summary>
        public float FallMultiplier { get { return _fallMultiplier; } }

        /// <summary>
        /// Maximum falling speed allow for the player
        /// </summary>
        private float _maximumFallingSpeed = 11.5f;
        /// <summary>
        /// Maximum falling speed allow for the player
        /// </summary>
        public float MaximumFallingSpeed { get { return _maximumFallingSpeed; } }
        #endregion

        #region Interaction
        private bool _pickupInputPress;
        /// <summary>
        /// Ket press when player is request to pickup something interactable
        /// </summary>
        public bool PickupInputPress 
        {
            get { return _pickupInputPress; }
            set { _pickupInputPress = value; }
        }
        private bool _lifting = false;
        public bool Lifting
        {
            get { return _lifting; }
            set { _lifting = value; }
        }

        private bool _interactInputPress;
        /// <summary>
        /// Key press when player is request to interact with something
        /// </summary>
        public bool InteractInputPress 
        { 
            get { return _interactInputPress; }
            set { _interactInputPress = value; }
        }
        /// <summary>
        /// Current slected item
        /// </summary>
        public PickableItem SelectedItem { get; set; }
        #endregion

        #region State Machine
        private PlayerBaseState _currentState;
        private PlayerStateFactory _stateFactory;

        public PlayerBaseState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }
        #endregion

        #region Debug Properties
        /// <summary>
        /// Final move vector3
        /// </summary>
        public Vector3 Move { get; private set; }
        public Vector3 CurrentMove { get { return _currentMovement; } }
        #endregion

        // Initialize values
        private void Awake()
        {
            _inputControls = new(); // Initialize input controls (Input asset created by the developer)

            // Subcribe input events to the input controls
            _inputControls.CharacterControls.Move.started += OnInputMove;
            _inputControls.CharacterControls.Move.canceled += OnInputMove;
            _inputControls.CharacterControls.Move.performed += OnInputMove;

            _inputControls.CharacterControls.Run.started += OnInputRun;
            _inputControls.CharacterControls.Run.canceled += OnInputRun;

            _inputControls.CharacterControls.Jump.started += OnInputJump;
            _inputControls.CharacterControls.Jump.canceled += OnInputJump;

            _inputControls.CharacterControls.Pickup.started += OnItemPickup;
            _inputControls.CharacterControls.Pickup.canceled += OnItemPickup;

            _inputControls.CharacterControls.Interact.started += OnInteract;
            _inputControls.CharacterControls.Interact.canceled += OnInteract;

            // Get component
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            if (_animator == null) _animator = GetComponentInChildren<Animator>();
            AudioController = GetComponentInChildren<PlayerAudioController>();
            ParticleController = GetComponentInChildren<PlayerParticleController>();
            PlayerInteractor = GetComponentInChildren<PlayerInteractor>();

            // Define animation hash
            _walkingAnimationHash = Animator.StringToHash("Walking");
            _runningAnimationHash = Animator.StringToHash("Running");
            _jumpingAnimationHash = Animator.StringToHash("Jumping");
            _fallingAnimationHash = Animator.StringToHash("Falling");

            InitializeJumpValues(); // Initialize jump variabels

            // Initialize state machine
            _stateFactory = new PlayerStateFactory(this);
            _currentState = _stateFactory.Grounded();
            _currentState.Enter();
        }

        private void Start()
        {
            _characterController.Move(_applyMovement * Time.deltaTime);
        }

        private void Update()
        {
            //if (_isRunning)
            //{
            //    move.x = move.x *= _runMultiplier;
            //    move.z = move.z *= _runMultiplier;
            //}
            //Move = move;
            _currentState.UpdateStates(); // Update the current state
            PlayerRotation();
            _characterController.Move(_applyMovement * Time.deltaTime);
        }

        private void OnEnable()
        {
            _inputControls.Enable(); // Enable to listen to the events
        }

        private void OnDisable()
        {
            _inputControls.Disable(); // Disable when object is not active to not listen to any events
        }

        private void InitializeJumpValues()
        {
            float timeToApex = _maxJumpTime / 2f;
            JumpGravity = (-2f * _maxJumpHeight) / Mathf.Pow(timeToApex, 2f);
            _initialJumpVelocity = (2f * _maxJumpHeight) / timeToApex;
        }

        #region New Input System callback
        private void OnInputMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>(); // Read the input values as vector2
            _currentMovement = new Vector3(_moveInput.x, _currentMovement.y, _moveInput.y); // Update the current movement
            _moveInputPress = _moveInput.x != 0 || _moveInput.y != 0; // Update the move input press boolean if any dimension is press
        }

        private void OnInputRun(InputAction.CallbackContext context)
        {
            _walkInputPress = context.ReadValueAsButton();
        }

        private void OnInputJump(InputAction.CallbackContext context)
        {
            _jumpInputPress = context.ReadValueAsButton();
            _requireJumpAgain = false; // Make it false when player is pressing the jump button
        }

        private void OnItemPickup(InputAction.CallbackContext context)
        {
            _pickupInputPress = context.ReadValueAsButton();
        }

        private void OnInteract(InputAction.CallbackContext context)
        {
            _interactInputPress = context.ReadValueAsButton();
        }
        #endregion

        /// <summary>
        /// Handle player rotation when moving
        /// </summary>
        private void PlayerRotation()
        {
            Vector3 posLookAt; // Look at position

            // Change the position to look at
            posLookAt = new Vector3(_currentMovement.x, 0f, _currentMovement.z);

            Quaternion currentRot = transform.rotation;
            if (_moveInputPress)
            {
                Quaternion targetRot = Quaternion.LookRotation(posLookAt);
                transform.rotation = Quaternion.Slerp(currentRot, targetRot, _rotationFactor * Time.deltaTime);
            }
        }
    }
}
