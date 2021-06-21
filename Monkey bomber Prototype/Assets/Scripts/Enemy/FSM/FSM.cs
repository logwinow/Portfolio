using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class FSM
{
    private List<State> _states;
    
    private STAGE _stage = STAGE.ENTER;
    private string _nextStateName;
    private State _currentState;
    
    protected enum STAGE
    {
        ENTER,
        UPDATE,
        EXIT
    }

    public void Initialize(List<State> states, string startState)
    {
        _states = states;
        _currentState = GetState(startState);
    }

    public void Process()
    {
        if (_currentState == null)
        {
            Debug.LogWarning("State is null");

            if (_stage == STAGE.EXIT)
                _currentState = GetState(_nextStateName);
            
            return;
        }

        switch (_stage)
        {
            case STAGE.ENTER:
                _currentState.Enter();

                _stage = STAGE.UPDATE;
                break;
            case STAGE.UPDATE:
                _currentState.Update();
                break;
            case STAGE.EXIT:

                _currentState.Exit();

                _currentState = GetState(_nextStateName);

                _stage = STAGE.ENTER;
                break;
        }
    }

    public void SetNextActionState(string nextStateName)
    {
        _stage = STAGE.EXIT;
        _nextStateName = nextStateName;
    }

    private State GetState(string stateName)
    {
        return _states.Find((s) => s.Name == stateName);
    }
}
