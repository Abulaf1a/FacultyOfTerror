using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;


public partial class ActorControl : Node
{
    public List<BaseActor> actors;

    static ActorControl actorControl;

    public static ActorControl GetInstance()
    {
        if (actorControl == null)
        {
            actorControl = new ActorControl();
        }
        return actorControl;
    }

    private ActorControl()
    {
        actors = new List<BaseActor>();
    }

    public void RegisterActor(BaseActor actor)
    {
        GetInstance().actors.Add(actor);

        PrintList();
    }

    public void PrintList()
    {

        Debug.WriteLine("New actor added to list, list is now : ");
        foreach (BaseActor actor in actors)
        {
            GD.Print(actor.Name);
        }
    }












}

