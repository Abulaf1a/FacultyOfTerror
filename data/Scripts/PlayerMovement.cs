using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D
{
	[Export] public float Speed = 5.0f;
	[Export] public float SprintSpeed = 8.0f;
	[Export] public float JumpVelocity = 4.5f;
	public bool Sprinting;
	public bool Crouching;
	public float Stamina = 100f;
	int health = 100;
	public float mouseSensitivity = 0.01f;
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	string cachedLastCol;
	string lastCol;
	public Node3D head;
	public Camera3D camera;
	float fovReset;
	float headReset;
	public CollisionShape3D standingCollision;
	public CollisionShape3D crouchCollision;

	TextureRect tex;

	public Marker3D headDeathMarker;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		base._Ready();
		head = GetNode<CollisionShape3D>("Head");
		camera = GetNode<Camera3D>("Head/Camera3D");
		standingCollision = GetNode<CollisionShape3D>("StandingCollision");
		crouchCollision = GetNode<CollisionShape3D>("CrouchCollision");
		headDeathMarker = GetNode<Marker3D>("HeadDeathMarker");
		tex = camera.GetChild<TextureRect>(0);
		//playerWeaponController = GetNode<PlayerWeaponController>("MeshInstance3D/Head/Camera3D/PlayerWeaponController");
		fovReset = camera.Fov;
		headReset = head.Position.Y;
		Sprinting = false;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		//https://docs.godotengine.org/en/stable/tutorials/inputs/inputevent.html 
		if (@event is InputEventMouseMotion eventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			//rotates head and cam.
			camera.RotateX(-eventMouseMotion.Relative.Y * mouseSensitivity);
			head.RotateY(-eventMouseMotion.Relative.X * mouseSensitivity);
			//clamps rotation within allowed degrees.
			Godot.Vector3 cameraRotation = camera.Rotation;
			cameraRotation.X = Mathf.Clamp(cameraRotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80)); //has broken! 
			camera.Rotation = cameraRotation;
		}

		//Sprint and crouch control 
		if (IsOnFloor() || IsOnWall())
		{
			//NOTE: Input.IsActionPressed includes echoes, @event.IsActionPressed does not. 
			if (@event.IsActionPressed("player_sprint") && Stamina > 10f && Input.IsActionPressed("player_forward", false))
			{
				SprintSwitch();
			}
			if (@event.IsActionPressed("player_crouch"))
			{
				Crouching = false;
				CrouchSwitch();
				Crouching = true;
			}
			if (@event.IsActionReleased("player_crouch"))
			{
				Crouching = true;
				CrouchSwitch();
				Crouching = false;
			}
		}
		if (@event.IsActionReleased("player_sprint") && Sprinting == true) SprintSwitch();
	}



	public override void _PhysicsProcess(double delta)
	{

		if (health > 0)
		{
			Godot.Vector3 NewVelocity = Velocity;
			if (!IsOnFloor()) NewVelocity.Y -= gravity * (float)delta;
			if (Input.IsActionJustPressed("player_jump") && IsOnFloor()) NewVelocity.Y = JumpVelocity;
			Godot.Vector2 inputDir = Input.GetVector("player_left", "player_right", "player_forward", "player_back");
			Godot.Vector3 direction = (head.GlobalTransform.Basis * new Godot.Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Godot.Vector3.Zero)
			{
				NewVelocity.X = direction.X * Speed;
				NewVelocity.Z = direction.Z * Speed;
			}
			else
			{
				NewVelocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				NewVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}
			NewVelocity.X = Mathf.Lerp(Velocity.X, NewVelocity.X, Convert.ToSingle(delta) * 100);
			NewVelocity.Z = Mathf.Lerp(Velocity.Z, NewVelocity.Z, Convert.ToSingle(delta) * 100);

			Velocity = NewVelocity;
			MoveAndSlide();
		}
		else
		{
			mouseSensitivity = 0.0005f;
			head.Position = head.Position.Lerp(headDeathMarker.Position, 0.05f);

			//send signal back to enemy -- stop going after player! 

		}


	}

	public void SprintSwitch()
	{
		if (Sprinting == false)
		{
			Sprinting = true;
			Speed = 8f;
			var FovIncTween = CreateTween();
			FovIncTween.TweenProperty(camera, "fov", camera.Fov + 10f, 0.5f);
		}
		else if (Sprinting == true)
		{
			Sprinting = false;
			Speed = 5.0f;
			var FovDecTween = CreateTween();
			FovDecTween.TweenProperty(camera, "fov", fovReset, 0.2f);
		}
	}
	public void CrouchSwitch()
	{
		if (Crouching == false)
		{
			var CrouchDownTween = CreateTween();
			CrouchDownTween.TweenProperty(head, "position:y", head.Position.Y - 0.5f, 0.1f);
			crouchCollision.Disabled = false;
			standingCollision.Disabled = true;
		}
		else if (Crouching == true)
		{
			var CrouchUpTween = CreateTween();
			CrouchUpTween.TweenProperty(head, "position:y", headReset, 0.1f);
			crouchCollision.Disabled = true;
			standingCollision.Disabled = false;
		}
	}

	void _on_monster_anim_01_attack_player()
	{
		health -= 10;
		GD.Print("health = " + health);
	}

	async void _on_monster_anim_01_attack_player_damage_indicator()
	{
		tex.Visible = true;
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout"); //timers don't work properly with less than 1 second??? 
		tex.Visible = false;
	}
}
