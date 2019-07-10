using Assets.GenericClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Vec3))]
[CanEditMultipleObjects]
sealed class Vec3Drawer : PropertyDrawer
{
    Texture2D NonTransparentTexture2d = null;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (NonTransparentTexture2d == null)
        {
            NonTransparentTexture2d = new Texture2D(1, 1, TextureFormat.ARGB32, false);

            var color = Color.black;//GUI.skin.box.normal.background.GetPixel(0, 0);
            color.a = 1;
            NonTransparentTexture2d.SetPixel(0, 0, color);
        }
        var targetWrapper = property.serializedObject.targetObject;//<---target object is the whole behaviour class???
        
        EditorGUI.BeginProperty(position, label, property);

        string[] variableName = property.propertyPath.Split('.');
        SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);
        if (!p.isArray)
        {
            var singleTarget = fieldInfo.GetValue(targetWrapper) as Vec3;
            position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));
            DrawSingle(position, singleTarget);

            return;
        }
        else
        {
            int index = Convert.ToInt32(variableName[2].Split('[',']')[1]);

            var listTarget = fieldInfo.GetValue(targetWrapper) as List<Vec3>;
            float height = GetPropertyHeight(property, label);

            position.height /= 2;
            //position = EditorGUI.PrefixLabel(position, label);
            EditorGUI.indentLevel = 0;

            DrawSingle(position, listTarget[index]);

            EditorGUI.EndProperty();
        }
    }
    public Rect DrawSingle(Rect position, Vec3 nullableVec3, bool noLabel = false)
    {
        position.height = GetLineHeight(1);

        var togglePos = position;
        togglePos.width = GetLineHeight(1);
        position.width -= togglePos.width;
        position.x += togglePos.width;

        if(noLabel)
            FillArea(position);//hiding element # labels from lists
        nullableVec3.HasValue = EditorGUI.Toggle(togglePos, "", nullableVec3.HasValue);
        if(nullableVec3.HasValue)
            nullableVec3.Value = EditorGUI.Vector3Field(position, "", nullableVec3.Value);
        position.y += GetLineHeight();

        
        return position;



    }
    public void FillArea(Rect position, Color? fillColor = null)
    {

        if (fillColor == null)
        {
            EditorGUI.HelpBox(position, "", MessageType.None);//sad but true, this is the best way i've got. seemless in the editor though!
            EditorGUI.HelpBox(position, "", MessageType.None);
            EditorGUI.HelpBox(position, "", MessageType.None);
            EditorGUI.HelpBox(position, "", MessageType.None);
        }
        else
        {
            var defaultGUIBackgroundTexture= GUI.skin.box.normal.background;
            var color = fillColor;//GUI.skin.box.normal.background.GetPixel(0, 0);
            NonTransparentTexture2d.SetPixel(0, 0, color.Value);
            EditorGUI.HelpBox(position, "", MessageType.None);
            GUI.skin.textField.normal.background = defaultGUIBackgroundTexture;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var targetWrapper = property.serializedObject.targetObject;

        string[] variableName = property.propertyPath.Split('.');
        SerializedProperty p = property.serializedObject.FindProperty(variableName[0]);
        if (!p.isArray)
        {
            return GetLineHeight(1);
        }
        else
        {
            var listTarget = fieldInfo.GetValue(targetWrapper) as List<IndexToIndex>;
            return GetLineHeight(1);
        }
    }
    public float GetLineHeight(float lineCount = 1)
    {
        return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
    }
}

#endif