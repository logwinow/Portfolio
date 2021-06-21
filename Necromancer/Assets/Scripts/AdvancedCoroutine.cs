using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


internal sealed class AdvancedCoroutine : AdvancedCoroutineBase<Func<IEnumerator>>
{
    public AdvancedCoroutine(MonoBehaviour owner, Func<IEnumerator> routine) : base(owner, routine) { }
    public override void Start()
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

internal sealed class AdvancedCoroutine<T> : AdvancedCoroutineBase<Func<T, IEnumerator>>
{
    public AdvancedCoroutine(MonoBehaviour owner, Func<T, IEnumerator> routine) : base(owner, routine) { }

    T arg1;

    public T FirstArgumentValue
    {
        get
        {
            return arg1;
        }
        set
        {
            arg1 = value;
        }
    }

    public override void Start()
    {
        if (arg1.Equals(default(T)))
            throw new Exception("arg1 is not specified");

        Start(arg1, 0);
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

internal sealed class AdvancedCoroutine<T, U> : AdvancedCoroutineBase<Func<T, U, IEnumerator>>
{
    public AdvancedCoroutine(MonoBehaviour owner, Func<T, U, IEnumerator> routine) : base(owner, routine) { }

    T arg1;
    U arg2;

    public T FirstArgumentValue
    {
        get
        {
            return arg1;
        }
        set
        {
            arg1 = value;
        }
    }

    public U SecondArgumentValue
    {
        get
        {
            return arg2;
        }
        set
        {
            arg2 = value;
        }
    }

    public override void Start()
    {
        if (arg1.Equals(default(T)) || arg2.Equals(default(U)))
            throw new Exception("arg1 or arg2 is not specified");

        Start(arg1, arg2, 0);
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
            arg2 = default(U);
        }
    }
}

internal sealed class AdvancedCoroutine<T, U, V> : AdvancedCoroutineBase<Func<T, U, V, IEnumerator>>
{
    public AdvancedCoroutine(MonoBehaviour owner, Func<T, U, V, IEnumerator> routine) : base(owner, routine) { }

    T arg1;
    U arg2;
    V arg3;

    public T FirstArgumentValue
    {
        get
        {
            return arg1;
        }
        set
        {
            arg1 = value;
        }
    }

    public U SecondArgumentValue
    {
        get
        {
            return arg2;
        }
        set
        {
            arg2 = value;
        }
    }
    public V ThirdArgumentValue
    {
        get
        {
            return arg3;
        }
        set
        {
            arg3 = value;
        }
    }

    public override void Start()
    {
        if (arg1.Equals(default(T)) || arg2.Equals(default(U)) || arg3.Equals(default(V)))
            throw new Exception("arg1 or arg2 or arg3 is not specified");

        Start(arg1, arg2, arg3, 0);
    }
    public void Start(T arg1, U arg2, V arg3)
    {
        Start(arg1, arg2, arg3, 0);
    }

    public void Start(T arg1, U arg2, V arg3, float delayTime)
    {
        if (IsWorking)
            Stop();

        this.arg1 = arg1;
        this.arg2 = arg2;
        this.arg3 = arg3;
        coroutine = owner.StartCoroutine(CheckStateCoroutine(delayTime));
    }

    IEnumerator CheckStateCoroutine(float delayTime)
    {
        if (delayTime > 0)
            yield return new WaitForSeconds(delayTime);

        yield return routine.Invoke(arg1, arg2, arg3);

        coroutine = null;
    }

    public override void Stop()
    {
        base.Stop();

        if (IsWorking)
        {
            arg1 = default(T);
            arg2 = default(U);
            arg3 = default(V);
        }
    }
}

internal abstract class AdvancedCoroutineBase<TFunc> where TFunc : Delegate
{
    protected AdvancedCoroutineBase(MonoBehaviour owner, TFunc routine)
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

    public abstract void Start();
}

