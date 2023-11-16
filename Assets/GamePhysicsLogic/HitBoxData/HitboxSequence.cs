using Godot;
using System;

public partial class HitboxSequence : Resource
{
	[Export]
	public HitboxData[] sequence;

	public int total_frames{get{return p_total_frames;}}
	private int p_total_frames;

	public HitboxSequence() : this(null) {}
	
	public HitboxSequence(HitboxData[] p_sequence)
	{
		sequence = p_sequence;
		if(sequence == null) return;
		p_total_frames = p_sequence.Length;
	}


}
