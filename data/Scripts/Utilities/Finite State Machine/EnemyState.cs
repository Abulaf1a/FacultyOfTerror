using Godot;
using System;

public partial class EnemyState : State{

    protected EnemySprite enemy;

    public override void _Ready()
    {
        //set Enemy to parent.
        enemy = GetOwner<EnemySprite>(); 

        GD.Print("owner of state is" + enemy.Name); 

        base._Ready();
    }

    

}