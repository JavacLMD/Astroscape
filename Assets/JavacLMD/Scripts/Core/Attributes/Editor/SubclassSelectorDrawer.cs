using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;


namespace JavacLMD.Core.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
    public class SubclassSelectorDrawer : PropertyDrawer
    {
        struct TypePopupCache
        {
            public AdvancedTypePopup TypePopup { get; }
            public AdvancedDropdownState State { get; }
            public TypePopupCache(AdvancedTypePopup typePopup, AdvancedDropdownState state)
            {
                TypePopup = typePopup;
                State = state;
            }
        }

        const int k_MaxTypePopupLineCount = 13;
        static readonly Type k_UnityObjectType = typeof(UnityEngine.Object);
        static readonly GUIContent k_NullDisplayLabel = new GUIContent(TypeMenuUtility.k_NullDisplayName);
        static readonly GUIContent k_IsNotManagedReferenceLabel = new GUIContent("The property type is not managed reference");

        readonly Dictionary<string, TypePopupCache> m_TypePopups = new Dictionary<string, TypePopupCache>();
        readonly Dictionary<string, GUIContent> m_TypeNameCaches = new Dictionary<string, GUIContent>();
        SerializedProperty m_TargetProperty;


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                Rect popupPosition = new Rect(position);
                popupPosition.width -= EditorGUIUtility.labelWidth;
                popupPosition.x += EditorGUIUtility.labelWidth;
                popupPosition.height = EditorGUIUtility.singleLineHeight;

                if (EditorGUI.DropdownButton(popupPosition, GetTypeName(property), FocusType.Keyboard))
                {
                    TypePopupCache popup = GetTypePopup(property);
                    m_TargetProperty = property;
                    popup.TypePopup.Show(popupPosition);
                }

                EditorGUI.PropertyField(position, property, label, true);
            } else
            {
                EditorGUI.LabelField(position, label, k_IsNotManagedReferenceLabel);
            }
            EditorGUI.EndProperty();
        }


        TypePopupCache GetTypePopup(SerializedProperty property)
        {
            string managedReferenceFieldTypeName = property.managedReferenceFieldTypename;
            if (!m_TypePopups.TryGetValue(managedReferenceFieldTypeName, out TypePopupCache result)) {
                var state = new AdvancedDropdownState();
                Type baseType = ManagedReferenceUtility.GetType(managedReferenceFieldTypeName);
                SubclassSelectorAttribute att = attribute as SubclassSelectorAttribute;

                List<Type> types = TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(p =>
                        (p.IsPublic || p.IsNestedPublic) &&
                        (!p.IsAbstract) &&
                        (!p.IsGenericType) &&
                        (!k_UnityObjectType.IsAssignableFrom(p)) &&
                        Attribute.IsDefined(p, typeof(SerializableAttribute))).ToList();
                
                var popup = new AdvancedTypePopup( types, k_MaxTypePopupLineCount, state);
                popup.OnItemSelected += item =>
                {
                    Type type = item.Type;
                    object obj = m_TargetProperty.SetManagedReference(type);
                    m_TargetProperty.isExpanded = (obj != null);
                    m_TargetProperty.serializedObject.ApplyModifiedProperties();
                    m_TargetProperty.serializedObject.Update();
                };

                result = new TypePopupCache(popup, state);
                m_TypePopups.Add(managedReferenceFieldTypeName, result);
            }
            return result;
        }

        private GUIContent GetTypeName(SerializedProperty property)
        {
            string managedReferenceFullTypeName = property.managedReferenceFullTypename;
            if (string.IsNullOrEmpty(managedReferenceFullTypeName)) return k_NullDisplayLabel;
            if (m_TypeNameCaches.TryGetValue(managedReferenceFullTypeName, out GUIContent typeNameCache)) return typeNameCache;

            Type type = ManagedReferenceUtility.GetType(managedReferenceFullTypeName);
            string typeName = null;

            AddTypeMenuAttribute typeMenu = TypeMenuUtility.GetAttribute(type);
            if (typeMenu != null)
            {
                typeName = typeMenu.GetTypeNameWithoutPath();
                if (!string.IsNullOrWhiteSpace(typeName)) typeName = ObjectNames.NicifyVariableName(typeName);
            }

            if (string.IsNullOrWhiteSpace(typeName)) typeName = ObjectNames.NicifyVariableName(type.Name);

            GUIContent result = new GUIContent(typeName);
            m_TypeNameCaches.Add(managedReferenceFullTypeName, result);
            return result;
        }




    }
}
