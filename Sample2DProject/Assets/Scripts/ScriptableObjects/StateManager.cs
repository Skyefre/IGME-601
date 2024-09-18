using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState, BaseState<EState>> States = new Dictionary<EState, BaseState<EState>>();

    protected BaseState<EState> currentState;

    protected bool isTransitioningState = false;

    private void Awake()
    {
        
    }
    void Start()
    {
        currentState.EnterState();
    }

    void Update()
    {
        EState nextStateKey = currentState.GetNextState();

        if(!isTransitioningState && nextStateKey.Equals(currentState.StateKey))
        {
            currentState.UpdateState();
        }
        else if (!isTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }

    public void TransitionToState(EState stateKey)
    {
        isTransitioningState = true;
        currentState.ExitState();
        currentState = States[stateKey];
        currentState.EnterState();
        isTransitioningState = false;
    }

   void OnTriggerEnter2D(Collider2D collision)
    {
        currentState.OnTriggerEnter2D(collision);

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        
    }
}
