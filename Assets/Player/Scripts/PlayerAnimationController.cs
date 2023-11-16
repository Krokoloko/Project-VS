using Godot;
using System;

public class PlayerAnimationController : Sprite3D
{
    public enum AnimationState 
    {
        NONE,
        JUMP,
        AERIAL_TO_JUMP,
        CROUCHATTACK_TO_CROUCH,
        IDLE,
        WALKING,
        CROUCHING,
        SHIELD,
        JAB1,
        JAB2,
        JAB3,
        RAPID_JAB,
        DASH_ATTACK,
        FORWARD_TILT,
        UP_TILT,
        DOWN_TILT,
        AERIAL_NEUTRAL,
        AERIAL_FORWARD,
        AERIAL_BEHIND,
        AERIAL_UP,
        AERIAL_DOWN,
        NEUTRAL_SPECIAL,
        FORWARD_SPECIAL,
        UP_SPECIAL,
        DOWN_SPECIAL,
        TAUNT1,
        TAUNT2,
        TAUNT3
    }

    public struct AnimationProgress
    {
        public float position;
        public float animation_length;
    }

    [Export(PropertyHint.Range, "0,1000")]
    public int default_frame;

    [Export]
    public int FPS;
    private float FPS_in_ms;

    public int current_frame {get{return p_current_frame;}}
    private int p_current_frame;

    private int frames_animation;

    private AnimationPlayer animation_player;
    private bool flip;
    private AnimationState state;
    public override void _Ready()
    {
        flip = false;
        animation_player = GetNode<AnimationPlayer>(this.GetPath() + "/AnimationPlayer");
        Frame = default_frame;
        state = AnimationState.NONE;
        p_current_frame = 0;
        frames_animation = 0;
        FPS_in_ms = 1000.0f/FPS;
    }

    public AnimationState GetState()
    {
        return state;
    }

    public void SetAnimationSpeed(float speed)
    {
        animation_player.PlaybackSpeed = speed;
    }
    public virtual void SetAnimation(AnimationState new_state)
    {
        if(state != new_state)
        {
            state = new_state;
            switch(state)
            {
                case AnimationState.IDLE:       
                    animation_player.Play("Idle");
                    break;
                case AnimationState.JUMP:
                    animation_player.Play("Jump");
                    break;

                case AnimationState.AERIAL_NEUTRAL:
                    animation_player.Play("NeutralAerial1");
                    break;

                case AnimationState.AERIAL_FORWARD:
                    animation_player.Play("ForwardAerial1");
                    break;

                case AnimationState.AERIAL_BEHIND:
                    animation_player.Play("BehindAerial1");
                    break;

                case AnimationState.AERIAL_UP:
                    animation_player.Play("UpAerial1");
                    break;

                case AnimationState.AERIAL_DOWN:
                    animation_player.Play("DownAerial1");
                    break;
                
                case AnimationState.WALKING:
                    animation_player.Play("Walk");
                    break;

                case AnimationState.JAB1:
                    animation_player.Play("Jab1");
                    break;

                case AnimationState.JAB2:
                    animation_player.Play("Jab2");
                    break;

                case AnimationState.JAB3:
                    animation_player.Play("Jab3");
                    break;

                case AnimationState.CROUCHING:
                    animation_player.Play("Crouch");
                    break;

                case AnimationState.FORWARD_TILT:
                    animation_player.Play("ForwardTilt1");
                    break;

                case AnimationState.DOWN_TILT:
                    animation_player.Play("DownTilt1");
                    break;

                case AnimationState.UP_TILT:
                    animation_player.Play("UpTilt1");
                    break;

                case AnimationState.DASH_ATTACK:
                    animation_player.Play("DashAttack1");
                    break;

                case AnimationState.TAUNT1:
                    animation_player.Play("Taunt_1");
                    break;

            }
        }
    }

    public AnimationProgress GetAnimationProgress()
    {
        AnimationProgress progress;
        progress.position = animation_player.CurrentAnimationPosition;
        progress.animation_length = animation_player.CurrentAnimationLength;
        return progress;        
    }

    public void FlipSprite(bool face_left)
    {
        flip = face_left;
        Scale = new Vector3((flip)?-1.0f:1.0f, 1.0f, 1.0f);
    }

    public bool GetFlip()
    {
        return flip;
    }

    public override void _Process(float delta)
    {
        p_current_frame = (int)((animation_player.CurrentAnimationLength/animation_player.CurrentAnimationPosition)/FPS_in_ms);
        switch(state)
        {
            case AnimationState.JAB1:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.JAB2:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.JAB3:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.AERIAL_NEUTRAL:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.AERIAL_TO_JUMP;
                }
                break;

            case AnimationState.AERIAL_FORWARD:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.AERIAL_TO_JUMP;
                }
                break;

            case AnimationState.AERIAL_BEHIND:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.AERIAL_TO_JUMP;
                }
                break;

            case AnimationState.AERIAL_UP:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.AERIAL_TO_JUMP;
                }
                break;

            case AnimationState.AERIAL_DOWN:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.AERIAL_TO_JUMP;
                }
                break;

            case AnimationState.DOWN_TILT:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.CROUCHATTACK_TO_CROUCH;
                }
                break;

            case AnimationState.FORWARD_TILT:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.UP_TILT:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.DASH_ATTACK:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;

            case AnimationState.TAUNT1:
                if(!animation_player.IsPlaying())
                {
                    state = AnimationState.NONE;
                }
                break;
        }
    }
}
