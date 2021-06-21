using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

internal sealed class OrdinaryCoroutine : OrdinaryCoroutineBase<Func<IEnumerator>>
{
    public OrdinaryCoroutine(MonoBehaviour owner, Func<IEnumerator> routine) : base(owner, routine) { }
    public void Start()
    {
        Start(0);
    }

    public void Start(float delayTime)
    {
        if (IsWorking)
            Stop();

        coroutine = owner.StartCoroutine(CheckStateCoroutine(delayTime));
    }
    IEnumerator CheckStateCoroutine(float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke();

        coroutine = null;
    }
}

internal sealed class OrdinaryCoroutine<T> : OrdinaryCoroutineBase<Func<T, IEnumerator>>
{
    public OrdinaryCoroutine(MonoBehaviour owner, Func<T, IEnumerator> routine) : base(owner, routine) { }

    T arg1;

    public T FirstArgumentValue
    {
        get
        {
            return arg1;
        }
    }

    public void Start(T arg1)
    {
        Start(arg1, 0);
    }

    public void Start(T arg1, float delayTime)
    {
        if (IsWorking)
            Stop();

        this.arg1 = arg1;
        coroutine = owner.StartCoroutine(CheckStateCoroutine(delayTime));
    }
    IEnumerator CheckStateCoroutine(float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke(arg1);

        coroutine = null;
    }

    public override void Stop()
    {
        base.Stop();

        if (IsWorking)
        {
            arg1 = default(T);
        }
    }
}

internal sealed class OrdinaryCoroutine<T, U> : OrdinaryCoroutineBase<Func<T, U, IEnumerator>>
{
    public OrdinaryCoroutine(MonoBehaviour owner, Func<T, U, IEnumerator> routine) : base(owner, routine) { }

    T arg1;
    U arg2;

    public T FirstArgumentValue
    {
        get
        {
            return arg1;
        }
    }

    public U SecondArgumentValue
    {
        get
        {
            return arg2;
        }
    }

    public void Start(T arg1, U arg2)
    {
        Start(arg1, arg2, 0);
    }

    public void Start(T arg1, U arg2, float delayTime)
    {
        if (IsWorking)
            Stop();

        this.arg1 = arg1;
        this.arg2 = arg2;
        coroutine = owner.StartCoroutine(CheckStateCoroutine(delayTime));
    }
    IEnumerator CheckStateCoroutine(float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke(arg1, arg2);

        coroutine = null;
    }

    public override void Stop()
    {
        base.Stop();

        if (IsWorking)
        {
            arg1 = default(T);
        }
    }
}

//public sealed class OrdinaryCoroutine<T> : OrdinaryCoroutineBase<Func<T, IEnumerator>>
//{
//    public OrdinaryCoroutine(MonoBehaviour owner, Func<T, IEnumerator> routine) : base(owner, routine) { }

//    public void Start(T arg1)
//    {
//        if (IsWorking)
//            Stop();

//        coroutine = owner.StartCoroutine(CheckStateCoroutine(arg1));
//    }
//    IEnumerator CheckStateCoroutine(T arg1)
//    {
//        yield return routine.Invoke(arg1);

//        coroutine = null;
//    }
//}

internal sealed class SimpleAdvancedCoroutine : OrdinaryCoroutineBase<Func<IEnumerator>>
{
    public SimpleAdvancedCoroutine(MonoBehaviour owner) : base(owner, null) { }
    public void Start(Func<IEnumerator> routine)
    {
        Start(routine, 0);
    }

    public void Start(Func<IEnumerator> routine, float delayTime)
    {
        if (IsWorking)
            Stop();

        this.routine = routine;
        coroutine = owner.StartCoroutine(CheckStateCoroutine(delayTime));
    }
    IEnumerator CheckStateCoroutine(float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke();

        coroutine = null;
    }
}

internal sealed class SimpleAdvancedCoroutine<T> : OrdinaryCoroutineBase<Func<T, IEnumerator>>
{
    public SimpleAdvancedCoroutine(MonoBehaviour owner) : base(owner, null) { }
    public void Start(Func<T, IEnumerator> routine, T arg1)
    {
        Start(routine, arg1, 0);
    }

    public void Start(Func<T, IEnumerator> routine, T arg1, float delayTime)
    {
        if (IsWorking)
            Stop();

        this.routine = routine;
        coroutine = owner.StartCoroutine(CheckStateCoroutine(arg1, delayTime));
    }
    IEnumerator CheckStateCoroutine(T arg1, float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke(arg1);

        coroutine = null;
    }
}

internal abstract class OrdinaryCoroutineBase<TFunc> where TFunc : Delegate
{
    protected OrdinaryCoroutineBase(MonoBehaviour owner, TFunc routine)
    {
        this.owner = owner;
        this.routine = routine;
    }

    protected MonoBehaviour owner;
    protected Coroutine coroutine;
    protected TFunc routine;

    public bool IsWorking
    {
        get
        {
            return coroutine != null;
        }
    }

    public virtual void Stop()
    {
        if (IsWorking)
        {
            owner.StopCoroutine(coroutine);

            coroutine = null;
        }
    }
}
