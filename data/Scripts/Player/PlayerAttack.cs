using Godot;
using System;
using System.Diagnostics;

public partial class PlayerAttack : Node3D
{

    [Export] private int damage = 10; 

    [Export] private float bulletSpeed = 10f; 

    private Timer timer; 

    public override void _Ready()
    {

        timer = GetNode<Timer>("Timer");


        timer.SetWaitTime(0.5f);
        timer.OneShot = true;
        // Called every time the node is added to the scene.
        // Initialization here.
    }

    public override void _PhysicsProcess(double delta)
    {
        // Called every frame. 'delta' is the elapsed time since the previous frame.
        // Update logic here.
    }

    public override void _UnhandledInput(InputEvent @event)
    {

        if(@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed())
        {
            if (eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                if(timer.IsStopped())
                {
                    PerformAttack();

                    timer.Start();
                }
                else
                {
                    GD.Print("Attack on cooldown!");
                }
            }
        }
    }

    public void PerformAttack()
    {
        // Logic for perfoming an attack based on player's current equipped weapon. 

        // Currently just fire projectile. 

        GD.Print("Performing attack with damage: " + damage);

        ProjectilePool projectilePool = ProjectilePool.GetInstance(); 

        CharacterBody3D player = (CharacterBody3D) FindParent("Player");//get player node

		projectilePool.FireProjectile(bulletSpeed, this, player.Velocity, GD.Load<PackedScene>("res://data/Assets/Sprites/PlayerProjectileSprite.tscn")); 

    }
}