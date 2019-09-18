using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets.CommonLibrary.Editor
{
#if UNITY_EDITOR
    //[CanEditMultipleObjects]
    public abstract class AdvancedPropertyDrawer<T> : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var targetWrapper = property.serializedObject.targetObject;
            string[] variableName = property.propertyPath.Split('.');
            SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);
            SerializedProperty it = p.Copy();
            //while (it.Next(true))
            //{ // or NextVisible, also, the bool argument specifies whether to enter on children or not
            //    Debug.Log(it.name);
            //}
            if (!p.isArray)
            {

                var target = (T)fieldInfo.GetValue(targetWrapper);

                OnGuiItem(target, position, property, label);
            }
            else
            {
                int index = Convert.ToInt32(variableName[2].Split('[', ']')[1]);

                var listTarget = (List<T>)fieldInfo.GetValue(targetWrapper);
                
                OnGuiItemInList(listTarget[index], position, property, label);
            }

            EditorGUI.EndProperty();
        }

        public virtual void OnGuiItemInList(T singleItem, Rect position, SerializedProperty property, GUIContent label)
        {
            OnGuiItem(singleItem, position, property, label);
        }
        public abstract void OnGuiItem(T singleItem, Rect position, SerializedProperty property, GUIContent label);


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
#endif
}
