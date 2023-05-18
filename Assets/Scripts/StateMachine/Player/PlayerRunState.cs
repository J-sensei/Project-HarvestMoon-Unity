using UnityEngine;

namespace StateMachine.Player
{
    public class PlayerRunState : PlayerBaseState
    {
        public PlayerRunState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {

        }
        public override void Enter()
        {
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, true);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, true);
        }

        public override void Update()
        {
            this.Context.ApplyMovementX = this.Context.CurrentMovementX * this.Context.RunMultiplier;
            this.Context.ApplyMovementZ = this.Context.CurrentMovementZ * this.Context.RunMultiplier;
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            
        }

        public override void InitializeSubState()
        {
            
        }

        public override void CheckSwitchState()
        {
            if (this.Context.UsingTool)
            {
                this.SwitchState(this.StateFactory.UsingTool());
                return;
            }

            if (!this.Context.MoveInputPress)
            {
                this.SwitchState(this.StateFactory.Idle());
            }
            else if(this.Context.MoveInputPress && this.Context.WalkInputPress)
            {
                this.SwitchState(this.StateFactory.Walk());
            }
        }
    }
}
