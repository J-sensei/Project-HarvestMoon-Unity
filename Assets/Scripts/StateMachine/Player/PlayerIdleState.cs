namespace StateMachine.Player
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {

        }

        public override void Enter()
        {
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);

            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;
        }

        public override void Update()
        {
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

            // Player is running if both key is press
            if (this.Context.MoveInputPress && this.Context.WalkInputPress)
            {
                this.SwitchState(this.StateFactory.Run());
            }
            else if(this.Context.MoveInputPress) // Player will just walking
            {
                this.SwitchState(this.StateFactory.Walk());
            }
        }
    }
}
