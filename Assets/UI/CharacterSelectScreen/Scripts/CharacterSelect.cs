using Godot;
using System;
using System.Collections.Generic;

public class CharacterSelect : Node2D
{
	[Export]
	public NodePath[] slot_nodes;

	[Export]
	public NodePath[] player_portrait_nodes;
	private Dictionary<PLAYER_CURSOR, PlayerSelectPortrait> player_portraits;
	private CharacterSlot[] slots;

	private CharacterResources[] hovered_characters;

	[Export]
	private NodePath ready_to_play_node = "";

	private Control ready_to_play_ui;

	[Export]
	public NodePath cursor_manager = "";
	private CursorManager cursors;

	[Export]
	private string stage_select_menu_file;
	private PackedScene stage_select_menu;

	[Export]
	private LevelResource load_this_level;

	[Export]
	private LevelResource testLevel;

	[Export]
	private NodePath testButtonNode = "";
	private Button testButton;

	private bool ready;
	private bool is_loading;

	public override void _Ready()
	{
		ready = false;
		is_loading = false;
		stage_select_menu = GD.Load<PackedScene>(stage_select_menu_file);
		
		testButton = GetNodeOrNull<Button>(testButtonNode);
		testButton?.Connect("button_down", this, "LoadTestLevel");
		ready_to_play_ui = GetNode<Control>(ready_to_play_node);
		ready_to_play_ui.Visible = false;
		
		player_portraits = new Dictionary<PLAYER_CURSOR, PlayerSelectPortrait>();
		hovered_characters = new CharacterResources[(int)PLAYER_CURSOR.COUNT];
		for(int i = 0; i < player_portrait_nodes.Length; i++)
		{
			PlayerSelectPortrait node = GetNode<PlayerSelectPortrait>(player_portrait_nodes[i]);
			player_portraits.Add(node.GetPlayer(), node);
		}

		Array.Resize<CharacterSlot>(ref slots, slot_nodes.Length);
		for (int i = 0; i < slot_nodes.Length; i++)
		{
			slots[i] = GetNode<CharacterSlot>(slot_nodes[i]);
			slots[i].Connect("OnHover", this, "IsHoveredOnCharacter");	
			slots[i].Connect("HoverExited", this, "UnhoverCharacter");
		}
		cursors = GetNode<CursorManager>(cursor_manager);
	}

	private void LoadTestLevel()
	{
		if(ready && testLevel != null)
		{
			LoadLevel(cursors.GetUICursors(), testLevel);
		}
	}

	private void UnhoverCharacter(CharacterResources character_info, PLAYER_CURSOR cursor)
	{
		if(character_info.name == hovered_characters[(int)(cursor)].name)
		{
			GD.Print("Unhover");
			hovered_characters[(int)(cursor)] = null;
		}
	}

	private void IsHoveredOnCharacter(CharacterResources character, PLAYER_CURSOR cursor)
	{
		GD.Print("Hover");
		hovered_characters[(int)(cursor)] = character;
	}

	private void LoadLevel(UICursor[] a_cursors, LevelResource levelResource)
	{
		CharacterResources[] load_character = new CharacterResources[a_cursors.Length];
		for(int j = 0; j < a_cursors.Length; j++)
		{
			load_character[j] = player_portraits[(PLAYER_CURSOR)(j)].GetCharacter();
		} 
		Node root = GetNode<Node>("../../");
		Node sceneroot = GetNode<Node>("../");
		sceneroot.GetChild<AudioStreamPlayer2D>(sceneroot.GetChildCount()-1).Stop();

		GD.Print("Unload scene");
		sceneroot.QueueFree();

		GD.Print("Load Level");
		Node level = GD.Load<PackedScene>(levelResource.level_source).Instance<Node>();
		for(int j = 0; j < a_cursors.Length; j++)
		{
			//Todo: set input binding of the players
			Player player = GD.Load<PackedScene>(load_character[j].prefab).Instance<Player>();
			player.SetPlayerID(a_cursors[j].player);
			player.SetStockCount(3);
			player.SetPlayerName(load_character[j].name);
			player.Translation = new Vector3(load_this_level.player_spawn_positions[j].x, load_this_level.player_spawn_positions[j].y, 0);
			level.GetNode<Node>("./PlayerContainer").AddChild(player);
		}
		for(int j = 0; j < slots.Length; j++)
		{
			slots[j]._ExitTree();
		}
		
		root.AddChild(level);
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
		UICursor[] a_cursors = cursors.GetUICursors();
		for (int i = 0; i < a_cursors.Length; i++)
		{
			if(hovered_characters[i] != null)
			{
				var click_state = a_cursors[i].GetClickState();
				if(click_state == CURSOR_CLICK_STATE.START)
				{
					bool test = true;
					player_portraits[a_cursors[i].player].SetCharacter(hovered_characters[i]);

					GD.Print(hovered_characters[i].name);
					for(int j = 0; j < a_cursors.Length; j++)
					{
						test = test && player_portraits[a_cursors[j].player].IsSelected();
					}
					ready_to_play_ui.Visible = test;
					ready = test;
				}
				else if(Input.IsActionJustPressed("Player_Pause_" + (i+1)) && ready)
				{
					CharacterResources[] load_character = new CharacterResources[a_cursors.Length];
					for(int j = 0; j < a_cursors.Length; j++)
					{
						load_character[j] = player_portraits[(PLAYER_CURSOR)(j)].GetCharacter();
					}
					Node root = GetNode<Node>("../../");
					Node sceneroot = GetNode<Node>("../");
					sceneroot.GetChild<AudioStreamPlayer2D>(sceneroot.GetChildCount()-1).Stop();

					GD.Print("Unload scene");
					sceneroot.QueueFree();

					GD.Print("Load Level");

					if(stage_select_menu != null)
					{
						StageSelectScript stageSelect = stage_select_menu.Instance<StageSelectScript>();
						
						for(int j = 0; j < a_cursors.Length; j++)
						{
							stageSelect.AddCharacter(a_cursors[j].player, load_character[j]);
						}

						root.AddChild(stageSelect);
						break;
					}else
					{
						Node level = GD.Load<PackedScene>(load_this_level.level_source).Instance<Node>();
						for(int j = 0; j < a_cursors.Length; j++)
						{
							Player player = GD.Load<PackedScene>(load_character[j].prefab).Instance<Player>();
							player.SetPlayerID(a_cursors[j].player);
							player.SetStockCount(3);
							player.SetPlayerName(load_character[j].name);
							player.Translation = new Vector3(load_this_level.player_spawn_positions[j].x, load_this_level.player_spawn_positions[j].y, 0);
							level.GetNode<Node>("./PlayerContainer").AddChild(player);
						}
						for(int j = 0; j < slots.Length; j++)
						{
							slots[j]._ExitTree();
						}
						
						root.AddChild(level);
						break;
					}
				}
			}
		}
	}
}
