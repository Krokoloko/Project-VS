using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class StageSelectScript : Node
{
    [Export]
    private List<LevelResource> stages;

    private LevelResource selected_stage;

    [Export]
    private NodePath cursor_manager_nodepath;
    private CursorManager cursor_manager;

    [Export]
    //change this to a class later
    private string stage_ui_element_file;
    private PackedScene stage_ui_element;

    [Export]
    private NodePath parent_path;

    private GridContainer parent;

    private UICursor[] uiCursors;

    private Dictionary<PLAYER_CURSOR, CharacterResources> selected_characters = new Dictionary<PLAYER_CURSOR, CharacterResources>();

    private MusicAndSoundManager audio_manager;

    public override void _Ready()
    {
        audio_manager = GetNode<MusicAndSoundManager>("/root/AudioManager");
        parent = GetNode<GridContainer>(parent_path);
        cursor_manager = GetNode<CursorManager>(cursor_manager_nodepath);
        uiCursors = cursor_manager.GetUICursors();
        if(stage_ui_element_file != null)
        {       
            stage_ui_element = GD.Load<PackedScene>(stage_ui_element_file);
            for(int i = 0; i < stages.Count; i++)
            {
                StageThumbnail element = stage_ui_element.Instance<StageThumbnail>();

                element.resource = stages[i];

                element.Connect("OnHover", this, "SelectStage");
                element.Connect("UnHover", this, "UnselectStage");

                parent.AddChild(element);
            }
        }
    }

    private void SelectStage(LevelResource resource)
    {
        audio_manager.PlaySFX(MusicAndSoundManager.SFX.HOVER);
        selected_stage = resource;
    }

    private void UnselectStage(LevelResource resource)
    {
        if(selected_stage == resource)
        {
            selected_stage = null;
        }
    }

    public override void _Process(float delta)
    {
        bool loaded = false;
        if(selected_stage != null)
        {
            for(int i = 0; i < uiCursors.Length; i++)
            {
                if(!loaded && uiCursors[i].GetClickState() == CURSOR_CLICK_STATE.START)
                {
                    this.QueueFree();
                    Node level = GD.Load<PackedScene>(selected_stage.level_source).Instance<Node>();
                    
                    for(int j = 0; j < selected_characters.Keys.Count; j++)
                    {
                        var character = selected_characters.Values.ToArray()[j];
                        var cursor = selected_characters.Keys.ToArray()[j];
                        Player player = GD.Load<PackedScene>(character.prefab).Instance<Player>();
                        player.SetPlayerID(cursor);
                        player.SetStockCount(3);
                        player.SetPlayerName(character.name);
                        player.Translation = new Vector3(selected_stage.player_spawn_positions[j].x, selected_stage.player_spawn_positions[j].y, 0);
                        level.GetNode<Node>("./PlayerContainer").AddChild(player);
                    }
                    audio_manager.PlayBGM(selected_stage.song_id);
                    Node world = GetParent<Node>();
                    world.AddChild(level);
                    loaded = true;
                }
            }
        }
    }

    public void AddCharacter(PLAYER_CURSOR cursor, CharacterResources selected_character)
    {
        if(!selected_characters.Keys.Contains(cursor))
        {
            selected_characters.Add(cursor, selected_character);
        }
    }
}
