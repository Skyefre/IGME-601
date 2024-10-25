using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : EnemyBaseState
{
    public ChaseState(Enemy enemy, string animationName) : base(enemy, animationName)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(Time.time >= enemy.stateTime + enemy.chaseTime)
        {
            if (enemy.CheckForPlayer())
                enemy.SwitchAIState(enemy.playerDetectedState);
            else
                enemy.SwitchAIState(enemy.patrolState);
        }
        else
        {
            if (enemy.CheckForMeleeTarget())
                enemy.SwitchAIState(enemy.meleeAttackState);
            Chase();
        }
    }

    void Chase()
    {
        if(enemy.facingRight)
           enemy.hspd = enemy.runSpeed / 2;
        else
           enemy.hspd = - enemy.runSpeed / 2;
    }
}
