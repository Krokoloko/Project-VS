using Godot;
using System;

public class LevelResource : Resource
{
    [Export]
    public string level_source; 

    [Export]
    public Vector2[] player_spawn_positions;

    LevelResource():this("", null){}
    LevelResource(string p_level_source, Vector2[] p_player_spawn_positions)
    {
        level_source = p_level_source;
        player_spawn_positions = p_player_spawn_positions;
    }
}
