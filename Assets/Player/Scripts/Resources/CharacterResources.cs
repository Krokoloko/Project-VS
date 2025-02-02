using Godot;
using System;

public class CharacterResources : Resource
{
    //Root node must be a Player class right now.
    [Export]
    public string prefab;

    [Export]
    public Texture portrait;
    [Export]
    public Vector2 portrait_scale;
    [Export]
    public string name;
    [Export]
    public Texture stock_icon;
    [Export]
    public Vector2 icon_scale;
    [Export]
    public AudioStream jab1_sfx;
    [Export]
    public AudioStream jab2_sfx;    
    [Export]
    public AudioStream jab3_sfx;
    [Export]
    public AudioStream dashattack_sfx;
    [Export]
    public AudioStream downtilt_sfx;
    [Export]
    public AudioStream forwardtilt_sfx;
    [Export]
    public AudioStream uptilt_sfx;
    [Export]
    public AudioStream neutralair_sfx;
    [Export]
    public AudioStream downair_sfx;
    [Export]
    public AudioStream forwardair_sfx;
    [Export]
    public AudioStream upair_sfx;
    [Export]
    public AudioStream backair_sfx;
    [Export]
    public AudioStream gethit_sfx;
    [Export]
    public AudioStream footstep_sfx;
    [Export]
    public AudioStream jump_sfx;
    [Export]
    public AudioStream doublejump_sfx;
    [Export]
    public AudioStream die_sfx;
    [Export]
    public AudioStream taunt1_sfx;

    public CharacterResources(): this(null, null, "None", null, new Vector2(), null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null){}
    public CharacterResources(string p_prefab, Texture p_portrait, string p_name, Texture p_stock_icon, Vector2 p_icon_scale,
        AudioStream p_jab1_sfx,
        AudioStream p_jab2_sfx,    
        AudioStream p_jab3_sfx,
        AudioStream p_dashattack_sfx,
        AudioStream p_downtilt_sfx,
        AudioStream p_forwardtilt_sfx,
        AudioStream p_uptilt_sfx,
        AudioStream p_neutralair_sfx,
        AudioStream p_downair_sfx,
        AudioStream p_forwardair_sfx,
        AudioStream p_upair_sfx,
        AudioStream p_backair_sfx,
        AudioStream p_gethit_sfx,
        AudioStream p_footstep_sfx,
        AudioStream p_jump_sfx,
        AudioStream p_doublejump_sfx,
        AudioStream p_die_sfx,
        AudioStream p_taunt1_sfx)
    {
        prefab = p_prefab;
        portrait = p_portrait;
        name = p_name;
        stock_icon = p_stock_icon;
        icon_scale = p_icon_scale;
        jab1_sfx = p_jab1_sfx;
        jab2_sfx = p_jab2_sfx;    
        jab3_sfx = p_jab3_sfx;
        dashattack_sfx = p_dashattack_sfx;
        downtilt_sfx = p_downtilt_sfx;
        forwardtilt_sfx = p_forwardtilt_sfx;
        uptilt_sfx = p_uptilt_sfx;
        neutralair_sfx = p_neutralair_sfx;
        downair_sfx = p_downair_sfx;
        forwardair_sfx = p_forwardair_sfx;
        upair_sfx = p_upair_sfx;
        backair_sfx = p_backair_sfx;
        gethit_sfx = p_gethit_sfx;
        footstep_sfx = p_footstep_sfx;
        jump_sfx = p_jump_sfx;
        doublejump_sfx = p_doublejump_sfx;
        die_sfx = p_die_sfx;
        taunt1_sfx = p_taunt1_sfx;
    }    
}
