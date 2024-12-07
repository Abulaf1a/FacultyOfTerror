using Godot;
using System;

public partial class ActorMarker : Node3D
{

	[Export] CharacterBody3D targetActor;

	CollisionObject3D collision;

	public void _Ready()
	{
		collision = GetNode<CollisionObject3D>("Area3D/CollisionShape3D");
	}

}
