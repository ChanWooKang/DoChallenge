using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;

public class FSM<T> : MonoBehaviour
{
    T owner;

    IFSMState<T> currState = null;
    IFSMState<T> prevState = null;

    public IFSMState<T> CurrentState { get { return currState; } }
    public IFSMState<T> PreviousState { get { return prevState; } }

    protected void InitState(T owner, IFSMState<T> init)
    {
        this.owner = owner;
        ChangeState(init);
    }

    protected void FSMUpdate() { currState?.Execute(owner); }
    public void ChangeState(IFSMState<T> newState)
    {
        prevState = currState;
        prevState?.Exit(owner);
        currState = newState;
        currState?.Enter(owner);
    }

    public void ReverseState()
    {
        if (prevState != null)
            ChangeState(prevState);
    }
}