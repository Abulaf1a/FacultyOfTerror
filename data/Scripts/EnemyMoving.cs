using Godot;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
#nullable enable
public partial class EnemyMoving : BaseActor
{

	/// <summary>
	/// TODO: Separate animation functions into separate class -- this class should deal with enemy movement ONLY 
	/// Separate movement targets to separate class - target acquisition class, the enemy should then communicate between classes using INSTANCE 
	/// that are connected in _Ready() function. 
	/// 
	/// ALL CLASSES SHOULD HAVE ONE JOB AND ONE JOB ONLY
	/// A FINITE STATE MACHINE CLASS can be used to control and synchronise actions between these classes!
	/// 
	/// S
	/// 
	/// Enemy
	/// - movement
	/// - target selection
	/// - animation selection
	/// - state machine
	/// 
	/// 
	/// The states should be:
	/// - walking 
	/// 	- following target
	/// 	- following player
	/// - attacking
	/// - dead
	/// - resting
	/// 
	/// </summary>

	[Signal]
	public delegate void AttackPlayerEventHandler();

	[Signal]
	public delegate void AttackPlayerDamageIndicatorEventHandler();
	private AnimationPlayer? animationPlayer;
	private CollisionShape3D? collision;
	private Skeleton3D? skeleton;
	private Transform3D bonePose;
	[Export] private float headBob = 5f;
	[Export] private int targetDetectionRadius = 5;
	private Godot.Collections.Array navMesh;
	private int neckIdx;
	private float resetWanderTarget = 0;
	private Random rand = new Random();
	private Godot.Collections.Array<Node> wanderTargets;
	private bool wandering;
	private Timer timer;

	public override void _Ready()
	{
		try
		{
			mesh = GetNode<Node3D>("Armature");

			skeleton = GetNode<Skeleton3D>("Armature/Skeleton3D");

			collision = GetNode<CollisionShape3D>("Armature/CollisionShape3D");

			animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

			ray = GetNode<RayCast3D>("RayCast3D");

			nav = GetNode<NavigationAgent3D>("NavigationAgent3D");

			animationPlayer.Play("Lurch");

			animationPlayer.SpeedScale = 1f;

			neckIdx = skeleton.FindBone("Upper neck");

			bonePose = skeleton.GetBoneGlobalPose(neckIdx);

			wanderTargets = GetTree().GetNodesInGroup("Marker");

			targetPos = target.Position;

		}
		catch { Debug.WriteLine("EnemyMoving nodes broken, check addresses in _Ready()."); }

		base._Ready();

	}

	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector3 velocity = Velocity;

		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

		UpdatePlayerDist();

		BodyTrack();

		HeadTrack();

		if (distToTarget > targetDetectionRadius)
		{
			if (wandering == false)
			{
				UpdateWanderTargetPos();
				wandering = true;
			}

			if (animationPlayer?.CurrentAnimation != "Lurch") animationPlayer?.Play("Lurch");

			targetPos.Y = GlobalPosition.Y;

		}

		else if (distToTarget <= targetDetectionRadius)
		{
			if (animationPlayer.CurrentAnimation != "Lurch") animationPlayer.Play("Lurch");

			UpdateTargetPos();

			if (wandering == true) wandering = false;

		}

		else
		{
			if (animationPlayer.CurrentAnimation != "Bite") animationPlayer.Play("Bite");

			BeginPlayerAttack();
		}

		nav.TargetPosition = targetPos;

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

	async void BeginPlayerAttack()
	{

		await ToSignal(GetTree().CreateTimer(animationPlayer.CurrentAnimationLength * 2), "timeout");

		EmitSignal(SignalName.AttackPlayer);
	}

	void BodyTrack()
	{
		ray.LookAt(targetPos, null, true);

		Transform3D meshTransform = mesh.Transform;

		Transform3D rayTransform = ray.Transform;

		Godot.Quaternion a = meshTransform.Basis.Orthonormalized().GetRotationQuaternion();

		Godot.Quaternion b = rayTransform.Basis.Orthonormalized().GetRotationQuaternion();

		Godot.Quaternion c = a.Slerp(b, 0.02f);//change slerp angle based on distance? 
											   //if (a.Y > b.Y) GD.Print("left"); //beyond threshold of X, play either left or right turn animation when stationary. 
											   //else GD.Print("right");
		meshTransform.Basis = new Basis(c);

		mesh.Transform = meshTransform;
	}

	void HeadTrack()
	{
		Transform3D bonePoseMod = skeleton.GlobalTransform * bonePose;

		Godot.Vector3 playerPosLow = target.GlobalPosition;

		playerPosLow.Y -= headBob;

		bonePoseMod = bonePoseMod.LookingAt(playerPosLow);

		Godot.Vector3 scale = mesh.Scale; //doesn't work -- scale of head is wrong. 

		bonePoseMod = bonePoseMod.ScaledLocal(scale);

		skeleton.SetBoneGlobalPoseOverride(neckIdx, skeleton.GlobalTransform.AffineInverse() * bonePoseMod, 1f, true);
	}

	public void UpdatePlayerDist()
	{
		distToTarget = GlobalPosition.DistanceTo(targetPos);
	}

	public void UpdateTargetPos()
	{
		targetPos = target.GlobalPosition;
		//targetPos.Y = GlobalPosition.Y;
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