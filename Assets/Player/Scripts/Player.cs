using Godot;
using System;
using System.Drawing.Text;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;


public class Player : KinematicBody
{
	public enum PlayerState
	{
		IDLE,
		WALKING,
		WALKING_TO_RUN,
		RUN,
		DASH_ATTACK,
		TILT_ATTACK_FORWARD,
		TILT_ATTACK_UP,
		TILT_ATTACK_DOWN,
		JAB_ATTACK1,
		JAB_ATTACK2,
		JAB_ATTACK3,
		JAB_RAPID,
		JUMP,
		AIRBORNE,
		AERIAL_ATTACK_FORWARD,
		AERIAL_ATTACK_NEUTRAL,
		AERIAL_ATTACK_BEHIND,
		AERIAL_ATTACK_UP,
		AERIAL_ATTACK_DOWN,
		SHIELDING,
		CROUCHING,
		TAUNT1,
		TAUNT2,
		TAUNT3
	}
	[Export(PropertyHint.Range,"0,100")]
	public float MOVE_SPEED = 10.0f;
	private const float FPS = 1.0f / 60.0f;
	//5 Frame window
	private const float DASH_INPUT_WINDOW = 1.0f / 60.0f * 5.0f;
	[Export(PropertyHint.Range,"0,200")]
	public float RUNSPEED = 25.0f; 

	private Vector3 motion;
	[Export(PropertyHint.Range, "1,3")]
	public int total_jabs;
	public bool has_rapid_jab;

	[Export(PropertyHint.Range,"0.01, 50")]
	public float JUMP_HEIGHT = 5.0f;
	
	[Export(PropertyHint.Range, "0.01, 100")]
	public float AIR_ACCELERATION = 10.0f;

	[Export(PropertyHint.Range, "0.01,100")]
	public float AIR_SPEED = 30.0f;
	public const float GRAVITY = 1.0f;
	
	[Export(PropertyHint.Range,"0.01, 50")]
	public float NEUTRAL_FALL_SPEED = 1.0f;
	
	[Export(PropertyHint.Range,"0.01, 50")]
	public float FAST_FALL_SPEED = 2.0f;
	
	[Export(PropertyHint.Range,"0.01, 50")]
	public float WEIGHT = 1.0f;

	private float weight_divisor;
	private float facing_direction; 
	private float falling_speed;
	private float treshold;
	private float track_time;
	private float accumalator;
	private float air_drift;

	private PlayerState state;
	private PlayerAnimationController anim_controller;


	public override void _Ready()
	{
		air_drift = 0.0f;
		weight_divisor = 1/WEIGHT;
		facing_direction = 1.0f;
		anim_controller = GetNode<PlayerAnimationController>(this.GetPath() + "/Sprite3D");
		state = PlayerState.IDLE;
		accumalator = 0.0f;
		track_time = 0.0f;
		treshold = 0.0f;
	}

	public override void _Process(float delta)
	{
		HandleInput();
		HandleAction(delta);
	}

	public override void _PhysicsProcess(float deltaTime)
	{
		MoveAndCollide(motion*deltaTime);
		MoveAndSlide(Vector3.Zero, Vector3.Up);
	}

	public void HandleInput()
	{
		bool right_pressed = Input.IsActionJustPressed("Player_Right");
		bool right_hold = Input.IsActionPressed("Player_Right");
		bool right_released = Input.IsActionJustReleased("Player_Right");

		bool left_pressed = Input.IsActionJustPressed("Player_Left");
		bool left_hold = Input.IsActionPressed("Player_Left");
		bool left_released = Input.IsActionJustReleased("Player_Left");
		//Move Left or right
		if(left_hold && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
		}
		if(right_hold && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
		}
		if(Input.IsActionPressed("Player_Down"))
		{
			state = PlayerState.CROUCHING;
		}
		if(Input.IsActionJustReleased("Player_Down") && state == PlayerState.CROUCHING)
		{
			state = PlayerState.IDLE;
		}
		bool attack_preformed = false;
		if(Input.IsActionJustPressed("Player_Jump"))
		{
			state = PlayerState.JUMP;
		}
		if(Input.IsActionJustPressed("Player_Attack"))
		{
			//Handle Jab attacks
			if(!attack_preformed && Input.IsActionPressed("Player_Up"))
			{
				state = PlayerState.TILT_ATTACK_UP;
				attack_preformed = true;
			}
			if(state == PlayerState.IDLE)
			{
				state = PlayerState.JAB_ATTACK1;
				treshold = 1.0f;
				attack_preformed = true;
			}
			else if(state == PlayerState.JAB_ATTACK1 && total_jabs > 1)
			{
				state = PlayerState.JAB_ATTACK2;
			}
			else if(state == PlayerState.JAB_ATTACK2 && total_jabs > 2)
			{
				state = PlayerState.JAB_ATTACK3;
			}

			if(!attack_preformed && state == PlayerState.WALKING)
			{
				state = PlayerState.TILT_ATTACK_FORWARD;
				attack_preformed = true;
			}
			if(!attack_preformed && state == PlayerState.RUN)
			{
				state = PlayerState.DASH_ATTACK;
				anim_controller.SetAnimationSpeed(1.0f);
				attack_preformed = true;
			}
			if(!attack_preformed && state == PlayerState.CROUCHING)
			{
				state = PlayerState.TILT_ATTACK_DOWN;
				attack_preformed = true;
			}
		}

		//Stop walking
		if(left_released && !right_hold && state == PlayerState.WALKING)
		{
			state = PlayerState.WALKING_TO_RUN;
			treshold = DASH_INPUT_WINDOW;
			track_time = 1;
		}
		if(right_released && !left_hold && state == PlayerState.WALKING)
		{
			state = PlayerState.WALKING_TO_RUN;
			treshold = DASH_INPUT_WINDOW;
			track_time = 1;
		}

		//Stop running
		if(left_released && !right_hold && state == PlayerState.RUN)
		{
			state = PlayerState.IDLE;
			ResetParameters();
		}
		if(right_released && !left_hold && state == PlayerState.RUN)
		{
			state = PlayerState.IDLE;
			ResetParameters();
		}

		//Turn around while running
		if(right_pressed && (state == PlayerState.WALKING_TO_RUN || state == PlayerState.RUN))
		{
			state = PlayerState.RUN;
			anim_controller.SetAnimationSpeed(2.0f);
		}
		if(left_pressed && (state == PlayerState.WALKING_TO_RUN || state == PlayerState.RUN))
		{
			state = PlayerState.RUN;
			anim_controller.SetAnimationSpeed(2.0f);
		}

		if(Input.IsActionJustPressed("TAUNT_1") && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN))
		{
			state = PlayerState.TAUNT1;
		}

