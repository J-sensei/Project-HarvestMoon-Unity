﻿using System.Collections.Generic;

namespace StateMachine.Player
{
    internal enum PlayerState
    {
        Idle, Walk, Run, Jump, Grounded, Fall, UsingTool, Lift, PickingItem, Dropping, Sleep
    }

    /// <summary>
    /// Control different states of the player
    /// </summary>
    public class PlayerStateFactory
    {
        /// <summary>
        /// Reference to the player state machine to aceess all the resources available
        /// </summary>
        private PlayerStateMachine _context;
        private Dictionary<PlayerState, PlayerBaseState> _states = new();

        public PlayerStateFactory(PlayerStateMachine context)
        {
            _context = context;
            _states[PlayerState.Idle] = new PlayerIdleState(_context, this);
            _states[PlayerState.Walk] = new PlayerWalkState(_context, this);
            _states[PlayerState.Run] = new PlayerRunState(_context, this);
            _states[PlayerState.Jump] = new PlayerJumpState(_context, this);
            _states[PlayerState.Grounded] = new PlayerGroundedState(_context, this);
            _states[PlayerState.Fall] = new PlayerFallState(_context, this);
            _states[PlayerState.UsingTool] = new PlayerUsingToolState(_context, this);
            _states[PlayerState.Lift] = new PlayerLiftState(_context, this);
            _states[PlayerState.PickingItem] = new PlayerPickingItemState(_context, this);
            _states[PlayerState.Dropping] = new PlayerLiftDroppingState(_context, this);
            _states[PlayerState.Sleep] = new PlayerSleepState(_context, this);
        }

        public PlayerBaseState Idle() { return _states[PlayerState.Idle]; }
        public PlayerBaseState Walk() { return _states[PlayerState.Walk]; }
        public PlayerBaseState Run() { return _states[PlayerState.Run]; }
        public PlayerBaseState Jump() { return _states[PlayerState.Jump]; }
        public PlayerBaseState Grounded() { return _states[PlayerState.Grounded]; }
        public PlayerBaseState Fall() { return _states[PlayerState.Fall]; }
        public PlayerBaseState UsingTool() { return _states[PlayerState.UsingTool]; }
        public PlayerBaseState Lift() { return _states[PlayerState.Lift]; }
        public PlayerBaseState PickingItem() { return _states[PlayerState.PickingItem]; }
        public PlayerBaseState DroppingItem() { return _states[PlayerState.Dropping]; }
        public PlayerBaseState Sleep() { return _states[PlayerState.Sleep]; }
    }
}
