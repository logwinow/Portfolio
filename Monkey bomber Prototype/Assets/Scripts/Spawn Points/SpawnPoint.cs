using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnPoint : MonoBehaviour
{
    [SerializeField] private float _resetTime;

    private ISpawnable _spawnable;

    private void Start()
    {
        Set();
    }

    protected abstract void Set();

    protected void Set(ISpawnable spawnable)
    {
        if (spawnable is Behaviour behaviour)
        {
            behaviour.transform.position = transform.position;
        }
        
        _spawnable = spawnable;
        spawnable.onDespawn += Clear;
    }

    protected virtual void Clear()
    {
        _spawnable.onDespawn -= Clear;

        StartCoroutine(ResetCoroutine());
    }

    private IEnumerator ResetCoroutine()
    {
        float time = 0;

        while ((time += Time.deltaTime) < _resetTime)
        {
            yield return null;
        }

        Set();
    }
}
