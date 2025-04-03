using System.Collections.Generic;
using System.Threading;
using Godot;

public partial class ProjectileManager : Node3D {


    static List<Projectile> pool; 

    public ProjectileManager(){

        pool = new List<Projectile>(); 

    }

    public static void Register(Projectile projectile){

        pool.Add(projectile); 

    }


    public static void Remove(Projectile projectile){

        if(pool.Contains(projectile)){
            pool.Remove(projectile); 
        }

    }

    


}