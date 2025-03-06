using Godot;

public partial class State : GodotObject{

    //attribute that holds current state. 
    public virtual void Enter(){ }
    public virtual void Execute() {}
    public virtual void Exit() {}
}