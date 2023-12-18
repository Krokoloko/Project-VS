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

    public CharacterResources(): this(null, null, "None"){}
    public CharacterResources(string p_prefab, Texture p_portrait, string p_name)
    {
        prefab = p_prefab;
        portrait = p_portrait;
        name = p_name;
    }    
}
