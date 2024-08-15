using Godot;
using System;
using System.Collections.Generic;

public class CharacterSelect : Node2D
{
	[Export]
	public NodePath[] slot_nodes;

	private CharacterSlot[] slots;

	private CharacterResources hovered_character;
	private CharacterResources selected_character;

	[Export]
	private NodePath ready_to_play_node = "";

	private Control ready_to_play_ui;

	[Export]
	public NodePath cursor_manager = "";
	private CursorManager cursors;

	[Export]
	private LevelResource load_this_level;

	bool is_loading;

	public override void _Ready()
	{
		is_loading = false;
		ready_to_play_ui = GetNode<Control>(ready_to_play_node);
		ready_to_play_ui.Visible = false;

		Array.Resize<CharacterSlot>(ref slots, slot_nodes.Length);
		for (int i = 0; i < slot_nodes.Length; i++)
		{
			slots[i] = GetNode<CharacterSlot>(slot_nodes[i]);
			slots[i].Connect("OnHover", this, "IsHoveredOnCharacter");	
			slots[i].Connect("HoverExited", this, "UnhoverCharacter");
		}
		cursors = GetNode<CursorManager>(cursor_manager);
	}

	private void UnhoverCharacter(CharacterResources character_info)
	{
		if(character_info.name == hovered_character.name)
		{
			GD.Print("Unhover");
			hovered_character = null;
		}
	}

	private void IsHoveredOnCharacter(CharacterResources character)
	{
		GD.Print("Hover");
		hovered_character = character;
	}
	public override void _Process(float delta)
	{
		if(Input.IsActionJustPressed("Goto_Trophy_Lottery"))
		{
			Node root = GetNode<Node>("../../");
			Node sceneroot = GetNode<Node>("../");
			sceneroot.GetChild<AudioStreamPlayer2D>(sceneroot.GetChildCount()-1).Stop();	
			sceneroot.QueueFree();
			Node trophy_gallery = GD.Load<PackedScene>("res://Assets/UI/TrophyGallery/TrophyGallery.tscn").Instance<Node>();
			root.AddChild(trophy_gallery);
		}
		if(Input.IsActionJustPressed("ui_accept") && selected_character != null)
		{
			CharacterResources load_character = selected_character;
			Node root = GetNode<Node>("../../");
			Node sceneroot = GetNode<Node>("../");
			sceneroot.GetChild<AudioStreamPlayer2D>(sceneroot.GetChildCount()-1).Stop();

			GD.Print("Unload scene");
			sceneroot.QueueFree();

			GD.Print("Load Level");
			TargetsGamemode level = GD.Load<PackedScene>(load_this_level.level_source).Instance<TargetsGamemode>();
			Player player = GD.Load<PackedScene>(load_character.prefab).Instance<Player>();
			player.Translation = new Vector3(load_this_level.player_spawn_positions[0].x,load_this_level.player_spawn_positions[0].y,0);
			
			level.GetNode<Node>("./PlayerContainer").AddChild(player);
			root.AddChild(level);
		}
		UICursor[] a_cursors = cursors.GetUICursors();
		for (int i = 0; i < a_cursors.Length; i++)
		{
			if(hovered_character != null)
			{
				if(a_cursors[i].GetClickState())
				{
					selected_character = hovered_character;
					ready_to_play_ui.Visible = true;
					ready_to_play_ui.GetChild<Sprite>(0).Texture = selected_character.portrait;
					ready_to_play_ui.GetChild<Sprite>(0).Scale = selected_character.portrait_scale;
					ready_to_play_ui.GetChild<Label>(1).Text = selected_character.name;
					GD.Print(selected_character.name);
				}
			}
		}
	}
}
