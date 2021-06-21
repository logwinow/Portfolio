using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPlayerPositionScript : MonoBehaviour
{
    Material mat;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        mat.SetVector("_PlayerPos", PlayerController.currentPosition);
    }
}
