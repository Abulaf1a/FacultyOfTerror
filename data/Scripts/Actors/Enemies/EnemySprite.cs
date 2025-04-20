using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


public partial class EnemySprite : EnemyActor
{
	/// <summary>
	/// Class for sprite enemy. Contains ready, physics process, and firing logic.
	/// </summary>
	[Signal]
	public delegate void AttackPlayerEventHandler();
	[Signal]
	public delegate void AttackPlayerDamageIndicatorEventHandler();
	private CollisionShape3D collision;
	[Export] private int targetDetectionRadius = 5;
	[Export] private float bulletSpeed = 3f; 
	private Godot.Collections.Array navMesh;
	private float resetWanderTarget = 0;
	private Random rand = new Random();
	private Godot.Collections.Array<Node> wanderTargets;
	private bool wandering;
	private Timer timer;
	public bool fired {get; private set;}
	private AnimatedSprite3D sprite;
	PackedScene projectile;


	[Export] public float WanderSpeed {get; private set;}
	
	public override void _Ready()
	{
		fired = false; 
		try
		{
			projectile = GD.Load<PackedScene>("res://data/Assets/Sprites/ProjectileSprite.tscn"); 
			collision = GetNode<CollisionShape3D>("CollisionShape3D");
			ray = GetNode<RayCast3D>("RayCast3D");
			nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
            wanderTargets = GetTree().GetNodesInGroup("Marker");
			timer = GetNode<Timer>("Timer"); 

			finiteStateMachine = GetNode<FiniteStateMachine>("FiniteStateMachine"); 
		}
		catch(Exception e) 
		{ 
			GD.Print("error in enemy sprite: " + e.Message); 
		}

		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{

		targetPos = target.GlobalPosition; //used by all states

		LookAt(targetPos); //required as projectiles are fired inheriting parent orientation. 
	}

	//named GetGravityFloat to avoid hiding a PhysicsBody3D method GetGravity()
	public float GetGravityFloat()
	{
		return gravity; 
	}
	//called by attack state 
	public bool TryFire()
	{

		GodotObject c = ray.GetCollider();

		if(c is Node3D)
		{
			Node3D check = (Node3D)c; 

			if(check.IsInGroup("Player") && distToTarget < targetDetectionRadius)
			{
				FireProjectile(); 
				timer.Start(); 

				fired = true; 
				return true; 
			}
			else if(check.IsInGroup("Enemy"))
			{
				return false; 
			}
		}

		return false; 
	}

	private void FireProjectile()
	{
		ProjectilePool projectilePool = ProjectilePool.GetInstance(); 

		projectilePool.FireProjectile(bulletSpeed, this, GD.Load<PackedScene>("res://data/Assets/Sprites/ProjectileSprite.tscn"));
    }

	public void SetTargetPos(Vector3 newTarget)
	{
		targetPos = newTarget; 
	}
	
	public void _on_timer_timeout()
	{
		fired = false; 
	}

	public RayCast3D GetRay()
	{
		return ray; 
	}

	public NavigationAgent3D GetNav()
	{
		return nav; 
	}

	public float GetSpeed()
	{
		return Speed;
	}

	public Vector3 GetTargetPos()
	{
		return targetPos; 
	}
}