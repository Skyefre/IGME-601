using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float lastRotateTime = 0f;
    private float intervalTime = 5f; 

    public PatrolState(Enemy enemy, string animationName) : base (enemy, animationName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
       //enemy.SetState(Enemy.EnemyState.Run);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CheckForLedge())
            Rotate();
        if (enemy.CheckForObstacles() && enemy.CheckForJumpSpace())
        {
            Jump();
        }
        if (enemy.CheckForPlayer())
            enemy.SwitchAIState(enemy.playerDetectedState);
     
    }

    public override void PhysicsUpdate()
    {
        if (Time.time > lastRotateTime + intervalTime)
        {
            lastRotateTime = Time.time;    // Update the last rotate time
            Rotate();
        }

        
        base.PhysicsUpdate();

        //if (enemy.facingRight)
        //    enemy.hspd = enemy.runSpeed / 3;
        //else
        //    enemy.hspd = -enemy.runSpeed / 3;

    }

    public void Rotate()
    {
        enemy.facingRight = !enemy.facingRight;
    }

    public void Jump()
    {
        enemy.hspd = enemy.facingRight ? 6 : -6;
        enemy.vspd = 15;
    }
}
