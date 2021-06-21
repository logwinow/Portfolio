using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SmartCoroutineJIT
{
    public SmartCoroutineJIT(MonoBehaviour owner)
    {
        this.owner = owner;
    }

    MonoBehaviour owner;
    IEnumerator routine;

    public bool IsWorking
    {
        get
        {
            return routine != null;
        }
    }

    public void Stop()
    {
        if (IsWorking)
        {
            owner.StopCoroutine(routine);

            routine = null;
        }
    }

    public void Start(IEnumerator routine)
    {
        Stop();

        owner.StartCoroutine(this.routine = CheckStateRoutine(routine));
    }

    IEnumerator CheckStateRoutine(IEnumerator routine)
    {
        yield return routine;

        this.routine = null;
    }
}
