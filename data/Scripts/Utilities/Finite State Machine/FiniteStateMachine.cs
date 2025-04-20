using Godot;
using System;
using System.Collections.Generic;

public partial class FiniteStateMachine : Node{


    /// <summary>
    /// Finite State Machine for Enemies.
    /// Separate state for movement and attacking 
    /// 
    /// Both states run from FSM _PhysicsProcess()
    /// 
    /// However, movement state starts and stops attack state
    /// (e.g. attack state determines movement state) 
    /// </summary>

    [Signal]
    public delegate void ChangeStateEventHandler(String next); 

    private State currentMoveState; 
    private State currentAttackState; 
    private Dictionary<String, State> states = new Dictionary<string, State>(); 

    [Export] State FirstState; 


    public override void _Ready(){

        Godot.Collections.Array<Node> children = GetChildren(); 

        //loop over children to add to the state machine list 
        foreach(Node child in children)
        {
            if(child is State state) //syntax here is called "pattern matching", 
            // checks and casts at the same time.
            {
                states.Add(state.Name, state); //casts to state - but to specific state? 
            }
        }

        currentMoveState = FirstState; 

        currentMoveState.Enter(this); 

    }

    public void _on_change_state(String next){

        GD.Print("state changing!"); 

        State nextState = states[next]; 

        currentMoveState = nextState; 

        GD.Print("state changed, current state now: " + currentMoveState.Name); 

        currentMoveState.Enter(this); 

    }

    public override void _PhysicsProcess(double delta)
    {
        currentMoveState?.Update(delta); 

        currentAttackState?.Update(delta);
    }

    public void SetAttackState(State state){

        if(state!= null){
            currentAttackState = state; 
            currentAttackState.Enter(this); 
        }
        else{
            currentAttackState = null; 
        }
   

    }

} 