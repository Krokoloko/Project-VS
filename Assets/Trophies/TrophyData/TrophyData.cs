using Godot;
using System;

public class TrophyData : Resource
{
    [Export]
    public string name;
    [Export]
    public Texture sprite;
    [Export]
    public Texture easter_egg_sprite;
    [Export]
    public AudioStream easter_egg_sound;
    [Export(PropertyHint.MultilineText)]
    public string description;
    [Export]
    public string franchise;
    public TrophyData() : this("",null,null,null,"","") {}

    public TrophyData(string p_name, Texture p_sprite, Texture p_easter_egg_sprite, AudioStream p_easter_egg_sound, string p_description, string p_franchise)
    {
        name = p_name;
        sprite = p_sprite;
        easter_egg_sprite = p_easter_egg_sprite;
        easter_egg_sound = p_easter_egg_sound;
        description = p_description;
        franchise = p_franchise;
    }
}
