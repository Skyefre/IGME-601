using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : StateManager<PlayerStateHandler.PlayerState>
{
    public enum PlayerState
    {
        Idle,
        Run,
        JumpSquat,
        Jump,
        Land,

    }

    private void Awake()
    {
        currentState = States[PlayerState.Idle];
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
