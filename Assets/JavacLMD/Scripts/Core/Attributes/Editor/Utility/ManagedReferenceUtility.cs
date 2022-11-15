using System;
using System.Reflection;
using UnityEditor;

namespace JavacLMD.Core.Attributes.Editor
{
    public static class ManagedReferenceUtility
    {
        public static object SetManagedReference(this SerializedProperty prop, Type type)
        {
            object obj = (type != null) ? Activator.CreateInstance(type) : null;
            prop.managedReferenceValue = obj;
            return obj;
        }

        public static Type GetType(string typeName)
        {
            int splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
            return assembly.GetType(typeName.Substring(splitIndex + 1));
        }
    }
}
