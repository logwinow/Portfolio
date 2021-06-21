using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRollable
{
    float Chance { get; }
}

public class Roll
{
    private float rollLength;

    public void CalculateRollLength(IEnumerable<IRollable> rollables)
    {
        rollLength = rollables.Sum(r => r.Chance);
    }

    public IRollable GetRandomElement(IEnumerable<IRollable> rollables, bool recalculateLength = false)
    {
        if (rollLength <= 0 || recalculateLength)
            CalculateRollLength(rollables);

        return GetRandomElement(rollables, rollLength);
    }

    //public static IRollable GetRandomElement(IEnumerable<IRollable> rollables)
    //{
    //    return GetRandomElement(rollables, rollables.Sum(r => r.Chance));
    //}

    public static IRollable GetRandomElement(IEnumerable<IRollable> rollables, float rollLength)
    {
        float randValue = UnityEngine.Random.Range(0, rollLength);

        float sumValuesBehind = 0;

        foreach (var r in rollables)
        {
            if ((sumValuesBehind += r.Chance) >= randValue)
                return r;
        }

        return null;
    }
}

/// <typeparam name="T">T is type of value</typeparam>
[Serializable]
public class Roll<T>
{
    public Roll((float Coeff, T Value)[] pairs)
    {
        coeffs = new float[pairs.Length];
        values = new T[pairs.Length];

        for (int i = 0; i < pairs.Length; i++)
        {
            coeffs[i] = pairs[i].Coeff;
            values[i] = pairs[i].Value;
        }
        
        foreach(var k in coeffs)
        {
            rollLength += k;
        }
    }

    [SerializeField]
    protected float[] coeffs;
    [SerializeField]
    protected T[] values;
    [SerializeField]
    protected float rollLength;
    public T GetRandomElement()
    {
        float randValue = UnityEngine.Random.Range(0, rollLength);

        float sumValuesBehind = 0;

        for (int i = 0; i < Count; i++)
        {
            if (coeffs[i] + sumValuesBehind >= randValue)
            {
                return values[i];
            }

            sumValuesBehind += coeffs[i];
        }

        return default(T);
    }

    public int Count
    {
        get => values.Length;
    }

    public T GetValueAtIndex(int i)
    {
        return values[i];
    }
}
