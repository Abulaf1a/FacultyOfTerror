using Godot;
using System;

public partial class PatrolWaitEnemyState : EnemyState{

    Timer timer; 
    Random wait;

    public override void Enter(FiniteStateMachine stateMachine)
    {
        GD.Print("patrol wait enemy state entered"); 

        timer = new Timer();

        AddChild(timer); 

        wait = new Random(); 

        timer.Start(wait.Next(3,8)); 

        timer.Autostart = true; 

        timer.Timeout += () => Exit("PatrolEnemyState"); 

        GD.Print("timer wait time = " + timer.WaitTime); 

        base.Enter(stateMachine);
    }

    public override void Update(double delta)
    {
        if(enemy.GlobalPosition.DistanceTo(player.GlobalPosition) < 9){
            Exit("FollowPlayerEnemyState"); 
        }
        base.Update(delta);
    }

    public override void Exit(string next)
    {
        RemoveChild(timer); 
        base.Exit(next);
    }
}