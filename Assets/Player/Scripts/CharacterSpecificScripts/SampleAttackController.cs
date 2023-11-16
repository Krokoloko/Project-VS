using Godot;
using System;

public class SampleAttackController : AttackController
{
    [Export]
    public Moveset moveset;
    private HitboxSequence current_action_move;
    private int current_action_id;
    private EACTION_TYPE action;

    private HitboxHandler hitboxHandler;

    public override void _Ready()
    {
        player = GetNode<Player>(GetParent().GetPath());
        hitboxHandler = GetNode<HitboxHandler>("/root/HitboxHandler");
        action = EACTION_TYPE.NONE;
        current_action_move = null;
        current_action_id = -1;
    }

    public override void _Process(float delta)
    {
        ProcessAction(delta);
        if(action != EACTION_TYPE.NONE)
        {
            if(hitboxHandler.SequenceIsFinished(current_action_id))
            {
                hitboxHandler.DeleteSequence(current_action_id);
                current_action_id = -1;
                action = EACTION_TYPE.NONE;
            }
        }
    }

    private void ProcessAction(float delta)
    {
        switch(action)
        {
            case EACTION_TYPE.JAB1:

                break;
        }
    }

    public override void StopAction()
    {
        action = EACTION_TYPE.NONE;
        hitboxHandler.DeleteSequence(current_action_id);
        current_action_move = null;
    }
    public override void PreformAction(EACTION_TYPE action_type)
    {
        if(action == action_type) return;
        
        action = action_type;
        switch(action_type)
        {
            case EACTION_TYPE.JAB1:
                current_action_move = moveset.Jab1;
                hitboxHandler.AddSequence(current_action_move, player.GetPath());
                break;
            
            case EACTION_TYPE.JAB2:
        
                break;

            case EACTION_TYPE.JAB3:
        
                break;

            case EACTION_TYPE.RAPIDJAB:
        
                break;

            case EACTION_TYPE.FORWARD_TILT:
                
                break;
            default:
                break;
        }
    }
}