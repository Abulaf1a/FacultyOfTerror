using Godot;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
#nullable enable
public partial class EnemyMoving : BaseActor
{
	[Signal]
	public delegate void AttackPlayerEventHandler();

	[Signal]
	public delegate void AttackPlayerDamageIndicatorEventHandler();

	//animation attributes for Mantis
	AnimationPlayer? animationPlayer;
	CollisionShape3D? collision;
	Skeleton3D? skeleton;
	Transform3D bonePose;
	[Export] float headBob = 5f;
	[Export] int targetDetectionRadius = 5;
	Godot.Collections.Array navMesh;
	int neckIdx;
	float resetWanderTarget = 0;
	Random rand = new Random();
	Godot.Collections.Array<Node> wanderTargets;
	Boolean wandering;

	Timer timer;
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

			UpdateWanderTargetPos();

			GD.Print("array size is: " + wanderTargets.Count); //WHY ARE THESE NOT PRINTING!! 
			GD.Print("pos 0 is" + wanderTargets[0]);

		}
		catch { Debug.WriteLine("EnemyMoving nodes broken, check addresses in _Ready(). Enemy moving is totally borkee!"); }

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
			if (wandering = false)
			{
				UpdateWanderTargetPos();
				GD.Print("player out of range resetting to " + targetPos);
				wandering = true;
			}

			if (animationPlayer?.CurrentAnimation != "Lurch") animationPlayer?.Play("Lurch");

			targetPos.Y = GlobalPosition.Y;

			//resetWanderTarget += (float)delta; //counter before actormarker signals were implemented

			nav.TargetPosition = targetPos;
		}

		else if (distToTarget <= targetDetectionRadius)
		{
			if (animationPlayer.CurrentAnimation != "Lurch") animationPlayer.Play("Lurch");

			UpdateTargetPos();

			if (wandering == true) wandering = false;

			nav.TargetPosition = targetPos;
		}
		else
		{
			if (animationPlayer.CurrentAnimation != "Bite") animationPlayer.Play("Bite");
			//await ToSignal(GetTree().CreateTimer(animationPlayer.CurrentAnimationLength * 2), "timeout");
			EmitSignal(SignalName.AttackPlayer);
			//each update loop, cache delta time, get the new one, subtract the two, giving you the delta delta. 
		}

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
		//HEAD TRACKING 
		//- instant
		Transform3D bonePoseMod = skeleton.GlobalTransform * bonePose;
		Godot.Vector3 playerPosLow = target.GlobalPosition;
		playerPosLow.Y -= headBob;
		bonePoseMod = bonePoseMod.LookingAt(playerPosLow); //'use model front' helps when meshes are rotated 180 deg!
		Godot.Vector3 scale = mesh.Scale; //doesn't work -- scale of head is wrong. 

		//resets scale as it has been discarded in the operation
		bonePoseMod = bonePoseMod.ScaledLocal(scale);
		skeleton.SetBoneGlobalPoseOverride(neckIdx, skeleton.GlobalTransform.AffineInverse() * bonePoseMod, 1f, true);
	}

	public void UpdatePlayerDist()
	{
		distToTarget = GlobalPosition.DistanceTo(targetPos);

		GD.Print("dist to target: "
		+ distToTarget + " distance to player: " + GlobalPosition.DistanceTo(target.GlobalPosition));
	}

	public void UpdateTargetPos()
	{
		targetPos = target.GlobalPosition;
		targetPos.Y = GlobalPosition.Y;
	}

	public void _on_actor_marker_body_entered(Node3D a)
	{

		GD.Print("Collision shape 3d entered");
		UpdateWanderTargetPos();


	}
}