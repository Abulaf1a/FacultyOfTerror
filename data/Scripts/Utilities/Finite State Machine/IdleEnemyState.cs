using Godot;
using System; 

public partial class IdleEnemyState : EnemyState {



    public override void Update(double delta)
    {

        // GD.Print("running idle state"); 

        enemy.GetTargetPos(); 

        if(enemy.Position.DistanceTo(enemy.GetTargetPos()) > 6)
        {
            Exit("FollowPlayerEnemyState");
        }

        enemy.Velocity = new Vector3(0,0,0); //enemy doesn't stop ?? why??? 
        enemy.MoveAndSlide();  
        
        //logic to happen every physics tick happens here.
        //this is determined in the State class, where the Update function is called from _PhysicsProcess();

        base.Update(delta);
    }


    public override void Enter(FiniteStateMachine stateMachine)
    {

        
        //enemy.SetTargetPos(enemy.GlobalPosition); 
        //logic for when the state is entered happens here
        base.Enter(stateMachine);
    }

    public override void Exit(String next)
    {
        if(stateMachine != null)
        {
                    stateMachine.EmitSignal(FiniteStateMachine.SignalName.ChangeState, next);
        }

        //logic for when the state is exited happens here. 
        base.Exit(next);
    }
}