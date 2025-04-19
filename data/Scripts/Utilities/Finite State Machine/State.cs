using System;
using Godot;

public partial class State : Node{

    protected FiniteStateMachine stateMachine; 

    //attribute that holds current state. 
    public virtual void Enter(FiniteStateMachine stateMachine) {this.stateMachine = stateMachine; }
    public virtual void Update(double delta) {}
    public virtual void Exit(String next) {}

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }
}