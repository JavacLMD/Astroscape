using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JavacLMD.FSM
{

    public interface IStateMachine<TState> where TState : IState
    {
        TState CurrentState { get; }
        
        bool SwitchState(object id);
        bool SwitchState(TState targetState);
        
        IStateMachine<TState> AddState(TState state);
    }
    
    public sealed class StateMachine<TState> : IStateMachine<TState> where TState : IState
    {
        private Dictionary<object, TState> _stateDictionary;
        protected Dictionary<object, TState> StateDictionary
        {
            get
            {
                _stateDictionary ??= new Dictionary<object, TState>();
                return _stateDictionary;
            }
        }

        [SerializeField]
        private TState _currentState;
        public TState CurrentState
        {
            get => _currentState;
            private set => _currentState = value;
        }


        public IStateMachine<TState> AddState(TState state)
        {
            if (!StateDictionary.ContainsKey(state.ID))
            {
                state.FSM = this as IStateMachine<IState>;
                StateDictionary.Add(state.ID, state);
            }

            return this;
        }

        public bool SwitchState(object id)
        {
            if (StateDictionary.TryGetValue(id, out var targetState))
            {
                return SwitchState(targetState);
            }
            return false;
        }

        public bool SwitchState(TState targetState)
        {
            CurrentState?.ExitState();
            CurrentState = targetState;
            CurrentState.EnterState();

            return CurrentState.Equals(targetState);
        }

        void UpdateState()
        {
            CurrentState?.UpdateState();
        }
        
        void LateUpdateState()
        {
            CurrentState?.LateUpdateState();
        }

        void FixedUpdateState()
        {
            CurrentState?.FixedUpdateState();
        }
        
        
    }
}
