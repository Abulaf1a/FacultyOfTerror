using System.ComponentModel.DataAnnotations.Schema;
using Godot;
using Godot.Collections;

public partial class Interactor : Area3D {

    /// <summary>
    /// keeps track of closes interactable to the player
    /// sends focus, unfocus and interact signals to applicable interactables.
    /// </summary>
    
    
    protected CharacterBody3D controller; 
    private Interactable closest; 
    public void Interact(Interactable interactable){
        interactable.EmitSignal(Interactable.SignalName.Interact, this);  
    }

    public void Focus(Interactable interactable){
        interactable.EmitSignal(Interactable.SignalName.Focused, this);  
    }

    public void Unfocus(Interactable interactable){
        interactable.EmitSignal(Interactable.SignalName.Unfocused, this);  
    }

    public Interactable GetClosest(){

        Array<Area3D> interactables = GetOverlappingAreas();
        float closeDist = float.PositiveInfinity; 
        if (interactables.Count > 0){
            foreach(Area3D area in interactables){
                if(area is Interactable){
                    if(area.GlobalPosition.DistanceTo(GlobalPosition) < closeDist){
                        closeDist = area.GlobalPosition.DistanceTo(GlobalPosition);
                        closest = area as Interactable;
                    }
                }
            }
            return closest;
        }

        return null; 
        
    }
}