		// if(Input.IsActionJustPressed("TAUNT_2") && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN))
		// {
		//     state = PlayerState.TAUNT2;
		// }

		//  if(Input.IsActionJustPressed("TAUNT_3") && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN))
		// {
		//     state = PlayerState.TAUNT3;
		// }

		if(state == PlayerState.WALKING || state == PlayerState.RUN)
		{			
			if(left_pressed)
			{
				anim_controller.FlipSprite(true);
				facing_direction = -1.0f;
			}
			if(right_pressed)
			{
				anim_controller.FlipSprite(false);
				facing_direction = 1.0f;
			}
			if(left_released)
			{
				if(right_hold)
				{
					anim_controller.FlipSprite(false);
					facing_direction = 1.0f;
				}
			}
			if(right_released)
			{
				if(left_hold)
				{
					anim_controller.FlipSprite(true);
					facing_direction = -1.0f;
				}
			}
		}   
		if(state == PlayerState.AIRBORNE)
			{
				if(left_hold)
				{
					air_drift = Mathf.Max(air_drift - AIR_ACCELERATION, -AIR_SPEED);
					motion = new Vector3(air_drift, motion.y, 0);
				}
				else if(right_hold)
				{
					air_drift = Mathf.Min(air_drift + AIR_ACCELERATION, AIR_SPEED);
					motion = new Vector3(air_drift, motion.y, 0);
				}else
				{
					float traction = AIR_ACCELERATION*0.5f;
					air_drift -= traction*Mathf.Sign(air_drift);
					motion = new Vector3(air_drift, motion.y, 0);
				}
			}
	}

	public void HandleAction(float deltaTime)
	{
		accumalator += track_time * deltaTime;
		if(anim_controller.GetState() == PlayerAnimationController.AnimationState.NONE)
		{
			state = PlayerState.IDLE;
			anim_controller.Rotation = new Vector3(0,0,0);
		}
		PlayerAnimationController.AnimationProgress animation_progress;
		switch(state)
		{
			case PlayerState.IDLE:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.IDLE);
				motion = Vector3.Zero;
				break;
			
			case PlayerState.WALKING:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.WALKING);
				motion = new Vector3(MOVE_SPEED * deltaTime * facing_direction,motion.y,0);
				break;
			case PlayerState.CROUCHING:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.CROUCHING);
				break;
			case PlayerState.JAB_ATTACK1:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB1);
				break;
			
			case PlayerState.JAB_ATTACK2:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB2);
				break;

			case PlayerState.JAB_ATTACK3:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB3);
				break;

			case PlayerState.JAB_RAPID:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.RAPID_JAB);
				break;
			
			case PlayerState.TILT_ATTACK_DOWN:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.DOWN_TILT);
				break;
			
			case PlayerState.TILT_ATTACK_FORWARD:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.FORWARD_TILT);
				break;
			
			case PlayerState.TILT_ATTACK_UP:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.UP_TILT);
				break;

			case PlayerState.DASH_ATTACK:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.DASH_ATTACK);
				animation_progress = anim_controller.GetAnimationProgress();
				anim_controller.Rotation = new Vector3(0,0,0*(1 - animation_progress.position/animation_progress.animation_length) + facing_direction*-1*2.0f*Mathf.Pi * animation_progress.position/animation_progress.animation_length);
				motion = new Vector3(facing_direction * RUNSPEED,motion.y,0);
				break;

			case PlayerState.WALKING_TO_RUN:
				if(accumalator >= treshold)
				{
					state = PlayerState.IDLE;
					ResetParameters();
				}
				break;
			case PlayerState.JUMP:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
				motion += Vector3.Up * JUMP_HEIGHT;
				falling_speed = NEUTRAL_FALL_SPEED;
				break;

			case PlayerState.AIRBORNE:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
				motion -= Vector3.Up * GRAVITY * falling_speed;
				break;

			case PlayerState.RUN:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.WALKING);
				motion = new Vector3(deltaTime * facing_direction * RUNSPEED,motion.y,0);
				break;

			case PlayerState.TAUNT1:
				anim_controller.SetAnimation(PlayerAnimationController.AnimationState.TAUNT1);
				break;
		}
		if(state == PlayerState.AIRBORNE && IsOnFloor())
		{
			state = PlayerState.IDLE;
			ResetParameters();
		}
		if(state == PlayerState.JUMP)
		{
			state = PlayerState.AIRBORNE;
		}
	}

	private void ResetParameters()
	{
			motion = Vector3.Zero;
			treshold = 0f;
			track_time = 0f;
			accumalator = 0f;            
			air_drift = 0f;
			anim_controller.SetAnimationSpeed(1.0f);
	}
}
