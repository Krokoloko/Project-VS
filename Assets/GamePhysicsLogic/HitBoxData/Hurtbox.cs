using Godot;
using System;
using System.Runtime.InteropServices;

public class Hurtbox : Spatial
{
	
	bool is_being_overlapped;
	public Area area; 
	private Player player;
	public override void _Ready()
	{
		is_being_overlapped = false;
		player = GetNodeOrNull<Player>("../");
		area = GetChild<Area>(0);
	}

	public void ApplyHitbox(CollisionData hitbox, bool reverse)
	{
		is_being_overlapped = true;
		if(player != null)
		{
			float knockback_strength = hitbox.base_knockback + hitbox.growth_knockback * (player.GetPercentage() + hitbox.base_damage);
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
		DebugDrawCS.DrawAabb(new AABB(GlobalTranslation, Scale),Color.Color8(0,255,0,255),delta);
	}	

	public bool IsOverLapped()
	{
		return is_being_overlapped;
	}
}
