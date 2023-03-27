using System;
using System.Collections.Generic;
using UnityEngine;

namespace JavacLMD.FiniteStateMachine
{


    public interface IStateMachine<TStateID>
    {
        IState<TStateID> CurrentState { get; }
        void SwitchState(TStateID id);
        void AddState(IState<TStateID> newState);
    }

    [Serializable]
    public class StateMachine<TStateID> : IStateMachine<TStateID>
    {
        [SerializeField] private IState<TStateID> _currentState;

        public IState<TStateID> CurrentState => _currentState;
        
        
        private Dictionary<TStateID, IState<TStateID>> _statesDictionary;

        public void SwitchState(TStateID id)
        {
            if (_statesDictionary != null && _statesDictionary.TryGetValue(id, out var targetState))
            {
                _currentState?.ExitState();
                _currentState = targetState;
                _currentState.EnterState();
            }
        }

        public void AddState(IState<TStateID> state)
        {
            _statesDictionary ??= new Dictionary<TStateID, IState<TStateID>>();
            if (!_statesDictionary.ContainsKey(state.ID))
            {
                state.SetStateMachine(this);
                _statesDictionary[state.ID] = state;
            }
        }
        
    }

    [Serializable]
    public class StateMachine : StateMachine<object>
    {
        
    }

}
