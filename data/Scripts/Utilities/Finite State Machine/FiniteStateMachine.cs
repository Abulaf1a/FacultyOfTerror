using Godot;
using System;
using System.Collections.Generic;

public partial class FiniteStateMachine : Node{


    [Signal]
    public delegate void ChangeStateEventHandler(String next); 

    private State currentState; 
    private Dictionary<String, State> states = new Dictionary<string, State>(); 

    [Export] State FirstState; 


    public override void _Ready(){

        Godot.Collections.Array<Node> children = GetChildren(); 

        //loop over children to add to the state machine list 
        foreach(Node child in children)
        {
            if(child is State state) //syntax here is called "pattern matching", checks and casts at the same time.
            {
                states.Add(state.Name, state); //casts to state - but to specific state? 
            }
        }

        currentState = FirstState; 

        currentState.Enter(this); 
    }

    public void _on_change_state(String next){

        GD.Print("state changing!"); 

        State nextState = states[next]; 

        currentState = nextState; 

        GD.Print("state changed, current state now: " + currentState.Name); 

        currentState.Enter(this); 

    }

    public override void _Process(double delta)
    {
        if(currentState != null)
        {
            // currentState._Process(delta); //idiot!!

        }
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        currentState?.Update(delta); 
         //null 'propagation' I guess like a tenary statement. 
    }

} 