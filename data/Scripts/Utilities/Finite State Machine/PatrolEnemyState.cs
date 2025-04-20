using Godot;
using System;
using System.Collections.Generic;
using System.Runtime;

public partial class PatrolEnemyState : EnemyState {

    Random random;
    Random sign; 

    public override void Enter(FiniteStateMachine stateMachine)
    {
        enemy = GetOwner<EnemySprite>(); 

        random = new Random();

        sign = new Random(); 

        enemy._Ready(); 

        //trying to do recursively in the first instance will cause a crash due to navigation map not being loaded?
        Vector3 target = GenerateRandomVector3(); 

        //set enemy target and navigation target to generated target
        enemy.SetTargetPos(target); 
        
        enemy.GetNav().TargetPosition = target;

        base.Enter(stateMachine);
    }

    Vector3 GenerateRandomVector3(){
        float currentX = enemy.GlobalPosition.X; 
        float currentZ = enemy.GlobalPosition.Z; 

        float newX;
        float newZ; 

        if(sign.Next(0,2) == 0)
        {
            newX = currentX - ((float)random.NextDouble())*4f; 
        }
        else
        {
            newX = currentX + ((float)random.NextDouble())*4f; 
        }

        if(sign.Next(0,2) == 0)
        {
            newZ = currentZ - ((float)random.NextDouble())*4f;
        }
        else
        {
            newZ = currentZ + ((float)random.NextDouble())*4f; 
        }
        
        return new Vector3(newX,enemy.GlobalPosition.Y,newZ); 
    }

    // bool IsPositionReachable(Vector3 pos){

    //     Vector3 currentTarget = enemy.GetNav().TargetPosition; 

    //     enemy.GetNav().TargetPosition = pos; 

    //     bool isReachable = enemy.GetNav().IsTargetReachable(); //returns true if target is within 'desired distance' which is set in editor (currently 1m)

    //     GD.Print("current location " + enemy.GlobalPosition); 
    //     GD.Print("target " + pos + " is reachable?" + isReachable); 

    //     enemy.GetNav().TargetPosition = currentTarget; 

    //     return isReachable; 
    // }

    public override void Update(double delta)
    {  
        if(enemy.GlobalPosition.DistanceTo(player.GlobalPosition) < 9){
            Exit("FollowPlayerEnemyState"); 
        }
        else if(enemy.GetNav().IsNavigationFinished())
        {
            Exit("PatrolWaitEnemyState"); 
        }


        Godot.Vector3 velocity = enemy.Velocity;
        if (!enemy.IsOnFloor()) velocity.Y -= enemy.GetGravityFloat() * (float)delta;

        Godot.Vector3 direction = (enemy.GetNav().GetNextPathPosition() - enemy.GlobalPosition).Normalized();
        velocity.X = Mathf.Lerp(velocity.X, direction.X * enemy.WanderSpeed, 0.5f);
        velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * enemy.WanderSpeed, 0.5f);

        enemy.Velocity = velocity;
        enemy.MoveAndSlide();

        base.Update(delta);
    }

    public override void Exit(string next)
    {
        base.Exit(next);
    }
}