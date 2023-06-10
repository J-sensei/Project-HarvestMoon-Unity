using Farming;
using Interactable;
using Inventory;
using UnityEngine;

namespace StateMachine.Player
{
    /// <summary>
    /// State when player standing on the ground
    /// </summary>
    public class PlayerGroundedState : PlayerBaseState
    {
        public PlayerGroundedState(PlayerStateMachine context, PlayerStateFactory stateFactory) : base(context, stateFactory)
        {
            this.IsRootState = true;
        }

        public override void Enter()
        {
            this.InitializeSubState();

            this.Context.Animator.SetBool(this.Context.FallingAnimationHash, false);
            this.Context.CurrentMovementY = this.Context.Gravity;
            this.Context.ApplyMovementY = this.Context.Gravity;

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
            PlayerBaseState state;
            if (this.Context.UsingTool)
            {
                state = this.StateFactory.UsingTool();
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
                    state = this.StateFactory.Run();
                }
                else // Pressing shift to walk
                {
                    state = this.StateFactory.Walk();
                }
            }

            // Bug found this is not call when not explicitly calling this,
            // just called the sub state enter to prevent sub state not enter!
            state.Enter(); // This to make sure the sub state enter can be call
            this.SetSubState(state);
        }

        public override void CheckSwitchState()
        {
            /*
                * Always check jumping and falling first as player cannot interact and do farming when jumping / falling
                * Pickup item has higher priority than farm interaction (e.g. item drop on farm land, player should able to pick it up)
            */
            // If player is grounded, pressing jump button will switch to jump state
            if(this.Context.JumpInputPress && !this.Context.RequireJumpAgain)
            {
                this.SwitchState(this.StateFactory.Jump()); // Switch to jump state
            } 
            else if(!this.Context.CharacterController.isGrounded)
            {
                this.SwitchState(this.StateFactory.Fall()); // Switch to fall state when not grounded
            }
            else if(this.Context.PickupInputPress) // Pick item is near the player 
            {
                this.Context.PickupInputPress = false; // Set to false to make sure it only trigger once
                OnPickupPress(this.Context.PlayerInteractor.SelectedtInteractable);
            }
            else if (this.Context.InteractInputPress)
            {
                this.Context.InteractInputPress = false; // Set to false to make sure it only trigger once
                OnInteractPress(this.Context.PlayerInteractor.SelectedtInteractable);
            }
        }

        /// <summary>
        /// Check when player press pickup key
        /// </summary>
        private void OnPickupPress(IInteractable interactable)
        {
            if (interactable == null) return; // No need to do anything if interactable object is null

            switch (interactable.GetInteractableType())
            {
                case InteractableType.Tool: case InteractableType.Item:
                    interactable.Interact();
                    if (interactable.GetInteractableType() == InteractableType.Item)
                    {
                        this.Context.PickingItem = true;
                        this.SwitchState(this.StateFactory.Lift());
                    }
                    break;
                case InteractableType.Crop:
                    interactable.Interact();

                    // Translate to lift state
                    this.Context.PickingItem = true;
                    this.SwitchState(this.StateFactory.Lift());
                    break;
            }
        }

        /// <summary>
        /// Check when player press interact key
        /// </summary>
        private void OnInteractPress(IInteractable interactable)
        {
            if (interactable == null) return; // No need to do anything if interactable object is null

            switch (interactable.GetInteractableType())
            {
                case InteractableType.Farm:
                    ItemData item = InventoryManager.Instance.GetHoldingItem(); // Get current holding item data
                    // Check what item is currently holding
                    if (item != null && item is ToolData)
                    {
                        FarmLand farm = this.Context.PlayerInteractor.SelectedtInteractable as FarmLand;
                        ToolData toolData = (ToolData)item;

                        // Check if the farm can be interact by the player tool
                        //if (farm.CheckTool(toolData.toolType)) // Redundant check
                        //{
                            this.Context.ToolEvent.AddListener(FarmEvent);
                        //}
                        this.Context.UsingTool = true; // This boolean will change to PlayerToolUsingState
                    }
                    else if (item != null && item is SeedData)
                    {
                        this.Context.ToolEvent.AddListener(PlantEvent);
                        this.Context.UsingTool = true; // This boolean will change to PlayerToolUsingState
                    }
                    break;
                case InteractableType.Environmental:
                    interactable.Interact(); // Use the environmental object
                    break;
            }
        }

        private void PlantEvent()
        {
            FarmLand farm = this.Context.PlayerInteractor.SelectedtInteractable as FarmLand;
            ItemData item = InventoryManager.Instance.GetHoldingItem();
            if (item == null) return;

            SeedData seedData = (SeedData)item;
            if (seedData != null)
            {
                farm.Plant(seedData);
            }
            else
            {
                Debug.LogWarning("[Player Ground State] Unable to cast item into seed data!");
            }
        }
        
        private void FarmEvent()
        {
            FarmLand farm = this.Context.PlayerInteractor.SelectedtInteractable as FarmLand;
            ItemData item = InventoryManager.Instance.GetHoldingItem();
            if (item == null) return;

            if (item != null && item is ToolData)
            {
                ToolData toolData = (ToolData)item;
                switch (toolData.toolType)
                {
                    case ToolData.ToolType.Hoe:
                        farm.Hoe();
                        break;
                    case ToolData.ToolType.WateringCan:
                        farm.Water();
                        break;
                    case ToolData.ToolType.Pickaxe: case ToolData.ToolType.Axe:
                        farm.RemoveCrop();
                        break;
                }
            }
        }
    }
}
