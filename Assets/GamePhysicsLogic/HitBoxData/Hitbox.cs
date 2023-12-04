using Godot;
using System;
using System.Diagnostics;
using System.Drawing.Text;

public class Hitbox : Spatial
{

	public int frames = 0;
	private const float time_per_frame = 1.0f/5.0f;
	private float timer;
	private StaticBody physics_body;
	private CollisionShape collider;

	public override void _Ready()
	{
		timer = 0.0f;
		physics_body = GetChild<StaticBody>(0);
		collider = physics_body.GetChild<CollisionShape>(0);
	}
	public override void _Process(float delta)
	{
		timer += delta;
        //GD.Print(collider.Shape.ResourceName);
        //DebugDrawCS.DrawBox(Transform.origin, collider.Scale, Color.Color8(255,0,0,255), true, delta);
		int frames_passed = (int)(timer/time_per_frame);
		if(frames <= frames_passed) 
		{
			//GD.Print("Delete hitbox");
			this.QueueFree();
		}
	}
}
