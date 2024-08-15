using Godot;
using System;

public class TrophyData : Resource
{
    [Export]
    public string name;
    [Export]
    public Texture sprite;
    [Export]
    public string description;
    [Export]
    public string franchise;
    public TrophyData() : this("",null,"","") {}

    public TrophyData(string p_name, Texture p_sprite, string p_description, string p_franchise)
    {
        name = p_name;
        sprite = p_sprite;
        description = p_description;
        franchise = p_franchise;
    }
}
