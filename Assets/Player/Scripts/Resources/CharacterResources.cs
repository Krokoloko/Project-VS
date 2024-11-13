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

    public CharacterResources(): this(null, null, "None", null, new Vector2()){}
    public CharacterResources(string p_prefab, Texture p_portrait, string p_name, Texture p_stock_icon, Vector2 p_icon_scale)
    {
        prefab = p_prefab;
        portrait = p_portrait;
        name = p_name;
        stock_icon = p_stock_icon;
        icon_scale = p_icon_scale;
    }    
}
