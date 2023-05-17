using UnityEngine;

namespace StateMachine.Player
{
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }

        public override void Enter()
        {
            this.InitializeSubState();

            // Polish stuff
            this.Context.AudioController.PlayAudio(this.Context.AudioController.FallHitGround);
            this.Context.ParticleController.PlayLandSmoke();

            this.Context.Animator.SetBool(this.Context.FallingAnimationHash, false);
            this.Context.CurrentMovementY = this.Context.Gravity;
            this.Context.ApplyMovementY = this.Context.Gravity;
            //this.Context.ApplyMovementY = this.Context.GroundedGravity;
            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;
            //this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            //this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);
            //this.InitializeSubState();
        }

        public override void Update()
        {
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            //this.Context.AudioController.PlayAudio(this.Context.AudioController.FallHitGround);
        }

        public override void InitializeSubState()
        {
            PlayerBaseState state;
            // Idle when character not moving / running
            if (!this.Context.MoveInputPress && !this.Context.RunInputPress)
            {
                state = this.StateFactory.Idle();
            }
            else if (this.Context.MoveInputPress && !this.Context.RunInputPress)
            {
                state = this.StateFactory.Walk();
            }
            else // Moving and Running
            {
                state = this.StateFactory.Run();
            }

            state.Enter(); // This to make sure the sub state enter can be call
            this.SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            // If player is grounded, pressing jump button will switch to jump state
            if(this.Context.JumpInputPress && !this.Context.RequireJumpAgain)
            {
                this.SwitchState(this.StateFactory.Jump()); // Switch to jump state
            } 
            else if(!this.Context.CharacterController.isGrounded)
            {
                this.SwitchState(this.StateFactory.Fall());
            }
            else if(this.Context.PickupInputPress) // Pick item is near the player 
            {
                // Switch pickup
                Debug.Log("Pickup");
            }
        }
    }
}
