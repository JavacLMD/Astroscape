using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JavacLMD.FSM;
using UnityEngine;

namespace JavacLMD.FiniteStateMachine
{
    
    [Serializable]
    public struct Transition
    {
        public IState ToState;
        public List<Condition> Conditions; //if all statements are true, 

        public Transition AddCondition(Condition condition)
        {
            Conditions.Add(condition);
            return this;
        }

        public bool ShouldTransition()
        {
            foreach (var c in Conditions)
            {
                if (c.Evaluate() == false) return false;
            }

            return true;
        }
        
    }

    public class FuncCondition : Condition
    {
        private Func<bool> _function;

        public FuncCondition(Func<bool> function, bool expectedResult = true) : base(expectedResult)
        {
            _function = function;
        }

        protected override bool Statement()
        {
            return _function();
        }
    }

    public abstract class Condition
    {
        private bool _isCached = false, _cachedStatement = false;
        private bool _expectedResult = true;

        protected Condition(bool expectedResult = true)
        {
            _expectedResult = expectedResult;
        }

        protected abstract bool Statement();

        public virtual bool Evaluate()
        {
            if (!_isCached)
            {
                _isCached = true;
                _cachedStatement = Statement();
            }
            
            return _cachedStatement == _expectedResult;
        }

        internal void ClearStatementCache()
        {
            _isCached = false;
        }

    }
    
}
