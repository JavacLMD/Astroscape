using System;
using UnityEngine;

namespace JavacLMD.FiniteStateMachine
{
    public class Transition<TStateID>
    {
        [SerializeField] private TStateID _targetStateID;
        [SerializeField] private Func<bool> _condition;

        public TStateID TargetStateID => _targetStateID;
        public Func<bool> Condition => _condition;

        public Transition(TStateID targetStateID, Func<bool> condition)
        {
            _targetStateID = targetStateID;
            _condition = condition;
        }

        public bool ShouldTransition()
        {
            return Condition == null || Condition();
        }
    }



}
