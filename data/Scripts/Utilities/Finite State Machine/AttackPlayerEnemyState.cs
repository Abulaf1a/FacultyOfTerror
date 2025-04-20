using Godot;
using System;

public partial class AttackPlayerEnemyState : EnemyState {

    public override void Enter(FiniteStateMachine stateMachine, EnemySprite enemy)
    {
        this.enemy = enemy;
        base.Enter(stateMachine);
    }

    public override void Update(double delta)
    {
        if(!enemy.fired)
		{
			enemy.TryFire(); 
		}

        base.Update(delta);
    }

    //Not currently called as movement state (FollowPlayerEnemyState) controls this state
    public override void Exit(string next) 
    {
        base.Exit(next);
    }


}