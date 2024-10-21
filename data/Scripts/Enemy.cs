using Godot;
using System;
using System.Diagnostics;
public partial class Enemy : BaseActor
{

	public AnimationPlayer animationPlayer;
	CollisionShape3D collision;
	bool visible;
	bool pause;

	double angerTimer;
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		collision = GetNode<CollisionShape3D>("CollisionShape3D");
		ray = GetNode<RayCast3D>("RayCast3D");
		nav = GetNode<NavigationAgent3D>("NavigationAgent3D");
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		Godot.Vector3 playerPos = target.GlobalPosition;
		bool hitPlayer = RayCastToPlayer(playerPos);
		if (hitPlayer)
		{
			UpdateAnimation();
		}
		//only need to change anims if in player's pov/ around player
		angerTimer = UpdateAngerTimer(playerPos);


		if (GlobalPosition.DistanceTo(playerPos) > 2 && !pause)
		{
			nav.TargetPosition = playerPos;
			Vector3 direction = (nav.GetNextPathPosition() - GlobalPosition).Normalized();
			LookAt(playerPos);
			//split out: no y velocity. 
			velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, 0.5f);
			velocity.Z = Mathf.Lerp(velocity.Z, direction.Z * Speed, 0.5f);
			RotateY(Mathf.DegToRad(180f));//Enemy is rotated due to incorrect rotation in editor. 
			Velocity = velocity;
			MoveAndSlide();
		}
	}

	public bool RayCastToPlayer(Godot.Vector3 playerPos)
	{
		ray.LookAt(playerPos, null, true);
		GodotObject c = ray.GetCollider();
		if (c.GetClass() != "CharacterBody3D") //ray hit player. 
		{
			return false;
		}
		return true;
	}

	public void UpdateAnimation()
	{
		if (!visible) //not in player's Fov or anger timer has reached 100 
		{
			pause = false;
			if (!animationPlayer.IsPlaying()) animationPlayer.Play("Run_001");
		}
		else if (angerTimer < 100)
		{
			pause = true;
			animationPlayer.Pause();
		}
		else //angry! 
		{
			if (pause == true) pause = false;
			if (!animationPlayer.IsPlaying()) animationPlayer.Play("Run_001");
		}
	}

	public double UpdateAngerTimer(Godot.Vector3 playerPos)
	{
		//based on distance betweeen player and enemy
		//and amount of time player looks at enemy
		if (visible && pause)
		{
			double distance = GlobalPosition.DistanceTo(playerPos);
			double k = 5.0;
			if (distance < 5)
			{
				angerTimer += Math.Exp(distance / k);
			}
		}
		else if (!visible)
		{
			if (angerTimer > 0) angerTimer--;
		}
		return angerTimer;
	}


	public async void _on_visible_on_screen_notifier_3d_screen_entered() //doesn't work for objects in the way. 
	{
		await ToSignal(GetTree().CreateTimer(0.1), "timeout"); //extra creep value
		visible = true;
	}
	public void _on_visible_on_screen_notifier_3d_screen_exited()
	{
		visible = false;
		pause = false;
		animationPlayer.Play("Run_001");
	}
	//if player gets within 2 metres of enemy, enemy scoops player of the ground and eats them! 
}
