using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementController : MonoBehaviour
    {
        private int _walkingAnimationHash;
        private int _runningAnimationHash;
        private int _jumpingAnimationHash;

        private InputControls _inputControls;
        private CharacterController _characterController;
        private Animator _animator;

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
        private bool isMoveInputPressed;
        private bool isRunning;

        // Jumping
        private bool isJumpInputPressed = false;
        private bool isJumping;
        float initialJumpVelocity;
        float maxJumpHeight = 2f;
        float maxJumpTime = 0.8f;
        bool isJumpingAnimating = false;

        // Placeholder
        // Player rotation factor per frame
        float rotationFactor = 7.0f;
        float runMultiplier = 3.0f;
        float gravity = -9.8f;
        float groundeGravity = -0.05f;

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
            if (isRunning)
            {
                move.x = move.x *= runMultiplier;
                move.z = move.z *= runMultiplier;
            }
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
            float timeToApex = maxJumpTime / 2f;
            gravity = (-2f * maxJumpHeight) / Mathf.Pow(timeToApex, 2f);
            initialJumpVelocity = (2f * maxJumpHeight) / timeToApex;
        }

        private void Jump()
        {
            if(!isJumping && _characterController.isGrounded && isJumpInputPressed)
            {
                _animator.SetBool(_jumpingAnimationHash, true);
                isJumpingAnimating = true;
                isJumping = true;
                _currentMovement.y = initialJumpVelocity * 0.5f;
            }else if(!isJumpInputPressed && isJumping && _characterController.isGrounded)
            {
                isJumping = false;
            }
        }

        private void OnInputMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
            _currentMovement = new Vector3(_moveInput.x, 0, _moveInput.y);
            isMoveInputPressed = _moveInput.x != 0 || _moveInput.y != 0;
        }

        private void OnInputRun(InputAction.CallbackContext context)
        {
            isRunning = context.ReadValueAsButton();
        }

        private void OnInputJump(InputAction.CallbackContext context)
        {
            isJumpInputPressed = context.ReadValueAsButton();
        }

        private void PlayerAnimation()
        {
            bool walking = _animator.GetBool(_walkingAnimationHash);
            bool running = _animator.GetBool(_runningAnimationHash);

            if(!walking && isMoveInputPressed)
            {
                _animator.SetBool(_walkingAnimationHash, true);
            }
            else if(walking && !isMoveInputPressed)
            {
                _animator.SetBool(_walkingAnimationHash, false);
            }

            if((isMoveInputPressed && isRunning) && !running)
            {
                _animator.SetBool(_runningAnimationHash, true);
            }
            else if((!isMoveInputPressed || !isRunning) && running)
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
            if (isMoveInputPressed)
            {
                Quaternion targetRot = Quaternion.LookRotation(posLookAt);
                transform.rotation = Quaternion.Slerp(currentRot, targetRot, rotationFactor * Time.deltaTime);
            }
        }

        private void Gravity()
        {
            bool isFalling = _currentMovement.y <= 0f || !isJumpInputPressed;
            float fallMultiplier = 2f;
            if (_characterController.isGrounded)
            {
                if (isJumpingAnimating)
                {
                    _animator.SetBool(_jumpingAnimationHash, false);
                    isJumpingAnimating = false;
                }
                _currentMovement.y = groundeGravity;
            }
            else if (isFalling)
            {
                float prevYVel = _currentMovement.y;
                float newYVel = _currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
                float nextYVel = Mathf.Max((prevYVel + newYVel) * 0.5f, -20f);
                _currentMovement.y = nextYVel;
            }
            else
            {
                float prevYVel = _currentMovement.y;
                float newYVel = _currentMovement.y + (gravity * Time.deltaTime);
                float nextYVel = (prevYVel + newYVel) * 0.5f;
                _currentMovement.y = nextYVel;
            }
        }
    }

}