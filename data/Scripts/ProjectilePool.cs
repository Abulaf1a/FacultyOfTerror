using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Godot;

public partial class ProjectilePool : Node3D {


    static ProjectilePool projectilePool; 


    static List<Projectile> projectiles; 

    static PackedScene scene; 

    static int limit = 10; 

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

    public Projectile FireProjectile(float bulletSpeed, Transform3D transform){

        GD.Print("pool count: " + projectiles.Count);

        if(projectiles.Count < limit){

            Projectile projectile = Projectile.NewProjectile(bulletSpeed, scene); 

            projectiles.Add(projectile); 

            return projectile; 
        }

        int position = CheckFreeObjectExists(); 

        if(position > 0){
            projectiles[position].Reuse(); 
            return projectiles[position];
        } 
      
        projectiles[0].Reuse();
        return projectiles[0];
        
        //if a free projectile exists in the pool, use it, change the location etc. 

        //else, if there are fewer projectiles than the limit, add a new projectile

        //else delete/reuse the oldest projectile or projectile furthest from the player
        //if using furthest from player - i need to figure out how to quickly work out furthest projectile from player. 


        //location and target passed into this method from enemysprite. 


        //
    }


    int CheckFreeObjectExists(){

        for(int i = 0; i < projectiles.Count; i++){
            if(projectiles[i].IsFree()){
                return i; 
            }
        }
        return -1; 
    }




    


}