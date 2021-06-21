using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NextScene
{
    public static int Index { get; private set; }
    public static void StartLoading()
    {
        Index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(5);
    }
}
