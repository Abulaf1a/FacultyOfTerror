using Godot;
using System;

public partial class Candle : Node3D
{

    private OmniLight3D effect; 

    private MeshInstance3D material;

    [Export] private Material highlight;

    [Export] private Material normal; 

    public override void _Ready(){
        effect = GetNode<OmniLight3D>("Light");
        material = GetNode<MeshInstance3D>("Wax");

    }
    public void _on_interactable_focused(Area3D interactor){
        material.MaterialOverlay = highlight;

    }

    public void _on_interactable_unfocused(Area3D interactor){
        material.MaterialOverlay = normal;

    }

    public void _on_interactable_interact(Area3D interactor){
        effect.Visible = !effect.Visible;

    }
}
