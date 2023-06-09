namespace StateMachine.Player
{
    public class PlayerLiftDroppingState : PlayerBaseState
    {
        public PlayerLiftDroppingState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }
        public override void Enter()
        {
            InitializeSubState();
            this.Context.Animator.SetBool(this.Context.LiftFinishAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.LiftFinishAnimationHash, false); // Player are not finish lifting yet

            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;

            this.Context.CurrentMovementX = 0;
            this.Context.CurrentMovementZ = 0;
        }

        public override void Update()
        {
            if (this.Context.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !this.Context.Animator.IsInTransition(0) && this.Context.DroppingItem)
            {
                //this.Context.EquipController.DetachItem();
                this.Context.DroppingItem = false;
            }

            this.CheckSwitchState();
        }

        public override void Exit()
        {
            this.Context.Animator.SetBool(this.Context.LiftFinishAnimationHash, true); // Finish lift dropping
        }

        public override void InitializeSubState()
        {

        }

        public override void CheckSwitchState()
        {
            if (!this.Context.DroppingItem)
            {
                this.SwitchState(this.StateFactory.Grounded());
            }
        }
    }
}
