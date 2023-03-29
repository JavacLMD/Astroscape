using System;
using System.Collections.Generic;

namespace JavacLMD.FSM
{


    public enum StateAction
    {
        Enter,
        Exit,
        Update,
        LateUpdate,
        FixedUpdate
    }
    
    
    public class State
    {
        
        private Dictionary<StateAction, List<Action>> _actionMap;



        public State()
        {
            _actionMap = new Dictionary<StateAction, List<Action>>();

        }
        


        void OnEnter()
        {
            foreach (var action in _actionMap[StateAction.Enter])
                action?.Invoke();
        }

        void OnExit()
        {
            foreach (var action in _actionMap[StateAction.Exit])
                action?.Invoke();
        }

        void OnUpdate()
        {
            foreach (var action in _actionMap[StateAction.Update])
                action?.Invoke();
        }
        
        void OnLateUpdate()
        {
            foreach (var action in _actionMap[StateAction.LateUpdate])
                action?.Invoke();
        }
        
        void OnFixedUpdate()
        {
            foreach (var action in _actionMap[StateAction.FixedUpdate])
                action?.Invoke();
        }

        internal void AddAction(StateAction actionType, Action action)
        {
            List<Action> actions = null;
            if (!_actionMap.TryGetValue(actionType, out actions))
            {
                actions = new List<Action>();
            }
            
            if (!actions.Contains(action))
                actions.Add(action);

            _actionMap[actionType] = actions;
        }
        

    }
    
    
    
}
