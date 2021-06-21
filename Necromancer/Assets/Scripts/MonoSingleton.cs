using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{

    private static T m_instance;
    public static T Instance
    {
        get
        {
            if (m_instance == null)
                Debug.LogError("m_instance is null");

            return m_instance;
        }
    }

    private void Awake()
    {
        m_instance = (T)this;

        Init();
    }

    public virtual void Init()
    {

    }
}
