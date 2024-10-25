using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public PatrolState(Enemy enemy, string animationName) : base (enemy, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckForObstacles())
            Rotate();
        if (enemy.CheckForPlayer())
            enemy.SwitchAIState(enemy.playerDetectedState);
     
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (enemy.facingRight)
            enemy.hspd = enemy.runSpeed / 3;
        else
            enemy.hspd = -enemy.runSpeed / 3;

    }

    public void Rotate()
    {
        enemy.facingRight = !enemy.facingRight;
    }
}
