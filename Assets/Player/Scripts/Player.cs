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
		TAUNT3,
		LAUNCHED,
		TUMBLING,
		DEAD,
		NONE
	}

	[Signal]
	public delegate void OnPlayerDie();

	[Signal]
	public delegate void OnPlayerDamaged();

	[Export]
	public CharacterResources resource_reference;

	[Export]
	private NodePath sfx_player_node;
	public AudioStreamPlayer3D sfx_player;

	[Export]
	public float MOVE_SPEED = 10.0f;
	private const float FPS = 1.0f / 60.0f;
	//3 Frame window
	private const float DASH_INPUT_WINDOW_CONTROLLER = FPS * 3.0f;
	//4 Frame window
	private const float DASH_INPUT_WINDOW_KEYBOARD = FPS * 4.0f;

	[Export]
	public float RUNSPEED = 25.0f; 

	private Vector3 motion;
	[Export(PropertyHint.Range, "1,3")]
	public int total_jabs;
	public bool has_rapid_jab;

	[Export]
	public float JUMP_HEIGHT = 5.0f;
	
	[Export]
	public float AIR_ACCELERATION = 10.0f;

	[Export]
	public float AIR_SPEED = 30.0f;

	[Export]
	public float[] DOUBLE_JUMPS = null;

	public const float GRAVITY = 1f;
	
	[Export]
	public float NEUTRAL_FALL_SPEED = 1.0f;
	
	[Export]
	public float FAST_FALL_SPEED = 2.0f;
	
	[Export]
	public float WEIGHT = 1.0f;

	private float weight_divisor;
	private float facing_direction; 
	private float falling_speed;
	private float treshold;
	private float track_time;
	private float accumalator;
	private float air_drift;

	private Hurtbox hurtbox;
	public Area area;
	private PlayerState state;
	public PlayerAnimationController anim_controller{get {return p_anim_controller;}}
	private PlayerAnimationController p_anim_controller;
	private AttackController attack_controller;
	private PLAYER_CURSOR player_id;

	public int player_number;
	private float damage_percantage = 0.0f;
	private bool floored;
	private int double_jumps;
	private float current_jump_force = 0;
	private PhysicsBody detected_go_through_platform = null;
	private uint platform_bittracker = 0;
	private bool go_through_platform = false;
	private bool print_once = true;
	private const float double_down_platform_timer_treshold = 0.3f;
	private bool double_down_platform_timer_on = false;
	private float double_down_platform_timer = 0.0f;
	
	//knockback stuff
	private Vector3 applied_knockback;
	private float knockback_timer = 0.0f;
	private float knockback_accumalator = 0.0f;


	private string PLAYER_LEFT;
	private string PLAYER_RIGHT;
	private string PLAYER_UP;
	private string PLAYER_DOWN;
	private string PLAYER_ATTACK;
	private string PLAYER_SPECIAL;
	private string PLAYER_TAUNT1;
	private string PLAYER_JUMP;

	//Misc info
	private Vector3 launch_direction;
	private Vector2 respawn_location;
	private string character_name = "";
	private int stock_count = -1;
	private bool using_controller = true;
	private bool flip_back = false;
	public void SetPlayerID(PLAYER_CURSOR id)
	{
		player_id = id;
		PLAYER_LEFT = "Player_Left_" + ((int)id + 1);
		PLAYER_RIGHT = "Player_Right_" + ((int)id + 1);
		PLAYER_UP = "Player_Up_" + ((int)id + 1);
		PLAYER_DOWN = "Player_Down_" + ((int)id + 1);
		PLAYER_ATTACK = "Player_Attack_" + ((int)id + 1);
		PLAYER_SPECIAL = "Player_Special_" + ((int)id + 1);
		PLAYER_TAUNT1 = "Player_Taunt1_" + ((int)id + 1);
		PLAYER_JUMP = "Player_Jump_" + ((int)id + 1);
	}

	public Texture GetStockIcon()
	{
		return resource_reference.stock_icon;
	}

	public Vector2 GetStockScale()
	{
		return resource_reference.icon_scale;
	}

	public override void _Ready()
	{
		for (int i = 16; i < 32; i++)
		{
			platform_bittracker |= (uint)(1 << i);
		}
		GD.Print(platform_bittracker);
		sfx_player = GetNode<AudioStreamPlayer3D>(sfx_player_node);

		MOVE_SPEED *= 1.9f;
		RUNSPEED *= 1.9f;
		JUMP_HEIGHT /= 3.5f;
		NEUTRAL_FALL_SPEED /= 75;
		FAST_FALL_SPEED /= 75;
		AIR_SPEED /= 10;
		AIR_ACCELERATION /= 20;
		WEIGHT /= 60;

		for (int i = 0; i < DOUBLE_JUMPS.Length; i++)
		{
			DOUBLE_JUMPS[i] /= 3.5f;
		}

		player_number = 1;
		floored = true;
		double_jumps = DOUBLE_JUMPS.Length;
		air_drift = 0.0f;
		weight_divisor = 1 / WEIGHT;
		facing_direction = 1.0f;
		p_anim_controller = GetNode<PlayerAnimationController>(this.GetPath() + "/Sprite3D");
		p_anim_controller.FlipSprite(true);
		attack_controller = GetNode<AttackController>(this.GetPath() + "/AttackController");
		hurtbox = GetNode<Hurtbox>(this.GetPath() + "/Hurtbox");
		hurtbox.area.SetCollisionLayerBit((int)player_id + 1, true);
		hurtbox.area.SetCollisionLayerBit(5, false);
		Sprite3D sprite = GetNode<Sprite3D>(this.GetPath() + "/Sprite3D");
		state = PlayerState.IDLE;
		area = GetNode<Area>(this.GetPath() + "/Area");
		area.CollisionMask = platform_bittracker;
		area.Connect("body_entered", this, "AreaDetected");
		area.Connect("body_exited", this, "AreaExited");
		this.Connect("OnPlayerDie", this, "PlayDieSFX");
		platform_bittracker |= 1;
		accumalator = 0.0f;
		track_time = 0.0f;
		treshold = 0.0f;

		if (player_id == PLAYER_CURSOR.P1)
		{
			using_controller = false;	
		}
	}

	public void EndPlayer()
	{
		state = PlayerState.NONE;
	}

	public void RespawnToLocation(Vector2 position)
	{
		respawn_location = position;
	}
	private void Respawn()
	{
		if(state != PlayerState.NONE)
		{
			Translation = new Vector3(respawn_location.x, respawn_location.y, 0.0f);
			state = PlayerState.AIRBORNE;
			ResetParameters();
			sfx_player.Disconnect("finished", this, "Respawn");
		}
	}

	private void PlayDieSFX()
	{
		sfx_player.Stop();
		sfx_player.Stream = resource_reference.die_sfx;
		sfx_player.Play();
		sfx_player.Connect("finished", this, "Respawn");
		motion = Vector3.Zero;
		state = PlayerState.DEAD;
	}

	public void SetStockCount(int count)
	{
		if(stock_count == -1)
		{
			stock_count = Mathf.Clamp(count, 1, 99);
		}
	}
	public void SetPlayerName(string name)
	{
		character_name = name;
	}
	public string GetCharacterName()
	{
		return character_name;
	}
	public string GetPlayerName()
	{
		return character_name;
	}
	public int GetStockCount()
	{
		return stock_count;
	}
	public void SubtractStock()
	{
		stock_count--;
		damage_percantage = 0.0f;
		EmitSignal("OnPlayerDie");
		stock_count = Mathf.Clamp(stock_count, 0, 100);
	}
	public PLAYER_CURSOR GetPlayerID()
	{
		return player_id;
	}
	private void AreaDetected(Node body)
	{
		GD.Print("Platform detected");
		GD.PrintT(motion, body.Name, CollisionMask);
		PhysicsBody p_body = (PhysicsBody)(body);
		if(p_body.Name == "platform")
		{
			detected_go_through_platform = p_body;
			platform_bittracker |= p_body.CollisionLayer;
			CollisionMask = platform_bittracker;
		}
	}
	private void AreaExited(Node body)
	{
		PhysicsBody p_body = (PhysicsBody)(body);
		if(p_body == detected_go_through_platform)
		{
			GD.Print("Platform exited");
			GD.PrintT(motion, body.Name, CollisionMask);
			detected_go_through_platform = null;
		}
	}
	public override void _Process(float delta)
	{
		if(state != PlayerState.LAUNCHED && state != PlayerState.NONE && state != PlayerState.DEAD && stock_count > 0)
		{
			HandleInput();
			HandleAction(delta);
		}
		if(state == PlayerState.DEAD)
		{
			var playback_t = sfx_player.GetPlaybackPosition();
			if(playback_t >= 1.5f)
			{
				Respawn();
			}
		}
		ProcessKnockbackState(delta);
	}
	public override void _PhysicsProcess(float deltaTime)
	{
		MoveAndSlide(motion, Vector3.Up, true);
		floored = IsOnFloor();
	}
	public void SetPassThroughPlatformBits()
	{
		for(int i = 15; i < 32; i++)
		{
			SetCollisionMaskBit(i, ((1<<i) & platform_bittracker) > 0);
		}
	}
	public void ApplyDamage(Vector2 direction, int frames_hitstun, float knockback_strength, float damage)
	{
		launch_direction = new Vector3(direction.x, direction.y, 0);
		//frames_hitstun in 60fps
		damage_percantage += damage;
		float launch_distance = knockback_strength;

		// if(facing_direction < 0)
		// {
		// 	direction.x = -direction.x; 
		// }

		knockback_timer = frames_hitstun * (1/60.0f);
		knockback_accumalator = 0;

		state = PlayerState.LAUNCHED;

		applied_knockback = new Vector3(direction.x, direction.y, 0.0f) * launch_distance;

		motion = applied_knockback;

		sfx_player.Stop();
		sfx_player.Stream = resource_reference.gethit_sfx;
		sfx_player.Play();
		EmitSignal("OnPlayerDamaged");
		anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
	}
	public Vector3 GetMotion()
	{
		return motion;
	}
	public float GetPercentage()
	{
		return damage_percantage;
	}
	public bool IsLaunchState()
	{
		return knockback_accumalator > 0.0f;
	}
	private void ProcessKnockbackState(float deltaTime)
	{
		if(state == PlayerState.LAUNCHED)
		{
			knockback_accumalator += deltaTime;
			motion += (-applied_knockback*2 + Vector3.Down * GRAVITY * falling_speed) * deltaTime;
			if(knockback_accumalator > 2*(1.0f/60.0f))
			{
				if(floored)
				{
					state = PlayerState.IDLE;
					motion = Vector3.Zero;
				}
			}
			Vector3 motion_normal = motion.Normalized();

			if (knockback_accumalator >= knockback_timer || motion_normal.Dot(-launch_direction) >= 0.9)
			{
				//implement teching here
				if (floored)
				{
					//should be knockdown state in the future
					state = PlayerState.IDLE;
				}
				else
				{
					//should be tumble state in the future
					state = PlayerState.AIRBORNE;
					falling_speed = NEUTRAL_FALL_SPEED;

				}
				knockback_accumalator = 0;
				motion = Vector3.Zero;
			}
		}
	}
	private void HandleInput()
	{
		if(Input.IsActionJustPressed("PlayerDebugPrint"))
		{
			GD.Print(player_id);
			PrintBits();
		}
		bool reverse_direction = facing_direction<0?true:false;

		bool right_pressed = Input.IsActionJustPressed(PLAYER_RIGHT);
		bool right_hold = Input.IsActionPressed(PLAYER_RIGHT);
		bool right_released = Input.IsActionJustReleased(PLAYER_RIGHT);

		bool left_pressed = Input.IsActionJustPressed(PLAYER_LEFT);
		bool left_hold = Input.IsActionPressed(PLAYER_LEFT);
		bool left_released = Input.IsActionJustReleased(PLAYER_LEFT);

		bool down_hold = Input.IsActionPressed(PLAYER_DOWN);
		bool down_released = Input.IsActionJustReleased(PLAYER_DOWN);

		bool jump_pressed = Input.IsActionJustPressed(PLAYER_JUMP);

		bool up_pressed = Input.IsActionJustPressed(PLAYER_UP);
		bool up_hold = Input.IsActionPressed(PLAYER_UP);

		//Move Left or right
		if((left_pressed || left_hold) && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
			if(using_controller && Input.GetActionStrength(PLAYER_LEFT) >= 0.9f)
			{
				state = PlayerState.RUN;
				anim_controller.SetAnimationSpeed(2.0f);
			}else
			{
				state = PlayerState.WALKING;
				treshold = DASH_INPUT_WINDOW_CONTROLLER;
				track_time = 1;	
			}
		}
		if((right_pressed || right_hold) && state == PlayerState.IDLE)
		{
			state = PlayerState.WALKING;
			
			if (using_controller && Input.GetActionStrength(PLAYER_RIGHT) >= 0.9f)
			{
				state = PlayerState.RUN;
				anim_controller.SetAnimationSpeed(2.0f);
			}
			else
			{
				state = PlayerState.WALKING;
				treshold = DASH_INPUT_WINDOW_CONTROLLER;
				track_time = 1;
			}
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
				double_jumps = DOUBLE_JUMPS.Length;
				double_down_platform_timer = 0;
			}else
			{
				double_down_platform_timer_on = true;
				double_down_platform_timer = 0;
			}
		}

		if(jump_pressed && (state == PlayerState.IDLE || state == PlayerState.CROUCHING || state == PlayerState.DASH_ATTACK || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN || state == PlayerState.RUN))
		{
			anim_controller.SetAnimationSpeed(1.0f);
			if(state == PlayerState.CROUCHING)
			{
				FallThroughPlatform();
			}
			else
			{
				sfx_player.Stop();
				sfx_player.Stream = resource_reference.jump_sfx;
				sfx_player.Play();
				state = PlayerState.JUMP;
				current_jump_force = JUMP_HEIGHT;
			}
			double_jumps = DOUBLE_JUMPS.Length;
		}
		else if(state == PlayerState.AIRBORNE && double_jumps > 0 && jump_pressed)
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
			sfx_player.Stop();
			sfx_player.Stream = resource_reference.doublejump_sfx;
			sfx_player.Play();
			double_jumps--;
			current_jump_force = DOUBLE_JUMPS[DOUBLE_JUMPS.Length-1-double_jumps];
			motion = new Vector3(motion.x,0,0);
		}

		bool attack_preformed = false;

		if(Input.IsActionJustPressed(PLAYER_ATTACK))
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
						attack_controller.PreformAction(EACTION_TYPE.BACK_AERIAL, step, reverse_direction);
						sfx_player.Stop();
						sfx_player.Stream = resource_reference.backair_sfx;
						sfx_player.Play();
					}else
					{
						state = PlayerState.AERIAL_ATTACK_FORWARD;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_FORWARD]).Step;
						attack_controller.PreformAction(EACTION_TYPE.FORWARD_AERIAL, step, reverse_direction);
						sfx_player.Stop();
						sfx_player.Stream = resource_reference.forwardair_sfx;
						sfx_player.Play();
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
						attack_controller.PreformAction(EACTION_TYPE.FORWARD_AERIAL, step, reverse_direction);
						sfx_player.Stop();
						sfx_player.Stream = resource_reference.forwardair_sfx;
						sfx_player.Play();
					}else
					{
						state = PlayerState.AERIAL_ATTACK_BEHIND;
						var animation_player = anim_controller.GetAnimationPlayer();
						float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_BEHIND]).Step;
						attack_controller.PreformAction(EACTION_TYPE.BACK_AERIAL, step, reverse_direction);
						sfx_player.Stop();
						sfx_player.Stream = resource_reference.backair_sfx;
						sfx_player.Play();
					}
				}
				else if(up_hold)
				{
					state = PlayerState.AERIAL_ATTACK_UP;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_UP]).Step;
					attack_controller.PreformAction(EACTION_TYPE.UP_AERIAL, step, reverse_direction);
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.upair_sfx;
					sfx_player.Play();
				}
				else if(down_hold)
				{
					state = PlayerState.AERIAL_ATTACK_DOWN;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_DOWN]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DOWN_AERIAL, step, reverse_direction);
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.downair_sfx;
					sfx_player.Play();
				}
				if(state == PlayerState.AERIAL_ATTACK_NEUTRAL)
				{
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.AERIAL_NEUTRAL]).Step;
					attack_controller.PreformAction(EACTION_TYPE.NEUTRAL_AERIAL, step, reverse_direction);
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.neutralair_sfx;
					sfx_player.Play();
				}
			}
			
			if(IsStateGrounded())
			{
				p_anim_controller.SetAnimationSpeed(1.0f);
				if(up_hold)
				{
					state = PlayerState.TILT_ATTACK_UP;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.UP_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.UP_TILT, step, reverse_direction);
					attack_preformed = true;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.uptilt_sfx;
					sfx_player.Play();
				}
				if(!attack_preformed && state == PlayerState.WALKING)
				{
					state = PlayerState.TILT_ATTACK_FORWARD;
					var animation_player = anim_controller.GetAnimationPlayer();
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.FORWARD_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.FORWARD_TILT, step, reverse_direction);
					attack_preformed = true;
					motion = Vector3.Zero;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.forwardtilt_sfx;
					sfx_player.Play();
				}
				if(!attack_preformed && state == PlayerState.RUN)
				{
					state = PlayerState.DASH_ATTACK;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.DASH_ATTACK]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DASHATTACK, step, reverse_direction);
					attack_preformed = true;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.dashattack_sfx;
					sfx_player.Play();
				}
				if(!attack_preformed && state == PlayerState.CROUCHING)
				{
					state = PlayerState.TILT_ATTACK_DOWN;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.DOWN_TILT]).Step;
					attack_controller.PreformAction(EACTION_TYPE.DOWN_TILT, step, reverse_direction);
					attack_preformed = true;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.downtilt_sfx;
					sfx_player.Play();
				}
				//Handle Jab attacks
				if(!attack_preformed && state == PlayerState.IDLE)
				{
					state = PlayerState.JAB_ATTACK1;
					treshold = 1.0f;
					attack_preformed = true;
					var animation_player = anim_controller.GetAnimationPlayer();	
					float step = animation_player.GetAnimation(anim_controller.animation_names[PlayerAnimationController.AnimationState.JAB1]).Step;
					attack_controller.PreformAction(EACTION_TYPE.JAB1, step, reverse_direction);
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.jab1_sfx;
					sfx_player.Play();
				}
				else if(state == PlayerState.JAB_ATTACK1 && total_jabs > 1)
				{
					state = PlayerState.JAB_ATTACK2;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.jab2_sfx;
					sfx_player.Play();
				}
				else if(state == PlayerState.JAB_ATTACK2 && total_jabs > 2)
				{
					state = PlayerState.JAB_ATTACK3;
					sfx_player.Stop();
					sfx_player.Stream = resource_reference.jab3_sfx;
					sfx_player.Play();
				}
			}
		}

		//Stop walking
		if(left_released && !right_hold && state == PlayerState.WALKING)
		{
			state = PlayerState.WALKING_TO_RUN;
			if(!using_controller)
			{
				treshold = DASH_INPUT_WINDOW_KEYBOARD;
				track_time = 1;
			}
		}
		if(right_released && !left_hold && state == PlayerState.WALKING)
		{
			state = PlayerState.WALKING_TO_RUN;
			if(!using_controller)
			{
				treshold = DASH_INPUT_WINDOW_KEYBOARD;
				track_time = 1;
			}
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

		//Turn around while running for keyboard
		if(!using_controller)
		{
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
		}else
		{
			if(state == PlayerState.WALKING && track_time == 1 && Input.GetActionStrength(PLAYER_LEFT) >= 0.9)
			{
				state = PlayerState.RUN;
				p_anim_controller.SetAnimationSpeed(2.0f);
			}
			if(Input.GetActionStrength(PLAYER_RIGHT) >= 0.9f && state == PlayerState.WALKING && track_time == 1)
			{
				state = PlayerState.RUN;
				p_anim_controller.SetAnimationSpeed(2.0f);
			}
		}

		if(Input.IsActionJustPressed(PLAYER_TAUNT1) && (state == PlayerState.IDLE || state == PlayerState.WALKING || state == PlayerState.WALKING_TO_RUN))
		{
			state = PlayerState.TAUNT1;
			sfx_player.Stop();
			sfx_player.Stream = resource_reference.taunt1_sfx;
			sfx_player.Play();
		}

		if(state == PlayerState.WALKING || state == PlayerState.RUN)
		{			
			if(left_pressed || left_hold)
			{
				p_anim_controller.FlipSprite(true);
				facing_direction = -1.0f;
			}
			if(right_pressed || right_hold)
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
	private void HandleAction(float deltaTime)
	{
		accumalator += track_time * deltaTime;
		if(p_anim_controller.GetState() == PlayerAnimationController.AnimationState.NONE)
		{
			state = PlayerState.IDLE;
			if(flip_back)
			{
				p_anim_controller.FlipSprite(true);
				flip_back = false;
			}
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
				if(accumalator >= treshold)
				{
					track_time = 0;
					accumalator = 0;
				}
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
				if(accumalator >= treshold || using_controller)
				{
					state = PlayerState.IDLE;
					ResetParameters();
				}
				break;

			case PlayerState.JUMP:
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.JUMP);
				motion += Vector3.Up * current_jump_force;
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
				if(p_anim_controller.GetFlip())
				{
					flip_back = true;
					p_anim_controller.FlipSprite(false);
				}
				p_anim_controller.SetAnimation(PlayerAnimationController.AnimationState.TAUNT1);
				break;
		}

		if(IsStateAerial())
		{
			motion -= Vector3.Up * GRAVITY * falling_speed;
			
			if(motion.y < 0)
			{
				CollisionMask = platform_bittracker;
				if(print_once)
				{
					GD.Print("FALLING");
					print_once = false;
				}
			}
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

				CollisionMask = platform_bittracker;
				GD.Print("LANDED");
				ResetParameters();
				motion = Vector3.Down;
			}else
			{
				go_through_platform = false;
			}
			if(Input.IsActionPressed(PLAYER_DOWN) && !floored && state != PlayerState.AERIAL_ATTACK_DOWN)
			{
				state = PlayerState.AIRBORNE;
				uint keep_bits = 0;
				for(int i = 0; i < 15; i++)
				{
					keep_bits |= (uint)(1 << i) & CollisionMask;
				}
				platform_bittracker &= keep_bits;
				CollisionMask = platform_bittracker;
			}
		}
		if(IsStateGrounded() && !floored)
		{
			state = PlayerState.AIRBORNE;
			falling_speed = NEUTRAL_FALL_SPEED;
			double_jumps = DOUBLE_JUMPS.Length;
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