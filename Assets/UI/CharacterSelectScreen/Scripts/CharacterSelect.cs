using Godot;
using System;
using System.Collections.Generic;

public class CharacterSelect : Node2D
{
	public enum Character
	{
		NONE,
		DK,
		WARIO,
		KIRBY
	}
	public struct Slot
	{
		public Character character_type;
		public Vector2 position;
		public Vector2 rectangle_size;
		public bool selectable;
	}

	[Export]
	public int slots_width;
	[Export]
	public int slots_height;

	[Export]
	public Slot[] slots;

	private Dictionary<UICursor.PLAYER_CURSOR, Character> characters;

	[Export]
	public NodePath cursor_manager;
	private CursorManager cursors;

	public override void _Ready()
	{
		Array.Resize<Slot>(ref slots, slots_width * slots_height);
		cursors = GetNode<CursorManager>(cursor_manager);
		characters.Add(UICursor.PLAYER_CURSOR.P1, Character.NONE);
		characters.Add(UICursor.PLAYER_CURSOR.P2, Character.NONE);
		characters.Add(UICursor.PLAYER_CURSOR.P3, Character.NONE);
		characters.Add(UICursor.PLAYER_CURSOR.P4, Character.NONE);
	}

	public override void _Process(float delta)
	{
		var a_cursors = cursors.GetUICursors();
		for (int i = 0; i < a_cursors.Length; i++)
		{
			if(a_cursors[i].GetClickState())
			{
				for (int j = 0; j < slots.Length; j++)
				{
					if(IsWithinSlotBoundries(slots[j], a_cursors[i]))
					{
						// Cursor has selected the character slot
						if(a_cursors[i].player == UICursor.PLAYER_CURSOR.P1)
						{
							characters[a_cursors[i].player] = slots[j].character_type;
						}
					}
				}
			}
		}
	}

	private bool IsWithinSlotBoundries(Slot slot, UICursor cursor)
	{
		Vector2 half_rect = slot.rectangle_size*0.5f;
		float minx = slot.position.x - half_rect.x;
		float miny = slot.position.y - half_rect.y;
		float maxx = slot.position.x + half_rect.x;
		float maxy = slot.position.y + half_rect.y;
		return slot.selectable && cursor.Position.x + cursor.select_point.x >= minx && cursor.Position.y + cursor.select_point.y >= miny &&
		 cursor.Position.x + cursor.select_point.x <= maxx && cursor.Position.y + cursor.select_point.y <= maxy;
	}
}
