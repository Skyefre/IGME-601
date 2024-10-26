using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectedState : EnemyBaseState
{
    public PlayerDetectedState(Enemy enemy, string animationName) : base(enemy, animationName)
    {

    }

    public override void Enter()
    {
        base.Enter();
        enemy.hspd = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!enemy.CheckForPlayer())
            enemy.SwitchAIState(enemy.patrolState);
        else
        {
            if (Time.time >= enemy.stateTime + enemy.playerDetectedWaitTime)
            {
                enemy.SwitchAIState(enemy.chaseState);
            }            
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
