
using System.Reflection.Metadata;
using Godot;

public partial class PlayerInteractor : Interactor {

    Interactable cached; 

    [Export] CharacterBody3D player; 

    public override void _Ready() {

        controller = player;
        cached = GetClosest();
       // base._Ready();
    }

    public override void _PhysicsProcess(double delta){

        Interactable newClosest = GetClosest(); 

        if(newClosest != null && newClosest != cached){
            Focus(newClosest); 
            if(cached != null) Unfocus(cached);
            cached = newClosest;
        }
        if(newClosest == null && cached != null){
            Unfocus(cached);
            cached = null; 
        }



       // base._PhysicsProcess(delta); 


    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("player_interact") && cached != null){
            Interact(cached);
        }
       // base._Input(@event);
    }
}