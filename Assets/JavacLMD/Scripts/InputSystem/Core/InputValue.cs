using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace JavacLMD.InputSystem
{



    [Serializable]
    public class InputValue
    {

        [SerializeField]
        private object _value;

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }



    }

    [Serializable]
    public class InputValue<T> : InputValue
    {
        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }
    }

    [Serializable]
    public sealed class FloatValue : InputValue<float> { }

}