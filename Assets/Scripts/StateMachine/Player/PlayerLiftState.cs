using Inventory;

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
            // TODO: Set animator lifting to true
            this.Context.Animator.SetBool(this.Context.LiftingAnimationHash, true);
            InitializeSubState();
        }

        public override void Update()
        {
            this.CheckSwitchState();
        }

        public override void Exit()
        {
            // TODO: Set lifting to false
            this.Context.Animator.SetBool(this.Context.LiftingAnimationHash, false);

            // TODO: Drop the item
            // Note: Item drop is tied to animation event now
            //this.Context.EquipController.DetachItem();
        }

        public override void InitializeSubState()
        {
            // TODO: Set lifting to true and each sub state will set lifting animation to true

            PlayerBaseState state;

            if (this.Context.PickingItem)
            {
                state = this.StateFactory.PickingItem();
            }
            else
            {
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
            }

            state.Enter();
            this.SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            // Not Grounded or After dropped item will change to grounded state
            if(!InventoryManager.Instance.CheckHoldingItemType(ItemType.Item))
            {
                this.SwitchState(this.StateFactory.Grounded());
            }
            else if (!this.Context.CharacterController.isGrounded)
            {
                this.Context.EquipController.DetachItem(); // TODO: Check if this work properly
                this.SwitchState(this.StateFactory.Fall());
            }
            else if (this.Context.PickupInputPress && !this.Context.PickingItem) // Prevent bug happen when pressing pickup when picking an item
            {
                //this.Context.PickingItem = true; // Use same variable for dropping
                this.Context.DroppingItem = true;
                this.Context.PickupInputPress = false; // Set to false to make sure it only trigger once
                this.SwitchState(this.StateFactory.DroppingItem());
            }
        }
    }
}
