using Godot;
using System;

public partial class ActorMarker : Node3D
{

	// [Signal]
	// public delegate void ActorMarkerActorEnteredEventHandler();
	[Export] CharacterBody3D targetActor;

	CollisionObject3D collision;

	public void _Ready()
	{
		collision = GetNode<CollisionObject3D>("Area3D/CollisionShape3D");
	}

	// public void _on_area_3d_body_entered(CollisionObject3D collider)
	// {
	// 	GD.Print("something entered my collision area! " + this.Name);
	// 	if (collider.GetParent().Name == targetActor.Name)
	// 	{
	// 		EmitSignal(SignalName.ActorMarkerActorEntered);
	// 	}
	// }
}
