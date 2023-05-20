using UnityEngine;

namespace StateMachine.Player
{
    public class PlayerFallState : PlayerBaseState
    {
        public PlayerFallState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }
        public override void Enter()
        {
            InitializeSubState();
            this.Context.Animator.SetBool(this.Context.FallingAnimationHash, true);
        }

        public override void Update()
        {
            Gravity();
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            // Polish stuff
            this.Context.AudioController.PlayAudio(this.Context.AudioController.FallHitGround);
            this.Context.ParticleController.PlayLandSmoke();
        }

        public override void InitializeSubState()
        {
            PlayerBaseState state;
            // Idle when character not moving / running
            if (!this.Context.MoveInputPress && !this.Context.WalkInputPress)
            {
                state = this.StateFactory.Idle();
            }
            else if (this.Context.MoveInputPress && !this.Context.WalkInputPress)
            {
                state = this.StateFactory.Run();
            }
            else // Pressing shift to walk
            {
                state = this.StateFactory.Walk();
            }

            state.Enter(); // This to make sure the sub state enter can be call
            this.SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            if (this.Context.CharacterController.isGrounded)
            {
                this.SwitchState(this.StateFactory.Grounded());
            }
        }

        private void Gravity()
        {
            float prevYVel = this.Context.CurrentMovementY;
            this.Context.CurrentMovementY = this.Context.CurrentMovementY + this.Context.Gravity * Time.deltaTime;
            this.Context.ApplyMovementY = Mathf.Max((prevYVel + Context.CurrentMovementY) * 0.5f, -this.Context.MaximumFallingSpeed);
        }
    }
}
