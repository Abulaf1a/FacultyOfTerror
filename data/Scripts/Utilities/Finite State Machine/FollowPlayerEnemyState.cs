using Godot;
using System;

public partial class FollowPlayerEnemyState : EnemyState {

    public CharacterBody3D player; // target. 

    public override void Enter(FiniteStateMachine stateMachine)
    {
        // GD.Print("follow player state entered!"); 
        //NULL instance error - player is in separate scene, this is inside the enemy scene, not main scene!!
        //make a singleton to access from everywhere -- allow us to get player from here. 

        player = (CharacterBody3D) GetTree().GetNodesInGroup("Player")[0]; //gets the player from the scene tree 

        //get player target location
        base.Enter(stateMachine);
    }

    public override void Update(double delta)
    {
        //check if player is outside of standing range (e.g. 1 m)
        //calculate next position moving towards to player 
        //send position to enem

        GD.Print("running follow state"); 

        if(enemy.GlobalPosition.DistanceTo(player.GlobalPosition) < 5){
            
            Exit("IdleEnemyState"); 
            
            return; 
        }

        else
        {
            //setting targetPos is handled by the EnemySprite as it is necessary for multiple states
            //design wise I don't yet know - perhaps all enemy behaviour should be managed in state update functions
            //but for now general behaviour is handled in the enemy sprite PhysicsProcess() 

            //get velocity and gravity
			Godot.Vector3 velocity = enemy.Velocity;
			if (!enemy.IsOnFloor()) velocity.Y -= enemy.GetGravityFloat() * (float)delta;
		
			//set target position to player's in this physics tick position
			Vector3 targetLocal = enemy.GetRay().ToLocal(enemy.GetTargetPos());  
			enemy.GetRay().TargetPosition = targetLocal; 
			enemy.GetNav().TargetPosition = enemy.GetTargetPos();

			//apply position to velocity using nav mesh to avoid obstacles
			Godot.Vector3 direction = (enemy.GetNav().GetNextPathPosition() - enemy.GlobalPosition).Normalized();
			velocity.X = Mathf.Lerp(velocity.X, direction.X * enemy.GetSpeed(), 0.5f);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * enemy.GetSpeed(), 0.5f);

			enemy.Velocity = velocity;

			enemy.MoveAndSlide();

        }

        base.Update(delta);
    }

    public override void Exit(String next)
    {

        GD.Print("changing state to: " +  next); 
        stateMachine.EmitSignal(FiniteStateMachine.SignalName.ChangeState, next);

        base.Exit(next);
    }


}