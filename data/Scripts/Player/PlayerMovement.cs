using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D, IDamageable
{
	[Export] private float WalkSpeed = 5.0f;
 	private float Speed;
	[Export] private float SprintSpeed = 8.0f;
	[Export] private float JumpVelocity = 4.5f;
	private bool Sprinting = false;
	private bool Crouching = false;
	private float Stamina = 100f;
	private int health = 100;
	private float mouseSensitivity = 0.01f;
	private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private string cachedLastCol;
	private string lastCol;
	private Node3D head;
	private Camera3D camera;
	private float fovReset;
	private float headReset;
	private CollisionShape3D standingCollision;
	private CollisionShape3D crouchCollision;
	private TextureRect damageEffect;
	private Marker3D headDeathMarker;
	private PlayerState playerState = PlayerState.ALIVE;
	
	public override void _Ready()
	{

		Speed = WalkSpeed; 
		Input.MouseMode = Input.MouseModeEnum.Captured;

		head = GetNode<CollisionShape3D>("Head");

		camera = head.GetNode<Camera3D>("Camera3D");
		//camera = head.GetChild<Camera3D>(0);
		damageEffect = camera.GetChild<TextureRect>(0);
		standingCollision = GetNode<CollisionShape3D>("StandingCollision");
		crouchCollision = GetNode<CollisionShape3D>("CrouchCollision");
		headDeathMarker = GetNode<Marker3D>("HeadDeathMarker");

		fovReset = camera.Fov;
		headReset = head.Position.Y;

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
			Godot.Vector3 velocity = Velocity;
			if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;
			if (Input.IsActionJustPressed("player_jump") && IsOnFloor()) velocity.Y = JumpVelocity;
			Godot.Vector2 inputDir = Input.GetVector("player_left", "player_right", "player_forward", "player_back");
			Godot.Vector3 direction = (head.GlobalTransform.Basis * new Godot.Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Godot.Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			velocity.X = Mathf.Lerp(Velocity.X, velocity.X, Convert.ToSingle(delta) * 100);
			velocity.Z = Mathf.Lerp(Velocity.Z, velocity.Z, Convert.ToSingle(delta) * 100);

			Velocity = velocity;
			MoveAndSlide();
		}
		else
		{
			//Player dies. 
			mouseSensitivity = 0.0005f;
			head.Position = head.Position.Lerp(headDeathMarker.Position, 0.05f);

		}

	}



	/// <summary>
	/// Sprinting and crouching
	/// </summary>
	public void SprintSwitch()
	{
		if (Sprinting == false)
		{
			Sprinting = true;
			Speed = SprintSpeed;
			var FovIncTween = CreateTween();
			FovIncTween.TweenProperty(camera, "fov", camera.Fov + 10f, 0.5f);
		}
		else if (Sprinting == true)
		{
			Sprinting = false;
			Speed = WalkSpeed;
			var FovDecTween = CreateTween();
			FovDecTween.TweenProperty(camera, "fov", fovReset, 0.2f);
		}
	}
	public void CrouchSwitch()
	{
		var CrouchTween = CreateTween();

		if (!Crouching)
		{
			CrouchTween.TweenProperty(head, "position:y", head.Position.Y - 0.5f, 0.1f);
			crouchCollision.Disabled = false;
			standingCollision.Disabled = true;
		}
		else
		{
			CrouchTween.TweenProperty(head, "position:y", headReset, 0.1f);
			crouchCollision.Disabled = true;
			standingCollision.Disabled = false;
		}
	}


	
	/// Damageable interface methods
	/// 
	public void TakeDamage(int damage){
		health -= damage; 
		// GD.Print("player damaged: health = " + health);
	}

	public void Heal(int heal)
	{
		health += heal;
		// GD.Print("health = " + health);
	}
	public int GetHealth()
	{
		return health;
	}
	public int GetMaxHealth()
	{
		return 100;
	}

	public bool IsDead()
	{
		if (health <= 0) return true;
		else return false;
	}
	public void Die()
	{
		health = 0;
		// GD.Print("Player is dead");
		playerState = PlayerState.DEAD;
	}
	



	/// <summary>
	/// Signals: 
	/// </summary>

	void _on_monster_anim_01_attack_player()
	{
		health -= 10;
		// GD.Print("health = " + health);
	}

	async void _on_monster_anim_01_attack_player_damage_indicator()
	{
		damageEffect.Visible = true;
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout"); //timers don't work properly with less than 1 second??? 
		damageEffect.Visible = false;
	}
}
