using Godot;
using System;

public partial class EnemySprite : EnemyActor
{
	[Signal]
	public delegate void AttackPlayerEventHandler();
	[Signal]
	public delegate void AttackPlayerDamageIndicatorEventHandler();
	private CollisionShape3D? collision;
	[Export] private int targetDetectionRadius = 5;
	private Godot.Collections.Array navMesh;

    //wander targets
	private float resetWanderTarget = 0;
	private Random rand = new Random();
	private Godot.Collections.Array<Node> wanderTargets;
	private bool wandering;
	private Timer timer;

	public override void _Ready()
	{
		try
		{

			collision = GetNode<CollisionShape3D>("CollisionShape3D");

			ray = GetNode<RayCast3D>("RayCast3D");

			nav = GetNode<NavigationAgent3D>("NavigationAgent3D");

			targetPos = target.Position;

            wanderTargets = GetTree().GetNodesInGroup("Marker");

		}
		catch { GD.Print("error"); }

		base._Ready();

	}

	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = Velocity;
		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

		UpdatePlayerDist();
		
		//Mantis specific logic for attacking the player.
		// if (distToTarget > targetDetectionRadius)
		// {
		// 	if (wandering == false)
		// 	{
		// 		UpdateWanderTargetPos();
		// 		wandering = true;
		// 	}
		// 	targetPos.Y = GlobalPosition.Y;

		// }

		//else if (distToTarget <= targetDetectionRadius)
		//{
			UpdateTargetPos();
		// 	if (wandering == true) wandering = false;
		// }

		nav.TargetPosition = targetPos;
        GD.Print("Target pos" + targetPos); 
		Godot.Vector3 direction = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
		velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, 0.5f);
		velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, 0.5f);
		Velocity = velocity;
		MoveAndSlide();
	}

	void UpdateWanderTargetPos()
	{
		int i = rand.Next(0, wanderTargets.Count);

		targetPos = (wanderTargets[i] as Node3D).GlobalPosition;

		targetPos.Y = GlobalPosition.Y;

		GD.Print(targetPos);
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
			GD.Print("player entered actormarker");
		}
		else if (actor.Name == "Mantis")
		{
			GD.Print("mantis entered actormarker");
			UpdateWanderTargetPos();
		}
	}
}

