using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWriggle : MonoBehaviour
{
    [SerializeField]
    private float wriggleScale;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * wriggleScale);   
    }
}
