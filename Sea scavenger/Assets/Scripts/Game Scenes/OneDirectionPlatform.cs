using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneDirectionPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.S))
        {
            SetPassableState();
            return;
        }

        if (target.transform.position.y <= transform.position.y)
        {
            SetPassableState();
        }
        else
        {
            SetNonPassableState();
        }
    }

    private void SetPassableState()
    {
        if (col.isTrigger)
            return;

        SetState(true, 2);
    }

    private void SetNonPassableState()
    {
        if (!col.isTrigger)
            return;

        SetState(false, 0);
    }

    private void SetState(bool triggerState, int layer)
    {
        col.isTrigger = triggerState;
        gameObject.layer = layer;
    }
}
