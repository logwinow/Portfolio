using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RefreshGUID
{
    [MenuItem("CONTEXT/SavableObject/Refresh GUID")]
    private static void RefreshGUIDMenuItem(MenuCommand command)
    {
        var serObj = new SerializedObject(command.context);
        var guidBytesProp = serObj.FindProperty("_guidBytes"); 
        guidBytesProp.ClearArray();
        
        Byte[] newGuidBytes = Guid.NewGuid().ToByteArray();

        for (int i = 0; i < newGuidBytes.Length; i++)
        {
            guidBytesProp.InsertArrayElementAtIndex(i);
            guidBytesProp.GetArrayElementAtIndex(i).intValue = newGuidBytes[i];
        }

        serObj.FindProperty("_isPermanent").boolValue = true;

        serObj.ApplyModifiedProperties();
    }
}
