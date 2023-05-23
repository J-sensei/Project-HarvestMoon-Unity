using Farming;
using Interactable;
using Inventory;
using Item;
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
                this.SwitchState(this.StateFactory.Fall());
            }
            else if(this.Context.PickupInputPress) // Pick item is near the player 
            {
                this.Context.PickupInputPress = false; // Set to false to make sure it only trigger once

                // TODO: Switch to lift state as player gonna lifting something is valid
                Debug.Log("Player request pickup");
                if(this.Context.PlayerInteractor.SelectedItem != null)
                {
                    Debug.Log("Player pickup: " + this.Context.PlayerInteractor.SelectedItem.name);
                    PickableItem item = this.Context.PlayerInteractor.SelectedItem;
                    InventoryManager.Instance.Pickup(item.ItemData); // Updatee the holding item
                    item.OnPickup(); // Destroy it when picked up
                    // TODO: Go to lift state
                    // If its item then lift
                    if(InventoryManager.Instance.CheckHoldingItemType(ItemType.Item))
                    {
                        this.Context.PickingItem = true;
                        this.SwitchState(this.StateFactory.Lift());
                    }
                    // Else just equip the tool
                }
                else if(this.Context.PlayerInteractor.SelectedCrop != null)
                {
                    // Pickup the item for crop, harvest the crop!
                    Crop crop = this.Context.PlayerInteractor.SelectedCrop;
                    InventoryManager.Instance.Pickup(crop.YieldItem);
                    crop.Harvest();

                    // Translate to lift state
                    this.Context.PickingItem = true;
                    this.SwitchState(this.StateFactory.Lift());
                }
            }
            else if (this.Context.InteractInputPress)
            {
                this.Context.InteractInputPress = false; // Set to false to make sure it only trigger once

                // TODO: Interact with corresponding
                Debug.Log("Player request interact");
                if(this.Context.PlayerInteractor.SelectedFarmLand != null)
                {
                    // This will enter tool event
                    // The farm method will call when animation is finish playing
                    ItemData item = InventoryManager.Instance.HoldingItem;
                    if (item != null && item is ToolData)
                    {
                        FarmLand farm = this.Context.PlayerInteractor.SelectedFarmLand;
                        ToolData toolData = (ToolData)item;

                        // Check if the farm can be interact by the player tool
                        if(farm.CheckTool(toolData.toolType))
                        {
                            this.Context.ToolEvent.AddListener(Farm);                   
                        }
                        this.Context.UsingTool = true; // This boolean will change to PlayerToolUsingState
                    }
                    else if(item != null && item is SeedData)
                    {
                        this.Context.ToolEvent.AddListener(Plant);
                        this.Context.UsingTool = true; // This boolean will change to PlayerToolUsingState
                    }
                }
                // TODO: other interaction like bed
                else if (this.Context.PlayerInteractor.SelectedtInteractable != null)
                {
                    IInteractable interactable = this.Context.PlayerInteractor.SelectedtInteractable;
                    interactable.Interact();
                }
            }
        }

        private void Plant()
        {
            FarmLand farm = this.Context.PlayerInteractor.SelectedFarmLand;
            ItemData item = InventoryManager.Instance.HoldingItem;
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

        /// <summary>
        /// Change the farm state
        /// </summary>
        private void Farm()
        {
            FarmLand farm = this.Context.PlayerInteractor.SelectedFarmLand;
            ItemData item = InventoryManager.Instance.HoldingItem;
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
                }
            }
        }
    }
}
