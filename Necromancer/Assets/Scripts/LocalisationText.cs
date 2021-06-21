using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalisationText : MonoBehaviour
{
    [SerializeField]
    LanguageNode languageNode;

    private void Start()
    {
        GetComponent<Text>().text = languageNode.GetText();
    }
}
