using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        /// <summary>
        /// Hash value for walkig animation
        /// </summary>
        private int _walkingAnimationHash;
        /// <summary>
        /// Hash value for running animation
        /// </summary>
        private int _runningAnimationHash;
        /// <summary>
        /// Hash value for jumping animation
        /// </summary>
        private int _jumpingAnimationHash;

        /// <summary>
        /// Unity new input system (Class is genenerated by the InputAction)
        /// </summary>
        private InputControls _inputControls;
        /// <summary>
        /// Character controller reference to move the character
        /// </summary>
        private CharacterController _characterController;
        /// <summary>
        /// Animator reference
        /// </summary>
        private Animator _animator;

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
        /// Is the movement input is pressed by the player
        /// </summary>
        private bool _moveInputPress;
        /// <summary>
        /// Is the character running
        /// </summary>
        private bool _isRunning;
        #endregion

        #region Jump
        /// <summary>
        /// Jump input is press
        /// </summary>
        private bool _jumpInputPress = false;
        /// <summary>
        /// Character is jumping
        /// </summary>
        private bool _isJumping;
        /// <summary>
        /// Initial jump velocity when jump button is press
        /// </summary>
        float initialJumpVelocity;
        /// <summary>
        /// Maximum jump height player can jump
        /// </summary>
        float _maxJumpHeight = 2f;
        /// <summary>
        /// Maximum jump time player can hold the button (seconds)
        /// </summary>
        float _maxJumpTime = 0.8f;
        /// <summary>
        /// Is jumping animation playing
        /// </summary>
        bool _isJumpingAnimating = false;
        #endregion

        // Placeholder
        // Player rotation factor per frame
        /// <summary>
        /// Rotation speed when moving
        /// </summary>
        float _rotationFactor = 7.0f;
        /// <summary>
        /// Multiplier for running
        /// </summary>
        float _runMultiplier = 3.0f;
        /// <summary>
        /// Gravity force pulling down the player
        /// </summary>
        float _gravity = -1119.8f;
        /// <summary>
        /// Gravity force pulling down the player when grounded
        /// </summary>
        float _groundeGravity = -0.05f;
        /// <summary>
        /// Multiplier multiply the gravity when falling down
        /// </summary>
        float _fallMultiplier = 2.0f;
        /// <summary>
        /// Maximum falling speed allow for the player
        /// </summary>
        float _maximumFallingSpeed = 20.0f;

        #region Debug Properties
        /// <summary>
        /// Final move vector3
        /// </summary>
        public Vector3 Move { get; private set; }
        public Vector3 CurrentMove { get { return _currentMovement; } }
        public bool IsFalling { get; private set; }
        #endregion

        private void Awake()
        {
            _inputControls = new(); // Initialize input controls (Input asset created by the developer)
            _inputControls.CharacterControls.Move.started += OnInputMove;
            _inputControls.CharacterControls.Move.canceled += OnInputMove;
            _inputControls.CharacterControls.Move.performed += OnInputMove;

            _inputControls.CharacterControls.Run.started += OnInputRun;
            _inputControls.CharacterControls.Run.canceled += OnInputRun;

            _inputControls.CharacterControls.Jump.started += OnInputJump;
            _inputControls.CharacterControls.Jump.canceled += OnInputJump;

            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            if(_animator == null) _animator = GetComponentInChildren<Animator>();

            _walkingAnimationHash = Animator.StringToHash("Walking");
            _runningAnimationHash = Animator.StringToHash("Running");
            _jumpingAnimationHash = Animator.StringToHash("Jumping");

            InitJumpVariables();
        }

        private void Update()
        {
            Vector3 move = _currentMovement * Time.deltaTime;
            if (_isRunning)
            {
                move.x = move.x *= _runMultiplier;
                move.z = move.z *= _runMultiplier;
            }
            Move = move;
            _characterController.Move(move);

            Gravity();
            Jump();
            PlayerRotation();
            PlayerAnimation();
        }

        private void OnEnable()
        {
            _inputControls.Enable(); // Enable to listen to the events
        }

        private void OnDisable()
        {
            _inputControls.Disable(); // Disable when object is not active to not listen to any events
        }

        private void InitJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2f;
            _gravity = (-2f * _maxJumpHeight) / Mathf.Pow(timeToApex, 2f);
            initialJumpVelocity = (2f * _maxJumpHeight) / timeToApex;
        }

        private void Jump()
        {
            if(!_isJumping && _characterController.isGrounded && _jumpInputPress)
            {
                _animator.SetBool(_jumpingAnimationHash, true);
                _isJumpingAnimating = true;
                _isJumping = true;
                _currentMovement.y = initialJumpVelocity * 0.5f;
            }else if(!_jumpInputPress && _isJumping && _characterController.isGrounded)
            {
                _isJumping = false;
            }
        }

        private void OnInputMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            _currentMovement = new Vector3(_moveInput.x, _currentMovement.y, _moveInput.y);
            _moveInputPress = _moveInput.x != 0 || _moveInput.y != 0;
        }

        private void OnInputRun(InputAction.CallbackContext context)
        {
            _isRunning = context.ReadValueAsButton();
        }

        private void OnInputJump(InputAction.CallbackContext context)
        {
            _jumpInputPress = context.ReadValueAsButton();
        }

        private void PlayerAnimation()
        {
            bool walking = _animator.GetBool(_walkingAnimationHash);
            bool running = _animator.GetBool(_runningAnimationHash);

            if(!walking && _moveInputPress)
            {
                _animator.SetBool(_walkingAnimationHash, true);
            }
            else if(walking && !_moveInputPress)
            {
                _animator.SetBool(_walkingAnimationHash, false);
            }

            if((_moveInputPress && _isRunning) && !running)
            {
                _animator.SetBool(_runningAnimationHash, true);
            }
            else if((!_moveInputPress || !_isRunning) && running)
            {
                _animator.SetBool(_runningAnimationHash, false);
            }
        }

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

        private void Gravity()
        {
            IsFalling = _currentMovement.y <= 0f || !_jumpInputPress;

            // Reset from jumping animation
            if (_characterController.isGrounded)
            {
                if (_isJumpingAnimating)
                {
                    _animator.SetBool(_jumpingAnimationHash, false);
                    _isJumpingAnimating = false;
                }
                _currentMovement.y = _groundeGravity; // Make the current movement of y to grounded gravity
            }
            else if (IsFalling)
            {
                float prevYVel = _currentMovement.y;
                float newYVel = _currentMovement.y + (_gravity * _fallMultiplier * Time.deltaTime);
                float nextYVel = Mathf.Max((prevYVel + newYVel) * 0.5f, -_maximumFallingSpeed);
                _currentMovement.y = nextYVel;
            }
            else
            {
                float prevYVel = _currentMovement.y;
                float newYVel = _currentMovement.y + (_gravity * Time.deltaTime);
                float nextYVel = (prevYVel + newYVel) * 0.5f;
                _currentMovement.y = nextYVel;
            }
        }
    }

}