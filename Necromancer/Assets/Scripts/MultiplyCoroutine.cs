using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class MultipleCoroutineBase<TKey>
{
    protected MultipleCoroutineBase(MonoBehaviour owner)
    {
        this.owner = owner;
        routines = new Dictionary<TKey, Coroutine>();
    }

    protected MonoBehaviour owner;
    protected Dictionary<TKey, Coroutine> routines;

    public bool IsNotEmpty
    {
        get
        {
            return routines.Count != 0;
        }
    }

    public virtual void Stop(TKey key)
    {
        if (IsNotEmpty)
        {
            Coroutine rout;
            if (routines.TryGetValue(key, out rout))
            {
                owner.StopCoroutine(rout);

                routines.Remove(key);
            }
        }
    }

    public virtual void StopAll()
    {
        while (!IsNotEmpty)
        {
            foreach (KeyValuePair<TKey, Coroutine> key in routines)
            {
                Stop(key.Key);
                break;
            }
        }
    }

    public virtual bool IsWorking(TKey key)
    {
        if (routines.ContainsKey(key))
            return true;
        return false;
    }
}

internal sealed class MultipleCoroutine<TKey> : MultipleCoroutineBase<TKey>
{
    public MultipleCoroutine(MonoBehaviour owner) : base(owner) { }

    public void Add(TKey key, IEnumerator cor)
    {
        Stop(key);

        routines.Add(key, owner.StartCoroutine(CheckStateCoroutine(key, cor)));
    }

    IEnumerator CheckStateCoroutine(TKey key, IEnumerator cor)
    {
        yield return cor;

        routines.Remove(key);
    }
}
