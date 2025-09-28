using Godot;
using System;
using System.Runtime.InteropServices;

public class Hurtbox : Spatial
{
	
	bool is_being_overlapped;
	public Area area; 
	private Player player;
	[Export]
	private NodePath debug_node;
	private Sprite3D debug;
	private HitboxHandler hitboxHandler;
	public override void _Ready()
	{
		debug = GetNode<Sprite3D>(debug_node);
		hitboxHandler = GetNode<HitboxHandler>("/root/HitboxHandler");
		debug.Visible = hitboxHandler.IsDebug();
		is_being_overlapped = false;
		player = GetNodeOrNull<Player>("../");
		area = GetChild<Area>(0);
	}

	public void ApplyHitbox(CollisionData hitbox, bool reverse)
	{
		is_being_overlapped = true;
		if(player != null)
		{
			var p = player.GetPercentage();
			var d = hitbox.base_damage;
			float knockback_strength = hitbox.base_knockback + hitbox.growth_knockback * (p + d);
			//(((p/10 + p*d/20) * (200/(player.WEIGHT+100)) * 1.4f) + 18)*hitbox.growth_knockback+hitbox.base_knockback;
			Vector2 velocity = hitbox.GetKnockbackVector();
			if(reverse)
			{
				velocity.x = -velocity.x;
			}
			player.ApplyDamage(velocity, 90, knockback_strength, hitbox.base_damage);
		}
	}

	public override void _Process(float delta)
	{
		//DebugDrawCS.DrawAabb(new AABB(GlobalTranslation, Scale),Color.Color8(0,255,0,255),delta);
		if(Input.IsActionJustPressed("Debug_Hitboxes"))
		{
			debug.Visible = !debug.Visible;
		}
	}	

	public bool IsOverLapped()
	{
		return is_being_overlapped;
	}
}
