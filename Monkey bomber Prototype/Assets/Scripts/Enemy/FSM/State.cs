using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> : State where T : FSM
{
    protected State(T fsm, string name) : base(name)
    {
        this.fsm = fsm;
    }
    
    protected T fsm;
}

public abstract class State
{
    private string _name;
    public string Name => _name;

    protected State(string name)
    {
        _name = name;
    }
    
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
