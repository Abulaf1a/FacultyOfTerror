using Godot;
using System; 


//FSM should continually check if the enemy has less than 0 health, and transition to dead if so? 
public partial class DeadEnemyState : EnemyState {


    public override void Enter(FiniteStateMachine stateMachine)
    {
        GD.Print("enemy is dead!");
        //change sprite! 
        base.Enter(stateMachine);
    }

    public override void Update(double delta)
    {
        base.Update(delta);
    }

    public override void Exit(string next)
    {
        base.Exit(next);
    }
}