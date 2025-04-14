using Godot;
using System;
using System.Diagnostics;


public partial class EnemySprite : EnemyActor
{
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
			targetPos = target.Position;
            wanderTargets = GetTree().GetNodesInGroup("Marker");
			timer = GetNode<Timer>("Timer"); 

		}
		catch(Exception e) { 

			GD.Print("error in enemy sprite: " + e.Message); 

		}

		base._Ready();

	}

	//enemy firing logic, 
	// if player is within X range, and cooldown timer has expired, and ray to player intercepts player, 
	// fire. 


	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = Velocity;
		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;
		UpdatePlayerDist();


		Vector3 targetLocal = ray.ToLocal(targetPos);  

		ray.TargetPosition = targetLocal; 

		if(!fired){

			TryFire(); 

			


		}

		UpdateTargetPos();

		nav.TargetPosition = targetPos;
		Godot.Vector3 direction = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
		velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, 0.5f);
		velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, 0.5f);
		Velocity = velocity;
		MoveAndSlide();

		LookAt(targetPos); 
	}

	//called every physics tick between timer timeout and a successful fire. 
	bool TryFire(){

		GodotObject c = ray.GetCollider();

		if(c is Node3D){
			Node3D check = (Node3D)c; 
			if(check.IsInGroup("Player") && distToTarget < targetDetectionRadius){
				Debug.WriteLine("is in player group" ); 

				FireProjectile(); 

				timer.Start(); 

				fired = true; 

				return true; 
			}
		}
		
		return false; 
	}

	void FireProjectile(){

		ProjectilePool projectilePool = ProjectilePool.GetInstance(); 

		var projectileInstance = projectilePool.FireProjectile(bulletSpeed, Transform);

		if(!projectileInstance.IsInsideTree()){
			AddSibling(projectileInstance); 
		}

		projectileInstance.Transform = Transform; 

		projectileInstance.GlobalPosition = GlobalPosition;
    }

	void UpdateWanderTargetPos()
	{
		int i = rand.Next(0, wanderTargets.Count);

		targetPos = (wanderTargets[i] as Node3D).GlobalPosition;

		targetPos.Y = GlobalPosition.Y;
	}

	public void UpdatePlayerDist()
	{
		distToTarget = GlobalPosition.DistanceTo(targetPos);
	}

	public void UpdateTargetPos()
	{
		targetPos = target.GlobalPosition;
	}
	
	public void _on_actor_marker_body_entered(Node3D actor)
	{
		if (actor.Name == "Player")
		{
			//GD.Print("player entered actormarker");
		}
		else if (actor.Name == "Mantis")
		{
			//GD.Print("mantis entered actormarker");
			UpdateWanderTargetPos();
		}
	}

	public void _on_timer_timeout(){

		fired = false; 

		
	}
}