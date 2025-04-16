using Godot;
using Godot.NativeInterop;
using System;
using System.Formats.Tar;
using System.Linq.Expressions;


public partial class Projectile : RigidBody3D
{    
    private Boolean free; 

    private Vector3 target;

    private Vector3 direction; 

    private Vector3 _position = new Vector3(); 

    private CollisionShape3D collision; 

    //private float initialVelocity; 

    private float step; 
    
    private string pType;
    //helper static class to create projectile.
    public static Projectile NewProjectile(float step, PackedScene scene){

        var projectileInstance = scene.Instantiate<Projectile>(); 

        //projectileInstance.SetInitialVelocity(initialVelocity);

        projectileInstance.SetPType(scene.ResourceName);

        projectileInstance.SetStep(step); 

        projectileInstance.GravityScale = 0f; 

        return projectileInstance; 
    }

    // public void SetInitialVelocity(float initialVelocity){
    //     this.initialVelocity = initialVelocity; 
    // }

    public override void _Ready()
    {

        ProjectilePool.Register(this); 

        collision = GetNode<CollisionShape3D>("CollisionShape3D"); 
        base._Ready();
    } 

    public void SetStep(float step){

        this.step = step; 
    }

    public void SetPType(string pType){
        this.pType = pType; 
    }

    public string GetPType(){
        return pType; 
    }

    public override void _PhysicsProcess(double delta)
    {
               
        Position += Transform.Basis * new Vector3(0,0,-step) * (float) delta; 

        base._PhysicsProcess(delta);
    }

    public void _on_body_entered(Node body){

        //send damage signal to object hit.

        GD.Print("Projectile hit: " + body.Name);


        


        Release(); //releases the object from being used (disappear and pause)

    }

    public void Reuse(){ 

        Visible = true; 

        SetPhysicsProcess(true); 

        free = false; 
    }

    public Boolean IsFree(){
        return free; 
    }

    public void Release(){

        Visible = false; 

        Transform = new Transform3D();

        SetPhysicsProcess(false);

        free = true; 

    }


}
