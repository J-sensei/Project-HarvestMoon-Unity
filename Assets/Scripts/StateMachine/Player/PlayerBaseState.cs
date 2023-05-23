using UnityEngine;

namespace StateMachine.Player
{
    /// <summary>
    /// Base class for player state, contains generic functions for state machine
    /// </summary>
    public abstract class PlayerBaseState
    {
        private bool _isRootState = false;
        /// <summary>
        /// Player state machine context to access alll the resources of the player
        /// </summary>
        private PlayerStateMachine _context;
        /// <summary>
        /// State factory reference
        /// </summary>
        private PlayerStateFactory _stateFactory;

        private PlayerBaseState _currentSuperState;
        private PlayerBaseState _currentSubState;

        protected bool IsRootState { set { _isRootState = value; } }
        protected PlayerStateMachine Context { get { return _context; } }
        protected PlayerStateFactory StateFactory { get { return _stateFactory; } }

        // Debug
        public PlayerBaseState CurrentSubState { get { return _currentSubState; } }

        public PlayerBaseState(PlayerStateMachine context, PlayerStateFactory stateFactory)
        {
            _context = context;
            _stateFactory = stateFactory;
        }

        /// <summary>
        /// Call when enter the state
        /// </summary>
        public abstract void Enter();
        /// <summary>
        /// Updating the state if its set to current state
        /// </summary>
        public abstract void Update();
        /// <summary>
        /// Call when exit the state 
        /// </summary>
        public abstract void Exit();
        /// <summary>
        /// Check when the state should be change to other state
        /// </summary>
        public abstract void CheckSwitchState();
        public abstract void InitializeSubState();

        /// <summary>
        /// Update both state and sub state
        /// </summary>
        public void UpdateStates() 
        {
            Update();
            if(_currentSubState != null)
            {
                _currentSubState.Update();
            }
        }

        /// <summary>
        /// Exit both state and sub state
        /// </summary>
        public void ExitStates()
        {
            Exit();
            if(_currentSubState != null)
            {
                _currentSubState.Exit();
            }
        }

        /// <summary>
        /// Make transition to the other state
        /// </summary>
        /// <param name="state">New State</param>
        protected void SwitchState(PlayerBaseState state) 
        {
            // Call exit state as this state is about to exit
            ExitStates();

            // Enter the new state
            state.Enter();

            // If its root state (Top level in the state hierachy) then only set the current state of the state machine to change it
            if(_isRootState)
            {
                // Change the current state of the context
                _context.CurrentState = state;
            }
            else if(_currentSuperState != null)
            {
                _currentSuperState.SetSubState(state);
            }
        }

        public void Switch(PlayerBaseState state)
        {
            SwitchState(state);
        }

        /// <summary>
        /// Set only parent state
        /// </summary>
        protected void SetSuperState(PlayerBaseState state) 
        {
            _currentSuperState = state;
        }

        /// <summary>
        /// Set a sub state in the base state, set both parent and child state 
        /// </summary>
        /// <param name="state">New sub state</param>
        protected void SetSubState(PlayerBaseState state)
        {
            _currentSubState = state;
            state.SetSuperState(this);
        }
    }
}
