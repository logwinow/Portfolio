using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossfireController : MonoBehaviour
{
    private void Start()
    {
        Close();
    }

    public void Show()
    {
        SetActive(true);
    }

    public void Close()
    {
        SetActive(false);
    }

    private void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
