using Godot;
using System;


public abstract partial class BaseActor : CharacterBody3D
{

    //in abstract base classes, you can find methods in the abstract class as well. 
    [Export] public float Speed;

    [Export] public Node3D target;

    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public RayCast3D? ray;
    public Node3D? mesh;
    public Godot.Vector3 targetPos;
    public float distToTarget;

    public NavigationAgent3D nav;



}


