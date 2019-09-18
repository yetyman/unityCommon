using Assets;
using Assets.CommonLibrary.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PrefabFactory))]
//[CanEditMultipleObjects]
sealed class PrefabFactoryDrawer : AdvancedPropertyDrawer<PrefabFactory>
{
    public void UpdateName(PrefabFactory target)
    {
        if(target!=null)
            if (string.IsNullOrWhiteSpace(target.PrefabName) && target.PrefabPool != null)
            {
                if (target.PrefabPool is GenericObjectPooler)
                    target.PrefabName = ((GenericObjectPooler)target.PrefabPool).GetPoolName();
                else if (target.PrefabPool is UniqueObjectPooler)
                {
                    target.PrefabName = ((UniqueObjectPooler)target.PrefabPool).name;
                }
            }
    }

    public override void OnGuiItem(PrefabFactory singleItem, Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, label, true);

        UpdateName(singleItem);
    }
}

#endif