using Godot;
using Godot.NativeInterop;
using System;
using System.Linq.Expressions;

//note on C# syntax - having target in the class declaration is called a 'primary constructor',
//this is used if a type requires a parameter to create an instance, which is true here because 
//I don't want a projectile without a target! 
public partial class Projectile : RigidBody3D
{
    //die after a certain timer. 
    Vector3 target;

    Vector3 direction; 

    Vector3 _position = new Vector3(); 


    CollisionShape3D collision; 

    float step; 

    //helper static class allows me to create a projectile.
    public static Projectile NewProjectile(float step, PackedScene scene){

        var projectileInstance = scene.Instantiate<Projectile>(); 

        projectileInstance.SetStep(step); 

        projectileInstance.GravityScale = 0f; 

        return projectileInstance; 
    }

    public override void _Ready()
    {

        ProjectileManager.Register(this); 

        collision = GetNode<CollisionShape3D>("CollisionShape3D"); 
        base._Ready();
    } 

    public void SetStep(float step){

        this.step = step; 
    }

    public override void _PhysicsProcess(double delta)
    {
               
        Position += Transform.Basis * new Vector3(0,0,-step) * (float) delta; 

        base._PhysicsProcess(delta);
    }

    public void _on_body_entered(float step){
        GD.Print("on body entered called"); 

        QueueFree(); //todo create a recycler/ 
    }


}
