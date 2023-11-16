using Godot;
using System;

public partial class HitboxData : Resource
{
	[Export]
	public CollisionData[] composition;

	[Export]
	public int frames;

	public HitboxData() : this(null, 0){}
	public HitboxData(CollisionData[] p_composition, int p_frames)
	{
		composition = p_composition;
		frames = p_frames;
	}
}
