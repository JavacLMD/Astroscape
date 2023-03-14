using System;
using System.Collections.Generic;
using JavacLMD.FiniteStateMachine;

namespace JavacLMD.FSM
{

    public interface IState
    {
        object ID { get; }
        /// <summary>
        /// The state's parent StateMachine
        /// </summary>
        IStateMachine<IState> FSM { get; set; }
  
        void EnterState();
        void ExitState();
        void UpdateState();
        void LateUpdateState();
        void FixedUpdateState();
    }
    
    public class State : IState
    {
        private Action _onStateEnter;
        private Action _onStateExit;
        private Action _onStateUpdate;
        private Action _onStateLateUpdate;
        private Action _onStateFixedUpdate;

        private List<Transition> _transitions;
        private Dictionary<object, List<Transition>> _triggerToTransitions;
        
        private IStateMachine<IState> _fsm;
        private object _id;
        public object ID
        {
            get => _id;
            internal set => _id = value;
        }

        /// <summary>
        /// State's parent FSM
        /// </summary>
        public IStateMachine<IState> FSM
        {
            get => _fsm;
            set => _fsm = value;
        }

        public State(object id, Action onEnterLogic = null, Action onExitLogic = null, Action onUpdateLogic = null, Action onLateUpdateLogic = null,
            Action onFixedUpdateLogic = null)
        {
            _id = id;
            _onStateEnter = onEnterLogic;
            _onStateExit = onExitLogic;
            _onStateUpdate = onUpdateLogic;
            _onStateLateUpdate = onLateUpdateLogic;
            _onStateFixedUpdate = onFixedUpdateLogic;
        }


        void IState.EnterState() => OnStateEnter();
        void IState.ExitState() => OnStateExit();

        void IState.UpdateState()
        {   
            CheckTransition();
            OnStateUpdate();
        } 
        void IState.LateUpdateState() => OnStateLateUpdate();
        void IState.FixedUpdateState() => OnStateFixedUpdate();

        protected virtual void OnStateEnter()
        {   
            _onStateEnter?.Invoke();
        }

        protected virtual void OnStateExit()
        {
            _onStateExit?.Invoke();
        }

        protected virtual void OnStateUpdate()
        {
            _onStateUpdate?.Invoke();
        }
        
        protected virtual void OnStateLateUpdate()
        {
            _onStateLateUpdate?.Invoke();
        }
        
        protected virtual void OnStateFixedUpdate()
        {
            _onStateFixedUpdate?.Invoke();
        }

        #region Triggers

        public State AddTransition(Transition transition)
        {
            _transitions = _transitions ?? new List<Transition>();
            _transitions.Add(transition);
            return this;
        }

        public State AddTriggerTransition(object triggerId, Transition transition)
        {
            _triggerToTransitions = _triggerToTransitions ?? new Dictionary<object, List<Transition>>();
            if (!_triggerToTransitions.TryGetValue(triggerId, out var transitions))
            {
                transitions = new List<Transition>();
                _triggerToTransitions.Add(triggerId, transitions);
            }
            _transitions.Add(transition);
            return this;
        }

        protected virtual void CheckTransition()
        {
            if (ShouldTransition(_transitions, out var state))
            {
                FSM.SwitchState(state);
                return;
            }
        }

        public virtual void TriggerTransition(object triggerId)
        {
            if (_triggerToTransitions.TryGetValue(triggerId, out var transitions))
            {
                if (ShouldTransition(transitions, out var state))
                {
                    FSM.SwitchState(state);
                }
            }
        }
        
        internal static bool ShouldTransition(IList<Transition> transitions, out IState state)
        {
            state = null;
            if (transitions == null || transitions.Count == 0) return false;

            foreach (var t in transitions)
            {
                if (t.ShouldTransition())
                {
                    state = t.ToState;
                    return true;
                }
            }

            return false;
        }

        #endregion
        
       
        
        
        
    }

    public class State<TStateID> : State
    {
        public new TStateID ID
        {
            get => (TStateID) base.ID;
            set => base.ID = value;
        }

        public State(TStateID id, 
            Action onEnterLogic = null, 
            Action onExitLogic = null, 
            Action onUpdateLogic = null, 
            Action onLateUpdateLogic = null, 
            Action onFixedUpdateLogic = null) : base(id, onEnterLogic, onExitLogic, onUpdateLogic, onLateUpdateLogic, onFixedUpdateLogic)
        {
            
        }
    }


}
