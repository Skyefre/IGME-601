using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : EnemyBaseState
{
    private Player player;
    public MeleeAttackState(Enemy enemy, string animationName) : base(enemy, animationName)
    {

    }


    public override void Enter()
    {
        base.Enter();
        enemy.SetState(Enemy.EnemyState.SideAttack);
        player.TakeDamage(player.gameObject, 1, 4, 4, 4);
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
    }

    public override void AnimationAttackTrigger()
    {
        base.AnimationAttackTrigger();
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();
        enemy.SwitchAIState(enemy.patrolState);
    }
}
