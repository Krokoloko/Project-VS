using Godot;
using System;
using System.Diagnostics;
using System.Drawing.Text;

public class Hitbox : Spatial
{

	public int frames = 0;
	public float time_per_frame = 1.0f/5.0f;
	public bool reverse = false;
	private float timer;
	private Area physics_body;
	public CollisionData hitboxData;
	private CollisionShape collider;

	public override void _Ready()
	{
		timer = 0.0f;
		physics_body = GetChild<Area>(0);
		collider = physics_body.GetChild<CollisionShape>(0);
		//Temp
		physics_body.Connect("body_entered",this,"DetectHurtbox");
		physics_body.Connect("area_entered",this,"DetectHurtbox");
	}

	private void DetectHurtbox(Node body)
	{
		Hurtbox hurtbox = GetNodeOrNull<Hurtbox>(body.GetPath() + "/..");
		Spatial position = GetNodeOrNull<Spatial>(body.GetPath() + "/..");
		GD.Print("Hurtbox found!");
		if(hurtbox != null)
		{
			hurtbox.ApplyHitbox(hitboxData, reverse);
		}
		else
		{
			GD.Print("Couldn't fetch hurtbox data");
		}
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
