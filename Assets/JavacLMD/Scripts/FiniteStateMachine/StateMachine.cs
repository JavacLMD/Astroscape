using System;
using System.Collections.Generic;

namespace JavacLMD.FiniteStateMachine
{


    public interface IStateMachine
    {
        IState CurrentState { get; }
        void SwitchState(object id);
        void AddState(IState state);
    }
    

    public interface IStateMachine<TStateID, TState> : IStateMachine where TState : IState
    {
        new TState CurrentState { get; }

        void SwitchState(TStateID id);
        void AddState(TState newState);
    }

    [Serializable]
    public class StateMachine<TStateID, TState> : IStateMachine<TStateID, TState> where TState : IState
    {
        public TState CurrentState { get; protected set; }
        IState IStateMachine.CurrentState => CurrentState;
        
        
        private Dictionary<object, TState> _statesDictionary;

        public void SwitchState(TStateID id)
        {
            if (_statesDictionary != null && _statesDictionary.TryGetValue(id, out TState targetState))
            {
                CurrentState?.ExitState();
                CurrentState = targetState;
                CurrentState.EnterState();
            }
        }

        public void AddState(TState state)
        {
            _statesDictionary ??= new Dictionary<object, TState>();
            if (!_statesDictionary.ContainsKey(state.ID))
            {
                state.SetStateMachine(this);
                _statesDictionary[state.ID] = state;
            }
        }

        void IStateMachine.SwitchState(object id)
        {
            if (id is TStateID targetId)
            {
                SwitchState(targetId);
            }
        }
        void IStateMachine.AddState(IState newState)
        {
            if (newState is TState newState1)
            {
                AddState(newState1);
            }
        }
        
    }

    [Serializable]
    public class StateMachine : StateMachine<object, IState>
    {
        
    }

}
