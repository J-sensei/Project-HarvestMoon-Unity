using UnityEngine;

namespace StateMachine.Player
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }

        public override void Enter()
        {
            Jump();
        }

        public override void Update()
        {
            Gravity();
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            // Set the jump animation to false as player not jumping
            this.Context.Animator.SetBool(this.Context.JumpingAnimationHash, false);
            //_context.IsJumpingAnimating = false;

            // Fix continue jumping when holding jump button
            if(this.Context.JumpInputPress)
            {
                this.Context.RequireJumpAgain = true;
            }
        }

        public override void InitializeSubState()
        {

        }

        public override void CheckSwitchState()
        {
            if(this.Context.CharacterController.isGrounded)
            {
                this.SwitchState(this.StateFactory.Grounded());
            }
        }

        private void Jump()
        {
            this.Context.Animator.SetBool(this.Context.JumpingAnimationHash, true);
            this.Context.IsJumping = true;
            this.Context.CurrentMovementY = this.Context.InitialJumpVelocity * 0.5f;
            this.Context.ApplyMovementY = this.Context.InitialJumpVelocity * 0.5f;
        }

        private void Gravity()
        {
            bool isFalling = this.Context.CurrentMovementY <= 0f || !this.Context.JumpInputPress;

            if (isFalling)
            {
                float prevYVel = this.Context.CurrentMovementY;
                float newYVel = this.Context.CurrentMovementY + (this.Context.Gravity * this.Context.FallMultiplier * Time.deltaTime);
                float nextYVel = Mathf.Max((prevYVel + newYVel) * 0.5f, -this.Context.MaximumFallingSpeed);

                // Apply falling
                this.Context.CurrentMovementY = nextYVel;
                this.Context.ApplyMovementY = nextYVel;
            }
            else
            {
                float prevYVel = this.Context.CurrentMovementY;
                float newYVel = this.Context.CurrentMovementY + (this.Context.Gravity * Time.deltaTime);
                float nextYVel = (prevYVel + newYVel) * 0.5f;

                // Apply falling
                this.Context.CurrentMovementY = nextYVel;
                this.Context.ApplyMovementY = nextYVel;
            }
        }
    }
}
