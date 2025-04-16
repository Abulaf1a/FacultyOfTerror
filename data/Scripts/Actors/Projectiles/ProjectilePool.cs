using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class ProjectilePool : Node3D {

    /// <summary>
    /// Singleton class to manage the pool of projectiles in the game.
    /// This class is used by both the player and enemy actors to fire projectiles.
    /// 
    /// The class can fire a projectile from two overloaded methods, taking either a parent node, or a parent and separate velocity.
    /// This second overloaded method is useful for when the parent node does not have a Velocity (e.g. not a CharacterBody3D).
    /// 
    /// When a projectile is fired, the class first checks if there are fewer projectiles than the max
    /// If there are, it instantiates a new projectile and adds it to the pool.
    /// If the pool has reached max size, it checks if any of the projectiles in the pool are free.
    /// 
    /// If there are no free projectiles, it reuses the first projectile in the pool.
    /// 
    /// TODO: ensure a projectile of a different type cannot be reused! 
    /// 
    /// </summary>
    private static ProjectilePool projectilePool; 
    private static List<Projectile> projectiles; 
    private static PackedScene scene; 
    private static int limit = 1000; //extract to debug setting

    public ProjectilePool(){
        projectiles = new List<Projectile>(); 
    }

    public static ProjectilePool GetInstance(){
        if(projectilePool == null){
            projectilePool = new ProjectilePool();

             
            scene = GD.Load<PackedScene>("res://data/Assets/Sprites/ProjectileSprite.tscn"); 

        }
        return projectilePool; 
    }



    public static void Register(Projectile projectile){
        projectiles.Add(projectile); 

    }


    public static void Remove(Projectile projectile){
        if(projectiles.Contains(projectile)){
            projectiles.Remove(projectile); 
        }

    }

    // Used where the firing parent contains correct rotation but not velocity - (e.g. with player)
    public Projectile FireProjectile(float bulletSpeed, Node3D parent, Vector3 velocity, PackedScene projectileType){

        scene = projectileType;
        float forwardsVelocity = velocity.Dot(parent.GlobalTransform.Basis.Z); 

        return FireWithVelocity(bulletSpeed, parent, forwardsVelocity);
        
    }

    // Used where the firing parent contains both velocity and rotation information 
    public Projectile FireProjectile(float bulletSpeed, Node3D parent, PackedScene projectileType){

        scene = projectileType; 

        Vector3 velocity = GetVelocity(parent); //returns a velocity, if not a CB3D, returns 0,0,0.
        float forwardsVelocity = velocity.Dot(parent.GlobalTransform.Basis.Z); 
                
        return FireWithVelocity(bulletSpeed, parent, forwardsVelocity); 
    }

    private Projectile FireWithVelocity(float bulletSpeed, Node3D parent, float forwardsVelocity){
        Projectile projectile = GetProjectileToUse(bulletSpeed, forwardsVelocity); 

        if(!projectile.IsInsideTree()){
            parent.GetTree().Root.AddChild(projectile); 
        }

        projectile.GlobalTransform = parent.GlobalTransform;

        return projectile; 
        
    }

    //Returns the velocity of the parent node by casting to CharacterBody3D, returning 0,0,0 if not.
    private Vector3 GetVelocity(Node3D parent){

        var parentCharacter = parent as CharacterBody3D;

        if(parentCharacter != null){

            GD.Print("parent character velocity: " + parentCharacter.Velocity);
            return parentCharacter.Velocity; 
        }


        GD.Print("parent is not a character body, velocity: " + parent.GlobalTransform.Basis.Z);
        return new Vector3(); // if not a character body, return 0 velocity. I assume initialising it like this will initialise to 0,0,0. 
        
    }

    //Gets a free projectile, either instantiating or reusing
    private Projectile GetProjectileToUse(float bulletSpeed, float forwardsVelocity){

        GD.Print(projectiles.Count + " projectiles in scene"); 


        if(projectiles.Count < limit){

            Projectile projectile = Projectile.NewProjectile(bulletSpeed - forwardsVelocity, scene); 
            projectiles.Add(projectile); 

            return projectile; 
        }

        int position = CheckFreeObjectExists(); 


        //still doesn't deal with checking the projectile at position 0 is the same type as the projectile to instantiate.
        if(position > 0){
            projectiles[position].Reuse(); 
            projectiles[position].SetStep(bulletSpeed); 
            return projectiles[position];
        } 
      
        projectiles[0].Reuse();
        projectiles[0].SetStep(bulletSpeed); 

        return projectiles[0];

        
    }

    //Called by GetProjectileToUse to check if an existing projectile is free. 
    int CheckFreeObjectExists(){


        for(int i = 0; i < projectiles.Count; i++){
            if(projectiles[i].IsFree() && projectiles[i].GetPType() == scene.ResourceName){
                return i; 
            }
        }
        return -1; 
    }
}