using Godot;
using System;

public class Hurtbox : Spatial
{
	
	bool is_being_overlapped;
	private Area area; 
	public override void _Ready()
	{
		is_being_overlapped = false;
		area = GetChild<Area>(0);
		area.Connect("body_entered", this, "body_enter_area");
		//GD.Print(CollisionLayer);
	}

	public override void _Process(float delta)
	{
		DebugDrawCS.DrawAabb(new AABB(GlobalTranslation, Scale),Color.Color8(0,255,0,255),delta);
	}

	private void body_enter_area(Node body)
	{
		GD.Print("hitbox detected");
		is_being_overlapped = true;
	}
	

	public bool IsOverLapped()
	{
		return is_being_overlapped;
	}
}
