using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyDoorsSource : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
