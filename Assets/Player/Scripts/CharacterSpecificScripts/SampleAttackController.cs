using Godot;
using System;

public class SampleAttackController : AttackController
{
    

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        player = GetNode<Player>(GetParent().GetPath());    
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        
    }

    public override void PreformAction(EACTION_TYPE action_type)
    {
        switch(action_type)
        {
            case EACTION_TYPE.FORWARD_TILT:
                
                break;
            default:
                break;
        }
    }
}