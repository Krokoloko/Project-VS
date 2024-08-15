using Godot;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Text;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;



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

	[Export]
	public CharacterResources resource_reference;

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
	public Area area;
	public PlayerAnimationController anim_controller{get {return p_anim_controller;}}
	private PlayerAnimationController p_anim_controller;
	private AttackController attack_controller;

	private bool floored;
	private bool has_double_jump;
	private PhysicsBody detected_go_through_platform = null;
	public int player_number;
	private uint platform_bittracker = 0;
	private bool go_through_platform = false;
	private bool print_once = true;
	private const float double_down_platform_timer_treshold = 0.3f;
	private bool double_down_platform_timer_on = false;
	private float double_down_platform_timer = 0.0f;

	public override void _Ready()
	{
		for(int i = 16; i < 32; i++)
		{
			platform_bittracker |= (uint)(1<<i);
		}
		player_number = 1;
		floored = true;
		has_double_jump = false;
		air_drift = 0.0f;
		weight_divisor = 1/WEIGHT;
		facing_direction = 1.0f;
		p_anim_controller = GetNode<PlayerAnimationController>(this.GetPath() + "/Sprite3D");
		p_anim_controller.FlipSprite(true);
		attack_controller = GetNode<AttackController>(this.GetPath() + "/AttackController");
		Sprite3D sprite = GetNode<Sprite3D>(this.GetPath() + "/Sprite3D");
		state = PlayerState.IDLE;
		area = GetNode<Area>(this.GetPath() + "/Area");
		area.CollisionLayer = platform_bittracker;
		area.CollisionMask = platform_bittracker;
		area.Connect("body_entered", this, "AreaDetected");
		area.Connect("body_exited", this, "AreaExited");
		platform_bittracker |= 1;
		accumalator = 0.0f;
		track_time = 0.0f;
		treshold = 0.0f;
	}

	private void AreaDetected(Node body)
	{
		if(IsStateAerial())
		{
			PhysicsBody p_body = (PhysicsBody)(body);
			if(p_body.Name != "Player")
			{
				detected_go_through_platform = p_body;
				platform_bittracker |= p_body.CollisionLayer;
				CollisionLayer = platform_bittracker;
				CollisionMask = platform_bittracker;
			}
		}
	}

	private void AreaExited(Node body)
	{
		PhysicsBody p_body = (PhysicsBody)(body);
		if(p_body == detected_go_through_platform)
		{
			detected_go_through_platform = null;
		}
	}

	public void SetPassThroughPlatformBits()
	{
		for(int i = 15; i < 32; i++)
		{
			SetCollisionLayerBit(i, ((1<<i) & platform_bittracker) > 0);
			SetCollisionMaskBit(i, ((1<<i) & platform_bittracker) > 0);
		}
	}

	public override void _Process(float delta)
	{
		HandleInput();
		HandleAction(delta);
	}

	public override void _PhysicsProcess(float deltaTime)
	{
		MoveAndSlide(motion, Vector3.Up, true);
		floored = IsOnFloor();
	}

	public void HandleInput()
	{
		bool right_pressed = Input.IsActionJustPressed("Player_Right");
		bool right_hold = Input.IsActionPressed("Player_Right");
		bool right_released = Input.IsActionJustReleased("Player_Right");

		bool left_pressed = Input.IsActionJustPressed("Player_Left");
		bool left_hold = Input.IsActionPressed("Player_Left");
		bool left_released = Input.IsActionJustReleased("Player_Left");

		bool down_hold = Input.IsActionPressed("Player_Down");
		bool down_released = Input.IsActionJustReleased("Player_Down");

		bool jump_pressed = Input.IsActionJustPressed("Player_Jump");

		bool up_pressed = Input.IsActionJustPressed("Player_Up");
		bool up_hold = Input.IsActionPressed("Player_Up");

		//Move Left or right
		if(left_hold && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
		}
		if(right_hold && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
		}
		if(down_hold && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.CROUCHING))
		{
			state = PlayerState.CROUCHING;
		}
		if(!down_hold && state == PlayerState.CROUCHING)
		{
			state = PlayerState.IDLE;
		}

		if(down_released)
		{
			if(double_down_platform_timer_on)
			{
				FallThroughPlatform();
				double_down_platform_timer_on = false;
				double_down_platform_timer = 0;
			}else
			{
				double_down_platform_timer_on = true;
				double_down_platform_timer = 0;
			}
		}

		if(jump_pressed && !IsStateAerial())
		{
			if(state == PlayerState.CROUCHING)
			{
				FallThroughPlatform();
			}
			else
			{
				state = PlayerState.JUMP;
			}
			has_double_jump = true;
		}
		else if(state == PlayerState.AIRBORNE && has_double_jump && jump_pressed)
		{
			state = PlayerState.JUMP;	
			if(detected_go_through_platform != null)
			{	
				uint keep_bits = 0;
				for(int i = 0; i < 15; i++)
				{
					keep_bits |= (uint)(1 << i) & CollisionMask;
				}
				platform_bittracker &= keep_bits;
				platform_bittracker |= ~detected_go_through_platform.CollisionLayer;
			}
			has_double_jump = false;
			motion = new Vector3(motion.x,0,0);
		}

		bool attack_preformed = false;

		if(Input.IsActionJustPressed("Player_Attack"))
		{
			if(state == PlayerState.AIRBORNE)
			{
				state = PlayerState.AERIAL_ATTACK_NEUTRAL;
				if(left_hold)
				{
					bool direction = (facing_direction > 0);
					if(direction)
					{
						state = PlayerState.AERIAL_ATTACK_BEHIND;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_BEHIND]).Step;
						attack_controller.PreformAction(EACTION_TYPE.BACK_AERIAL, step);
					}else
					{
						state = PlayerState.AERIAL_ATTACK_FORWARD;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_FORWARD]).Step;
						attack_controller.PreformAction(EACTION_TYPE.FORWARD_AERIAL, step);
					}
				}
				else if(right_hold)
				{
					bool direction = (facing_direction > 0);
					if(direction)
					{
						state = PlayerState.AERIAL_ATTACK_FORWARD;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_FORWARD]).Step;
						attack_controller.PreformAction(EACTION_TYPE.FORWARD_AERIAL, step);
					}else
					{
						state = PlayerState.AERIAL_ATTACK_BEHIND;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_BEHIND]).Step;
						attack_controller.PreformAction(EACTION_TYPE.BACK_AERIAL, step);
					}
				}
				else if(up_hold)
				{
					state = PlayerState.AERIAL_ATTACK_UP;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_UP]).Step;
					attack_controller.PreformAction(EACTION_TYPE.UP_AERIAL, step);
				}
				else if(down_hold)
				{
					state = PlayerState.AERIAL_ATTACK_DOWN;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_DOWN]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DOWN_AERIAL, step);
				}
				if(state == PlayerState.AERIAL_ATTACK_NEUTRAL)
				{
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_NEUTRAL]).Step;
					attack_controller.PreformAction(EACTION_TYPE.NEUTRAL_AERIAL, step);
				}
			}
			
			if(IsStateGrounded())
			{
				if(up_hold)
				{
					state = PlayerState.TILT_ATTACK_UP;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.UP_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.UP_TILT, step);
					attack_preformed = true;
				}
				if(!attack_preformed && state == PlayerState.WALKING)
				{
					state = PlayerState.TILT_ATTACK_FORWARD;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.FORWARD_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.FORWARD_TILT, step);
					attack_preformed = true;
					motion = Vector3.Zero;
				}
				if(!attack_preformed && state == PlayerState.RUN)
				{
					state = PlayerState.DASH_ATTACK;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.DASH_ATTACK]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DASHATTACK, step);
					p_anim_controller.SetAnimationSpeed(1.0f);
					attack_preformed = true;
				}
				if(!attack_preformed && state == PlayerState.CROUCHING)
				{
					state = PlayerState.TILT_ATTACK_DOWN;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.DOWN_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DOWN_TILT, step);
					attack_preformed = true;
				}
				//Handle Jab attacks
				if(!attack_preformed && state == PlayerState.IDLE)
				{
					state = PlayerState.JAB_ATTACK1;
					treshold = 1.0f;
					attack_preformed = true;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.JAB1]).Step;
					attack_controller.PreformAction(EACTION_TYPE.JAB1, step);
				}
				else if(state == PlayerState.JAB_ATTACK1 && total_jabs > 1)
				{
					state = PlayerState.JAB_ATTACK2;
				}
				else if(state == PlayerState.JAB_ATTACK2 && total_jabs > 2)
				{
					state = PlayerState.JAB_ATTACK3;
				}
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
			p_anim_controller.SetAnimationSpeed(2.0f);
		}
		if(left_pressed && (state == PlayerState.WALKING_TO_RUN || state == PlayerState.RUN))
		{
			state = PlayerState.RUN;
			p_anim_controller.SetAnimationSpeed(2.0f);
		}

		if(Input.IsActionJustPressed("TAUNT_1") && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN))
		{
			state = PlayerState.TAUNT1;
		}

		if(state == PlayerState.WALKING || state == PlayerState.RUN)
		{			
			if(left_pressed)
			{
				p_anim_controller.FlipSprite(true);
				facing_direction = -1.0f;
			}
			if(right_pressed)
			{
				p_anim_controller.FlipSprite(false);
				facing_direction = 1.0f;
			}
			if(left_released)
			{
				if(right_hold)
				{
					p_anim_controller.FlipSprite(false);
					facing_direction = 1.0f;
				}
			}
			if(right_released)
			{
				if(left_hold)
				{
					p_anim_controller.FlipSprite(true);
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
				//air_drift = Mathf.Min(Mathf.Max(air_drift, 0),air_drift);
				motion = new Vector3(air_drift, motion.y, 0);
			}
		}
	}

	public void HandleAction(float deltaTime)
	{
		accumalator += track_time * deltaTime;
		if(p_anim_controller.GetState() == PlayerAnimationController.AnimationState.NONE)
		{
			state = PlayerState.IDLE;
		}
		if(p_anim_controller.GetState() == PlayerAnimationController.AnimationState.AERIAL_TO_JUMP)
		{
			state = PlayerState.AIRBORNE;
		}
		if(p_anim_controller.GetState() == PlayerAnimationController.AnimationState.CROUCHATTACK_TO_CROUCH)
		{
			if(state == PlayerState.TILT_ATTACK_DOWN)
			{
				state = PlayerState.CROUCHING;
			}
		}
		
		switch(state)
		{
			case PlayerState.IDLE:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.IDLE);
				motion = Vector3.Down;
				break;
			
			case PlayerState.WALKING:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.WALKING);
				motion = new Vector3(MOVE_SPEED * deltaTime * facing_direction,-0.1f,0);
				break;

			case PlayerState.CROUCHING:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.CROUCHING);
				break;

			case PlayerState.JAB_ATTACK1:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB1);
				break;
			
			case PlayerState.JAB_ATTACK2:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB2);
				break;

			case PlayerState.JAB_ATTACK3:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JAB3);
				break;

			case PlayerState.JAB_RAPID:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.RAPID_JAB);
				break;
			
			case PlayerState.TILT_ATTACK_DOWN:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.DOWN_TILT);
				break;
			
			case PlayerState.TILT_ATTACK_FORWARD:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.FORWARD_TILT);
				break;
			
			case PlayerState.TILT_ATTACK_UP:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.UP_TILT);
				break;

			case PlayerState.DASH_ATTACK:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.DASH_ATTACK);
				motion = new Vector3(facing_direction * deltaTime * RUNSPEED,motion.y,0);
				break;

			case PlayerState.WALKING_TO_RUN:
				if(accumalator >= treshold)
				{
					state = PlayerState.IDLE;
					ResetParameters();
				}
				break;

			case PlayerState.JUMP:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
				motion += Vector3.Up * JUMP_HEIGHT;
				falling_speed = NEUTRAL_FALL_SPEED;
				break;

			case PlayerState.AIRBORNE:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
				break;

			case PlayerState.AERIAL_ATTACK_NEUTRAL:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.AERIAL_NEUTRAL);
				break;
			
			case PlayerState.AERIAL_ATTACK_FORWARD:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.AERIAL_FORWARD);
				break;

			case PlayerState.AERIAL_ATTACK_BEHIND:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.AERIAL_BEHIND);
				break;

			case PlayerState.AERIAL_ATTACK_UP:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.AERIAL_UP);
				break;

			case PlayerState.AERIAL_ATTACK_DOWN:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.AERIAL_DOWN);
				break;

			case PlayerState.RUN:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.WALKING);
				motion = new Vector3(deltaTime * facing_direction * RUNSPEED,-0.1f,0);
				break;

			case PlayerState.TAUNT1:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.TAUNT1);
				break;
		}

		if(IsStateAerial())
		{
			motion -= Vector3.Up * GRAVITY * falling_speed;
			
			//Contains bug where bit manipulation doesn't go correctly
			if(motion.y < 0)
			{
				CollisionLayer = platform_bittracker;
				CollisionMask = platform_bittracker;
				if(print_once)
				{
					GD.Print("FALLING");
					PrintBits();
					print_once = false;
				}
			}
			//Contains bug where bit manipulation doesn't go correctly
			if(floored && !go_through_platform)
			{
				print_once = true;
				state = PlayerState.IDLE;
				
				uint keep_bits = 0;
				for(int i = 0; i < 15; i++)
				{
					keep_bits |= (uint)(1 << i) & CollisionMask;
				}
				platform_bittracker &= keep_bits;
				if(detected_go_through_platform != null)
				{
					platform_bittracker |= detected_go_through_platform.CollisionLayer;
				}

				CollisionLayer = platform_bittracker;
				CollisionMask = platform_bittracker;
				GD.Print("LANDED");
				PrintBits();
				ResetParameters();
				motion = Vector3.Down;
			}else
			{
				go_through_platform = false;
			}
		}
		if(IsStateGrounded() && !floored)
		{
			state = PlayerState.AIRBORNE;
			falling_speed = NEUTRAL_FALL_SPEED;
		}
		if(state == PlayerState.JUMP)
		{
			state = PlayerState.AIRBORNE;
		}
		if(double_down_platform_timer_on)
		{
			double_down_platform_timer += deltaTime;
			if(double_down_platform_timer >= double_down_platform_timer_treshold)
			{
				double_down_platform_timer_on = false;
			}
		}
	}

	private void FallThroughPlatform()
	{
		if(detected_go_through_platform != null)
		{
			state = PlayerState.AIRBORNE;
			uint keep_bits = 0;
			for(int i = 0; i < 15; i++)
			{
				keep_bits |= (uint)(1 << i) & CollisionMask;
			}
			platform_bittracker &= keep_bits;
			CollisionLayer = platform_bittracker;
			CollisionMask = platform_bittracker;
			go_through_platform = true;
		}
	}
	private void PrintBits()
	{
		string CollisionMaskPrint = "";
		string CollisionLayerPrint = "";
		string PlatformBitTrackerPrint = "";
		for(int i = 31; i >= 0; i--)
		{
			if(GetCollisionLayerBit(i))
			{
				CollisionLayerPrint += '1';
			}else
			{
				CollisionLayerPrint += '0';
			}
			if(GetCollisionMaskBit(i)) 
			{
				CollisionMaskPrint += '1';
			}else
			{
				CollisionMaskPrint += '0';
			}
			if((platform_bittracker & (1<<i)) == 1)
			{
				PlatformBitTrackerPrint += "1";
			}else
			{
				PlatformBitTrackerPrint += "0";
			}
		}
		GD.Print("Tracker: " + PlatformBitTrackerPrint);
		GD.Print("Mask: " + CollisionMaskPrint);
		GD.Print("Layer: " + CollisionLayerPrint);
	}

	private void ResetParameters()
	{
		motion = Vector3.Zero;
		treshold = 0f;
		track_time = 0f;
		accumalator = 0f;            
		air_drift = 0f;
		p_anim_controller.SetAnimationSpeed(1.0f);
	}

	private bool IsStateGrounded()
	{
		bool grounded = false;
		if(state == PlayerState.IDLE || state == PlayerState.CROUCHING || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN || state == PlayerState.RUN
			|| state == PlayerState.JAB_ATTACK1 || state == PlayerState.JAB_ATTACK2 || state == PlayerState.JAB_ATTACK3 || state == PlayerState.JAB_RAPID) grounded = true;
		return grounded;
	}

	private bool IsStateAerial()
	{
		bool aerial = false;
		if(state == PlayerState.AERIAL_ATTACK_BEHIND || state == PlayerState.AERIAL_ATTACK_DOWN || state == PlayerState.AERIAL_ATTACK_FORWARD || state == PlayerState.AERIAL_ATTACK_NEUTRAL || state == PlayerState.AERIAL_ATTACK_UP || state == PlayerState.AIRBORNE)
		{
			aerial = true;
		}
		return aerial;
	}
}
