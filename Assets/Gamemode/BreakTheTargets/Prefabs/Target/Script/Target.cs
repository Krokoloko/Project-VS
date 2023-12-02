using Godot;
using System;

public class Target : Node
{
	[Export]
	NodePath hurtbox_node;

	[Export]
	NodePath audioplayer_node;

	[Export]
	NodePath sprite_node;


	Sprite3D sprite;
	AudioStreamPlayer3D audio_player;
	Hurtbox hurtbox;
	bool dying;
	public override void _Ready()
	{
		dying = false;
		hurtbox = GetNode<Hurtbox>(hurtbox_node);
		audio_player = GetNode<AudioStreamPlayer3D>(audioplayer_node);
		sprite = GetNode<Sprite3D>(sprite_node);

		audio_player.Connect("finished", this, "Destroy");
	}

	public bool IsDead()
	{
		return dying;
	}

	public override void _PhysicsProcess(float delta)
	{
		if(hurtbox.IsOverLapped())
		{
			if(!dying)
			{
				dying = true;
				audio_player.Play();
				sprite.Opacity = 0.0f;				
			}
		}
	}

	private void Destroy()
	{
		this.QueueFree();
		//GD.Print("Target destroyed");
	}

	public override void _Process(float delta)
	{
		if(dying)
		{
			//GD.Print(audio_player.GetPlaybackPosition() + delta, " >= ", audio_player.Stream.GetLength());
			if(audio_player.GetPlaybackPosition() + delta*3 >= audio_player.Stream.GetLength())
			{
				this.QueueFree();
				//GD.Print("Target destroyed");
			}
		}
	}
}
