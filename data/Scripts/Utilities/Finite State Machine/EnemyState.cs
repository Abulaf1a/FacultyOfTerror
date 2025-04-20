using Godot;
using System;

public partial class EnemyState : State{

    protected EnemySprite enemy;

    protected CharacterBody3D player; 

    public override void _Ready()
    {
        player = (CharacterBody3D) GetTree().GetNodesInGroup("Player")[0]; 
        
        enemy = GetOwner<EnemySprite>(); 

        GD.Print("owner of state is" + enemy.Name); 

        base._Ready();
    }

    //virtual is meant to be overridden. 
    public virtual void Enter(FiniteStateMachine finiteStateMachine, EnemySprite enemy)
    {
        this.enemy = enemy; 
        base.Enter(finiteStateMachine);
    }

    //but you can also override an override method https://stackoverflow.com/questions/1152925/override-an-overridden-method-c
    public override void Exit(string next)
    {
        GD.Print("enemy state exit method called"); 
        stateMachine.EmitSignal(FiniteStateMachine.SignalName.ChangeState, next);
        base.Exit(next);
    }

    

}