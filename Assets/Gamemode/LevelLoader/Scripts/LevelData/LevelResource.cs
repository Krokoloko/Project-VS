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
    public Texture title;

    [Export]
    public Texture logo;

    [Export]
    public Vector2[] player_spawn_positions;

    [Export]
    public string song_id;

    LevelResource() : this("", 0, null, null, null, null, "none") { }
    LevelResource(string p_level_source, int p_ID, Texture p_thumbnail, Texture p_title, Texture p_logo, Vector2[] p_player_spawn_positions, string p_song_id)
    {
        level_source = p_level_source;
        ID = p_ID;
        thumbnail = p_thumbnail;
        title = p_title;
        logo = p_logo;
        player_spawn_positions = p_player_spawn_positions;
        song_id = p_song_id;
    }
}