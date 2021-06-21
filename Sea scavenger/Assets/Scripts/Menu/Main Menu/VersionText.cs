using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionText : MonoBehaviour
{
    private TextMeshProUGUI _versionTMP;

    private void Awake()
    {
        _versionTMP = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        _versionTMP.text = $"{Application.productName} {Application.version}";
    }
}
