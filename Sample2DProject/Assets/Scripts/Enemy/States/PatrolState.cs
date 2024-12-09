using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float patrolRotateTimer = 0f;
    public float basePatrolRotateVal = 5f;
    public float patrolRotateVal = 5f;
    public float patrolRotateValRange = 2f;

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
        //if (Time.time > patrolRotateTimer + patrolRotateVal)
        //{
        //    patrolRotateTimer = Time.time;    // Update the last rotate time
        //    Rotate();
        //}
        if(patrolRotateTimer >= patrolRotateVal)
        {
            patrolRotateTimer = 0;
            patrolRotateVal = Random.Range(basePatrolRotateVal - patrolRotateValRange, basePatrolRotateVal + patrolRotateValRange);
            Rotate();
        }
        else
        {
            patrolRotateTimer += Time.deltaTime;
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
