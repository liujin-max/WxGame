using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 状态机
/// </summary>
public class FSM<T> where T : class
{
    public Dictionary<_C.FSMSTATE, State<T>> States = new Dictionary<_C.FSMSTATE, State<T>>();

    public State<T> CurrentState;//当前状态
    public T Owner;
 
    public FSM(T owner, State<T>[] states)
    {
        Owner = owner;

        foreach (var state in states)
        {
            state.SetFSM(this);
            States.Add(state.ID, state);
        }
    }

    public State<T> GetState(_C.FSMSTATE id)
    {
        return States[id];
    }

    public void Transist(_C.FSMSTATE id)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        State<T> state = GetState(id);
        if (state != null)
        {
            CurrentState = state;
            CurrentState.Enter();
        }
    }

    public void Transist(_C.FSMSTATE id, Dictionary<string, object> keyValuePairs)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }

        State<T> state = GetState(id);
        if (state != null)
        {
            CurrentState = state;
            CurrentState.Enter(keyValuePairs);
        }
    }
 

    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }
}