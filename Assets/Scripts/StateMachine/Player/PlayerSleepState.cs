using GameDateTime;
using SceneTransition;

namespace StateMachine.Player
{
    /// <summary>
    /// Force player go to sleep and proceed to next day
    /// </summary>
    public class PlayerSleepState : PlayerBaseState
    {
        private bool _isSleeping = true;
        private bool _isSleepStand = false;
        private bool _isSwitch = false;

        public PlayerSleepState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }

        public override void Enter()
        {
            this.Context.Sleeping = true;
            _isSleeping = true;
            _isSwitch = false;
            _isSleepStand = false;
            this.Context.Animator.SetBool(this.Context.SleepAnimationHash, true); // Set to sleep animation

            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;
        }

        public override void Exit()
        {
            this.Context.Sleeping = false;
            this.Context.Animator.SetBool(this.Context.SleepStandAnimationHash, true);
            this.Context.Animator.SetBool(this.Context.WalkingAnimationHash, false);
            this.Context.Animator.SetBool(this.Context.RunningAnimationHash, false);
        }

        public override void Update()
        {
            this.Context.ApplyMovementX = 0;
            this.Context.ApplyMovementZ = 0;

            if (_isSleeping && this.Context.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !this.Context.Animator.IsInTransition(0))
            {
                this.Context.Animator.SetBool(this.Context.SleepAnimationHash, false);
                this.Context.Animator.SetBool(this.Context.SleepStandAnimationHash, false);
                _isSleeping = false;

                // Called sleep
                SceneTransitionManager.Instance.SetSceneLocation(SceneLocation.MainMenu);
                SceneTransitionManager.Instance.SwitchScene(SceneLocation.Home); // Always back to home after a force sleep
                this.Context.PlayerStatus.Recover(0.5f); // As this if force sleep only recover half of the status
                GameTimeManager.Instance.Sleep(); // Sleep time
            }
            else if(!_isSleeping && !_isSleepStand && this.Context.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !this.Context.Animator.IsInTransition(0))
            {
                _isSleepStand = true;
            }

            this.CheckSwitchState();
        }

        public override void InitializeSubState()
        {
            
        }

        public override void CheckSwitchState()
        {
            if (_isSleepStand && !_isSwitch)
            {
                // 2 Seconds delay to make sure the animation is fully recover?
                this.SwitchState(this.StateFactory.Grounded()); // Switch back to grounded as player is finish sleep state
                //this.SwitchState(this.StateFactory.Grounded()); // Switch back to grounded as player is finish sleep state
                _isSwitch = true;
            }
        }
    }
}
