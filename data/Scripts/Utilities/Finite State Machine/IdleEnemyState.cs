using Godot;
using System; 

public partial class IdleEnemyState : EnemyState {

    AttackPlayerEnemyState attackState;

    public override void _Ready()
    {
        attackState = GetParent().GetNode<AttackPlayerEnemyState>("AttackPlayerEnemyState");
        base._Ready();
    }

    public override void Enter(FiniteStateMachine stateMachine)
    {
        stateMachine.SetAttackState(attackState); 
        base.Enter(stateMachine);
    }

  public override void Update(double delta)
    {
        //called from FSM! 
        // attackState.Update(delta);

        enemy.GetTargetPos(); 

        if(enemy.Position.DistanceTo(enemy.GetTargetPos()) > 3)
        {
            Exit("FollowPlayerEnemyState");
        }
        
        base.Update(delta);
    }

    public override void Exit(String next)
    {
        stateMachine.SetAttackState(null); 
        base.Exit(next);
    }
}