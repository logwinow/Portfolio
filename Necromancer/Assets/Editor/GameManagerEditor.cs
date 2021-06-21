using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Save data"))
        {
            ((GameManager)target).SaveData();
        }

        if (GUILayout.Button("Load data"))
        {
            ((GameManager)target).LoadData();
        }

        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
