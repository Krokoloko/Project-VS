using Godot;
using System;

public class LevelResource : Resource
{
    [Export]
    public string level_source; 

    [Export]
    public int ID;

    [Export]
    public Texture thumbnail;

    [Export]
    public Vector2[] player_spawn_positions;

    LevelResource():this("", 0, null, null){}
    LevelResource(string p_level_source, int p_ID, Texture p_thumbnail, Vector2[] p_player_spawn_positions)
    {
        level_source = p_level_source;
        ID = p_ID;
        thumbnail = p_thumbnail;
        player_spawn_positions = p_player_spawn_positions;
    }
}