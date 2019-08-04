using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Upgrade))]
//[CanEditMultipleObjects]
sealed class UpgradeDrawer : PropertyDrawer
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

            var listTarget = fieldInfo.GetValue(targetWrapper) as Upgrade;
            var val = position.height;
            position.height = GetLineHeight(1);

            property.NextVisible(true);
            listTarget.Name = EditorGUI.TextField(position, listTarget.Name);
            position.yMin += position.height;
            position.height = val - position.height;
            EditorGUI.PropertyField(position, property);

            EditorGUI.EndProperty();

        }
        else
        {
            int index = Convert.ToInt32(variableName[2].Split('[', ']')[1]);

            var listTarget = fieldInfo.GetValue(targetWrapper) as List<Upgrade>;
            float height = GetPropertyHeight(property, label);

            var val = position.height;
            position.height = GetLineHeight(1);
            EditorGUI.indentLevel = 0;
            listTarget[index].Name = EditorGUI.TextField(position, listTarget[index].Name);
            position.yMin += position.height;
            position.height = val - position.height;
            property.NextVisible(true);

            EditorGUI.PropertyField(position, property);

            EditorGUI.EndProperty();
        }


    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var targetWrapper = property.serializedObject.targetObject;
        string[] variableName = property.propertyPath.Split('.');
        SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);
        int listenerCount;
        if (!p.isArray)
        {

            var listTarget = fieldInfo.GetValue(targetWrapper) as Upgrade;
            listenerCount = listTarget.Event.GetPersistentEventCount();
        }
        else
        {
            int index = Convert.ToInt32(variableName[2].Split('[', ']')[1]);

            var listTarget = fieldInfo.GetValue(targetWrapper) as List<Upgrade>;

            listenerCount = listTarget[index].Event.GetPersistentEventCount();
        }
        if (listenerCount == 0) listenerCount = 1;
        //TODO: Get Event list Height! woo. probably (listener count *2 +1) *line height
        return GetLineHeight(listenerCount*2.5f+3f);
    }
    public float GetLineHeight(float lineCount = 1)
    {
        return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
    }
}

#endif