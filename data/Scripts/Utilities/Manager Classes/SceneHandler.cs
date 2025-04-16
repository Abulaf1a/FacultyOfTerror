using Godot;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;


public partial class SceneHandler : Node3D
{
	// Called when the node enters the scene tree for the first time.

	/// <summary>
	/// Provides a static instance of itself, utility class for accessing the root node of the scene tree.
	/// Doesn't work though because it isn't in the scene tree itself! 
	/// To change or get rid of!
	/// </summary>
	private Node3D root; 

	private static SceneHandler sceneHandler; 

	public static SceneHandler GetSceneHandler(){
		if(sceneHandler == null){
			return new SceneHandler();
		}
		return sceneHandler; 
		 
	}

	public override void _Ready()
	{

		GD.Print("scene handler ready");
		sceneHandler = this;
		root = this; 
	}

	public Node3D GetRoot(){
		return root; 
	}

	public SceneTree GetSceneTree(){
		return GetTree(); 
	}

	public override void _Process(double delta)
	{
	}
}
