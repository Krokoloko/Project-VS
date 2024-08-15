using Godot;
using System;

public class SampleAttackController : AttackController
{
    [Export]
    public Moveset moveset;
    [Export]
    public string name;
    [Export]
    public NodePath audio_player_node;
    private AudioStreamPlayer audio_player;

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
        if(name == "Lanky")
        {
            audio_player = GetNode<AudioStreamPlayer>(audio_player_node);   
            audio_player.Autoplay = false;
        }
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
        if(name == "Lanky")
        {
            if(audio_player.Playing)
            {
                if(audio_player.GetPlaybackPosition() + delta*3.0f >= audio_player.Stream.GetLength())
                {
                    audio_player.Stop();
                }
            }
        }
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
    public override void PreformAction(EACTION_TYPE action_type, float speed)
    {
        if(action == action_type) return;
        
        action = action_type;
        switch(action_type)
        {
            case EACTION_TYPE.JAB1:
                current_action_move = moveset.Jab1;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                if(name == "Lanky")
                {
                    audio_player.Play();
                }
                break;
            
            case EACTION_TYPE.JAB2:
        
                break;

            case EACTION_TYPE.JAB3:
        
                break;

            case EACTION_TYPE.RAPIDJAB:
        
                break;
            case EACTION_TYPE.DOWN_AERIAL:
                current_action_move = moveset.DownAerial;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.FORWARD_AERIAL:
                current_action_move = moveset.ForwardAerial;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.BACK_AERIAL:
                current_action_move = moveset.BackAerial;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.UP_AERIAL:
                current_action_move = moveset.UpAerial;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.FORWARD_TILT:
                current_action_move = moveset.ForwardTilt;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.UP_TILT:
                current_action_move = moveset.UpTilt;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.DOWN_TILT:
                current_action_move = moveset.DownTilt;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.DASHATTACK:
                current_action_move = moveset.DashAttack;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            case EACTION_TYPE.NEUTRAL_AERIAL:
                current_action_move = moveset.NeutralAerial;
                hitboxHandler.AddSequence(current_action_move, speed, player.GetPath());
                break;
            default:
                break;
        }
    }
}