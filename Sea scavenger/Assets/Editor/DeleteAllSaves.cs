using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class DeleteAllSaves
{
    [MenuItem("Utils/Delete all saves")]
    private static void DeleteAllSavesMenuItem()
    {
        var dirPath = Path.Combine(Application.dataPath,
            "Resources", "Saves");

        Directory.Delete(dirPath, true);
        File.Delete(dirPath + ".meta");
    }

    [MenuItem("Utils/Delete all saves", true)]
    private static bool DeleteAllSavesMenuItemValidate()
    {
        return Directory.Exists(Path.Combine(Application.dataPath,
            "Resources", "Saves"));
    }
}
