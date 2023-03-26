using System;
using UnityEngine;

namespace JavacLMD.FiniteStateMachine
{

    public interface IState
    {
        object ID { get; }
        void EnterState();
        void ExitState();

        void UpdateState();
        void LateUpdateState();
        void FixedUpdateState();

        internal void SetStateMachine(IStateMachine fsm);
    }

    public interface IState<out TStateID> : IState
    {
        new TStateID ID { get; }
    }

    [Serializable]
    public class State<TStateID> : IState<TStateID>
    {

        [SerializeField] private readonly TStateID _id;
        private IStateMachine<TStateID, IState<TStateID>> _fsm;

        public TStateID ID => _id;
        object IState.ID => ID;
        
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

        void IState.SetStateMachine(IStateMachine fsm)
        {
            if (fsm is IStateMachine<TStateID, IState<TStateID>> finiteStateMachine)
            {
                _fsm = finiteStateMachine;
            }
            else
            {
                Debug.Log("");
            }
        }
    }
    
}
