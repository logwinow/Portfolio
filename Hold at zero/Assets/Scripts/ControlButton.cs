using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlButton : MonoBehaviour
{
    [SerializeField]
    protected SceneController sceneController;
    [SerializeField]
    List<KeyCode> refKeys;
    [SerializeField]
    PressMode mode = PressMode.Hold;
    protected string keyPressedEvent;

    [SerializeField]
    enum PressMode
    {
        Hold = 0,
        Down = 1
    }

    protected virtual void Awake()
    {
        InitializeKeyPressedEvent();
    }

    protected abstract void InitializeKeyPressedEvent();

    void Update()
    {
        bool keyDown = PressedTarget(Input.GetKeyDown);

        if (mode == PressMode.Down)
        {
            if (keyDown)
                ChangeCurrentFeature();
        }
        else
        {
            if (PressedTarget(Input.GetKey))
                ChangeCurrentFeature();
        }

        if (keyDown)
        {
            EventAfterPressed();
        }
        else if (PressedTarget(Input.GetKeyUp))
            EventAfterWring();
    }

    bool PressedTarget(Func<KeyCode, bool> keyCodeFunc)
    {
        foreach (KeyCode key in refKeys)
        {
            if (keyCodeFunc.Invoke(key))
            {
                return true;
            }
        }
        return false;
    }

    protected abstract void ChangeCurrentFeature();
    protected virtual void EventAfterPressed()
    {
        Messenger<bool>.Broadcast(keyPressedEvent, true);
    }

    protected virtual void EventAfterWring()
    {
        Messenger<bool>.Broadcast(keyPressedEvent, false);
    }
}

[Serializable]
public class ControlFeature
{
    public Feature feature;
    [SerializeField]
    float decreaseValue;
    public float DecreaseValue
    {
        get
        {
            return decreaseValue;
        }
    }
}
