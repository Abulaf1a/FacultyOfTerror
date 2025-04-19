using Godot;
using System;
using System.Diagnostics;


public partial class EnemySprite : EnemyActor
{
	/// <summary>
	/// Class for sprite enemy. Contains ready, physics process, and firing logic.
	/// 
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
	private bool fired; 
	private AnimatedSprite3D sprite;
	PackedScene projectile;
	public override void _Ready()
	{
		fired = false; 
		try
		{
			projectile = GD.Load<PackedScene>("res://data/Assets/Sprites/ProjectileSprite.tscn"); 
			collision = GetNode<CollisionShape3D>("CollisionShape3D");
			ray = GetNode<RayCast3D>("RayCast3D");
			nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
			// targetPos = target.Position; //unused, kept for reference. Now handled by FSM
            wanderTargets = GetTree().GetNodesInGroup("Marker");
			timer = GetNode<Timer>("Timer"); 
		}
		catch(Exception e) 
		{ 
			GD.Print("error in enemy sprite: " + e.Message); 
		}

		base._Ready();
		
	}

	 //design wise I don't yet know - perhaps all enemy behaviour should be managed in state update functions
	//but for now general behaviour is handled in the enemy sprite PhysicsProcess() 
	public override void _PhysicsProcess(double delta)
	{
		if (enemyState == EnemyStateEnum.DEAD) return; 

		targetPos = target.GlobalPosition; //again used by all states

		//firing behaviour - to move into separate STATE! 
		if(!fired)
		{
			TryFire(); 
		}

		LookAt(targetPos); //required as projectiles are fired inheriting parent orientation. 

	}

	//called GetGravityFloat to avoid hiding a PhysicsBody3D method GetGravity()
	public float GetGravityFloat(){
		return gravity; 
	}
	//called every physics tick between timer timeout and a successful fire. 
	bool TryFire(){

		GodotObject c = ray.GetCollider();

		if(c is Node3D){
			Node3D check = (Node3D)c; 
			if(check.IsInGroup("Player") && distToTarget < targetDetectionRadius){

				FireProjectile(); 

				timer.Start(); 

				fired = true; 

				return true; 
			}
			else if(check.IsInGroup("Enemy")){
				return false; 

			}
		}

		return false; 
	}

	void FireProjectile(){

		ProjectilePool projectilePool = ProjectilePool.GetInstance(); 

		projectilePool.FireProjectile(bulletSpeed, this, GD.Load<PackedScene>("res://data/Assets/Sprites/ProjectileSprite.tscn"));

    }

	public void SetTargetPos(Vector3 newTarget){
		targetPos = newTarget; 
	}
	
	public void _on_timer_timeout(){

		fired = false; 

	}

	public RayCast3D GetRay(){
		return ray; 
	}

	public NavigationAgent3D GetNav(){
		return nav; 
	}

	public float GetSpeed(){
		return Speed;
	}

	public Vector3 GetTargetPos(){
		return targetPos; 
	}

	  // enemy.GetRay
            // enemy.GetNav
            // enemy.GetSpeed
            // enemy.GetTargetPos


	//previously used before FSM

	// void UpdateWanderTargetPos()
	// {
	// 	int i = rand.Next(0, wanderTargets.Count);

	// 	targetPos = (wanderTargets[i] as Node3D).GlobalPosition;

	// 	targetPos.Y = GlobalPosition.Y;
	// }

	// public void UpdatePlayerDist()
	// {
	// 	distToTarget = GlobalPosition.DistanceTo(targetPos);
	// }

	// public void UpdateTargetPos()
	// {
	// 	targetPos = target.GlobalPosition;
	// }



	// Also unused

	// public void _on_actor_marker_body_entered(Node3D actor)
	// {
	// 	if (actor.Name == "Player")
	// 	{
	// 		//GD.Print("player entered actormarker");
	// 	}
	// 	else if (actor.Name == "Mantis")
	// 	{
	// 		//GD.Print("mantis entered actormarker");
	// 		// UpdateWanderTargetPos();
	// 	}
	// }
}