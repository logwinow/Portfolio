using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

public class EditorContextUtils
{
    [MenuItem("CONTEXT/GameManager/UpdateIDs")]
    private static void UpdateIDs(MenuCommand command)
    {
        SerializedObject icSerObj = new SerializedObject(command.context);

        SerializedProperty itemsProp;
        SerializedObject prefabSerObj;

        itemsProp = icSerObj.FindProperty("items");

        for (int i = 0; i < itemsProp.arraySize; i++)
        {
            prefabSerObj = new SerializedObject(
                (itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("prefab")
                    .objectReferenceValue as GameObject).GetComponent<CollectableObject>());

            prefabSerObj.FindProperty("id").FindPropertyRelative("section1").intValue = 
                itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("sectionID")
                .FindPropertyRelative("section1").intValue;
            prefabSerObj.FindProperty("id").FindPropertyRelative("section2").intValue =
                itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("sectionID")
                .FindPropertyRelative("section2").intValue;
            prefabSerObj.FindProperty("id").FindPropertyRelative("section3").intValue =
                itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("sectionID")
                .FindPropertyRelative("section3").intValue;
            prefabSerObj.FindProperty("id").FindPropertyRelative("section3").intValue =
                itemsProp.GetArrayElementAtIndex(i).FindPropertyRelative("sectionID")
                .FindPropertyRelative("section3").intValue;

            prefabSerObj.ApplyModifiedProperties();
        }
        
        icSerObj.ApplyModifiedProperties();
    }
}
