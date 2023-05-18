using UnityEngine;

namespace StateMachine.Player
{
    /// <summary>
    /// State where player is playing tool animation
    /// </summary>
    public class PlayerUsingToolState : PlayerBaseState
    {
        public PlayerUsingToolState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {

        }

        public override void Enter()
        {
            this.Context.Animator.SetBool(this.Context.ToolAnimationHash, true); // Player is using tool
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);

            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;
        }

        public override void Update()
        {
            if(this.Context.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !this.Context.Animator.IsInTransition(0))
            {
                this.Context.UsingTool = false;
                this.Context.ToolEvent?.Invoke();
                this.Context.ToolEvent.RemoveAllListeners();
            }

            this.CheckSwitchState();
        }

        public override void Exit()
        {
            this.Context.Animator.SetBool(this.Context.ToolAnimationHash, false);
        }

        public override void InitializeSubState()
        {
            
        }

        public override void CheckSwitchState()
        {
            if (this.Context.UsingTool)
            {
                return;
            }

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

            this.SwitchState(state);
        }
    }
}
