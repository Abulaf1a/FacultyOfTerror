using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class ProjectilePool : Node3D {


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

    public Projectile FireProjectile(float bulletSpeed, Node3D parent, PackedScene projectileType){

        scene = projectileType; 

        Projectile projectile = GetProjectileToUse(bulletSpeed); 

        GD.Print("projectile: " + projectile.Name);

        if(!projectile.IsInsideTree()){
            parent.GetTree().Root.AddChild(projectile);
            
        }

        projectile.GlobalTransform = parent.GlobalTransform;

        return projectile; 

    }

    private Projectile GetProjectileToUse(float bulletSpeed){

        GD.Print(projectiles.Count + " projectiles in scene"); 


        if(projectiles.Count < limit){

            Projectile projectile = Projectile.NewProjectile(bulletSpeed, scene); 
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

    int CheckFreeObjectExists(){


        for(int i = 0; i < projectiles.Count; i++){
            if(projectiles[i].IsFree() && projectiles[i].GetPType() == scene.ResourceName){
                return i; 
            }
        }
        return -1; 
    }
}