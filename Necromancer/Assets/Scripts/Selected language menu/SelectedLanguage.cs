using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedLanguage : MonoBehaviour
{

    public void SetLanguage(int index)
    {
        Language.laguage = (Language.LaguageType)index;
        NextScene.StartLoading();
    }
}
