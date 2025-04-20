using Godot;
using System;

public partial class FollowPlayerEnemyState : EnemyState {

    // public CharacterBody3D player; 

    AttackPlayerEnemyState attackState; //the follow playerenemystate attacks player. 

    public override void _Ready()
    {
        attackState = GetParent().GetNode<AttackPlayerEnemyState>("AttackPlayerEnemyState"); 

        base._Ready();
    }

    public override void Enter(FiniteStateMachine stateMachine)
    {
        stateMachine.SetAttackState(attackState); 

        //now accessible by all states
        // player = (CharacterBody3D) GetTree().GetNodesInGroup("Player")[0]; //gets the player from the scene tree 

        base.Enter(stateMachine);
    }

    public override void Update(double delta)
    {
        //check if player is outside of standing range (e.g. 1 m)
        //calculate next position moving towards to player 
        //send position to enem

        // called from FSM
        // attackState.Update(delta); 

        GD.Print("running follow state"); 

        if(enemy.GlobalPosition.DistanceTo(player.GlobalPosition) < 2){
            
            Exit("IdleEnemyState"); 
            
            return; 
        }
        else if(enemy.GlobalPosition.DistanceTo(player.GlobalPosition) > 10){
            Exit("PatrolEnemyState"); 
        }

        else
        {
            //setting targetPos is handled by the EnemySprite as it is necessary for multiple states
            //design wise I don't yet know - perhaps all enemy behaviour should be managed in state update functions
            //but for now general behaviour is handled in the enemy sprite PhysicsProcess() 
            //set target position to player's in this physics tick position
			Vector3 targetLocal = enemy.GetRay().ToLocal(enemy.GetTargetPos());  

			enemy.GetRay().TargetPosition = targetLocal; 

			enemy.GetNav().TargetPosition = enemy.GetTargetPos();

            //get velocity and gravity
			Godot.Vector3 velocity = enemy.Velocity;
			if (!enemy.IsOnFloor()) velocity.Y -= enemy.GetGravityFloat() * (float)delta;
		
			

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
        stateMachine.SetAttackState(null); 

        GD.Print("changing state to: " +  next); 

        base.Exit(next);
    }


}