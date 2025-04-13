using Godot;
using System;


public abstract partial class BaseActor : CharacterBody3D
{
    [Export] protected float Speed;

    [Export] protected Node3D target;

    protected float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    protected RayCast3D? ray;
    protected Node3D? mesh;
    protected Godot.Vector3 targetPos;
    protected float distToTarget;
    protected NavigationAgent3D nav;

    public override void _Ready()
    {
        ActorControl.GetInstance().RegisterActor(this);
    }
}


