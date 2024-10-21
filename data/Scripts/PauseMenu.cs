using Godot;
using System;
using System.Diagnostics;

public partial class PauseMenu : Control
{
	[Export] public CanvasLayer canvasLayer;
	bool pause = false;
	public override void _Ready()
	{
		canvasLayer.Hide(); //Just makes sure pause menu hidden if for whatever reason it was toggled on in editor
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_pause"))
		{
			if (canvasLayer.Visible)
			{
				canvasLayer.Hide(); Input.MouseMode = Input.MouseModeEnum.Captured;
			}
			else
			{
				canvasLayer.Show(); Input.MouseMode = Input.MouseModeEnum.Confined;
			}
		}
	}
	//Reuse following code structure for any other menus 
	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
	public void _on_continue_pressed()//identical snippet of code in SceneHandler opens the pause menu in the first place, dealing with 'esc' key presses. 
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		canvasLayer.Hide();
		pause = !pause;
	}
}
