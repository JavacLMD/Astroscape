using JavacLMD.Core.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubclassSelectorExample : MonoBehaviour
{

    [SerializeReference, SubclassSelector]
    public BaseClass exampleClass;

    [SerializeReference, SubclassSelector]
    public BaseClass[] exampleList = new BaseClass[3];
    
}

public class ClassList : List<BaseClass> { }

public abstract class BaseClass
{
    public int BaseField;
}

[Serializable]
public class A1 : BaseClass { public int a1_IntField; }
[Serializable] public class A2 : A1 { public Vector2 a2_Vector2Field; }
[Serializable] public class A3 : BaseClass { public string a3_StringField; }
[Serializable] public class B1 : BaseClass { public float b1_FloatField; }
[Serializable] public class B2 : BaseClass { public int b2_IntField; }
[Serializable] public class B3 : B2 { public int b3_IntField; }
[Serializable] public class C1<T> : BaseClass { public T C; }