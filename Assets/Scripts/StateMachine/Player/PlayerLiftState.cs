namespace StateMachine.Player
{
    public class PlayerLiftState : PlayerBaseState
    {
        public PlayerLiftState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }
        public override void Enter()
        {
            InitializeSubState();
            // TODO: Set animator lifting to true
        }

        public override void Update()
        {
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            // TODO: Set lifting to false
        }

        public override void InitializeSubState()
        {
            // TODO: Set lifting to true and each sub state will set lifting animation to true

            PlayerBaseState state;
            // Idle when character not moving / running
            if (!this.Context.MoveInputPress && !this.Context.WalkInputPress)
            {
                state = this.StateFactory.Idle();
            }
            else if (this.Context.MoveInputPress && !this.Context.WalkInputPress)
            {
                state = this.StateFactory.Walk();
            }
            else // Moving and Running
            {
                state = this.StateFactory.Run();
            }

            state.Enter();
            this.SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            // TODO: !Grounded or After dropped item will change to grounded state

            //if (this.Context.CharacterController.isGrounded)
            //{
            //    this.SwitchState(this.StateFactory.Grounded());
            //}
        }
    }
}
