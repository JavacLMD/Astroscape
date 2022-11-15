using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

namespace JavacLMD.Core.Attributes.Editor
{

    public class AdvancedTypePopupItem : AdvancedDropdownItem
    {
        public Type Type { get; }
        public AdvancedTypePopupItem(Type type, string name) : base(name) { Type = type; }
    }

    public class AdvancedTypePopup : AdvancedDropdown
    {
        static readonly float k_HeaderHeight = EditorGUIUtility.singleLineHeight * 2f;
        const int kMaxNamespaceNestCount = 16;
        Type[] m_Types;
        public event Action<AdvancedTypePopupItem> OnItemSelected;

        public AdvancedTypePopup(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) : base(state)
        {
            SetTypes(types);
            minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + k_HeaderHeight);
        }


        public static void AddTo(AdvancedDropdownItem root, IEnumerable<Type> types)
        {
            int itemCount = 0;

            var nullItem = new AdvancedTypePopupItem(null, TypeMenuUtility.k_NullDisplayName)
            {
                id = itemCount++
            };

            root.AddChild(nullItem);

            bool isSingleNamespace = true;
            Type[] typeArray = types.OrderByType().ToArray();
            string[] namespaces = new string[kMaxNamespaceNestCount];
            foreach (Type type in typeArray)
            {
                string[] splittedTypePath = TypeMenuUtility.GetSplittedTypePath(type);
                if (splittedTypePath.Length <= 1) continue;

                //Explicitly set sub category
                if (TypeMenuUtility.GetAttribute(type) != null)
                {
                    isSingleNamespace = false;
                    break;
                }

                for (int k = 0; (splittedTypePath.Length - 1) > k; k++)
                {
                    string ns = namespaces[k];
                    if (ns == null)
                        namespaces[k] = splittedTypePath[k];
                    else if (ns != splittedTypePath[k])
                    {
                        isSingleNamespace = false;
                        break;
                    }
                }

                if (!isSingleNamespace) break;

            }

            foreach (Type type in typeArray)
            {
                string[] splttedTypePath = TypeMenuUtility.GetSplittedTypePath(type);
                if (splttedTypePath.Length == 0) continue;

                AdvancedDropdownItem parent = root;
                if (!isSingleNamespace) 
                { 
                    for (int k = 0; (splttedTypePath.Length - 1) > k; k++)
                    {
                        AdvancedDropdownItem foundItem = GetItem(parent, splttedTypePath[k]);
                        if (foundItem != null)
                        {
                            parent = foundItem;
                        } else
                        {
                            var newItem = new AdvancedDropdownItem(splttedTypePath[k])
                            {
                                id = itemCount++
                            };
                            parent.AddChild(newItem);
                            parent = newItem;
                        }
                    }
                }

                var item = new AdvancedTypePopupItem(type, ObjectNames.NicifyVariableName(splttedTypePath[splttedTypePath.Length-1])) {
                    id = itemCount++
                };
                parent.AddChild(item);
            }

        }

        public void SetTypes(IEnumerable<Type> types)
        {
            m_Types = types.ToArray();
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Type");
            AddTo(root, m_Types);
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is AdvancedTypePopupItem typePopupItem)
            {
                OnItemSelected?.Invoke(typePopupItem);
            }
        }

        static AdvancedDropdownItem GetItem(AdvancedDropdownItem parent, string name)
        {
            foreach (AdvancedDropdownItem item in parent.children)
            {
                if (item.name == name)
                {
                    return item;
                }
            }
            return null;
        }

    }


}