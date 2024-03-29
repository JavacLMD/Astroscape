using System;
using System.Collections.Generic;
using UnityEngine;

namespace JavacLMD.FiniteStateMachine
{

    public interface IState<TStateID>
    {
        TStateID ID { get; }
        void EnterState();
        void ExitState();

        void UpdateState();
        void LateUpdateState();
        void FixedUpdateState();

        internal void SetStateMachine(IStateMachine<TStateID> fsm);
    }

    [Serializable]
    public class State<TStateID> : IState<TStateID>
    {

        [SerializeField] private readonly TStateID _id;

        private IStateMachine<TStateID> _fsm;
        private List<Transition<TStateID>> _transitions;

        public TStateID ID => _id;
        
        public State(TStateID id)
        {
            _id = id;
        }

        public void EnterState()
        {
            Debug.Log($"Entered State {this.GetType().Name}: {ID}");
        }

        public void ExitState()
        {
            Debug.Log($"Exited State {this.GetType().Name}: {ID}");
        }

        public void UpdateState()
        {
            CheckTransitions();
            Debug.Log($"Update State {this.GetType().Name}: {ID}");
        }

        public void LateUpdateState()
        {
            Debug.Log($"Late Update State {this.GetType().Name}: {ID}");
        }

        public void FixedUpdateState()
        {
            Debug.Log($"Fixed Update State {this.GetType().Name}: {ID}");
        }


        public void AddTransition(Transition<TStateID> transition)
        {
            _transitions ??= new List<Transition<TStateID>>();
            if (!_transitions.Contains(transition)) 
                _transitions.Add(transition);
        }

        protected virtual void CheckTransitions()
        {
            foreach (var t in _transitions)
            {
                if (t.ShouldTransition())
                {
                    _fsm.SwitchState(t.TargetStateID);
                    return;
                }
            }
        }
        
        
        
        internal void SetStateMachine(IStateMachine<TStateID> fsm)
        {
            ((IState<TStateID>) this).SetStateMachine(fsm);
        }
        void IState<TStateID>.SetStateMachine(IStateMachine<TStateID> fsm)
        {
            _fsm = fsm;
        }
    }
    
}
