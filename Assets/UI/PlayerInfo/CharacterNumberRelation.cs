using Godot;
using System;
using System.Collections.Generic;

public class CharacterNumberRelation : Resource
{
    [Export]
    public string[] symbols;

    [Export]
    public Texture[] textures;

    [Export]
    public Vector2 scaling;

    public CharacterNumberRelation(string[] p_symbols, Texture[] p_textures, Vector2 p_scaling)
    {
        this.symbols = p_symbols;
        this.textures = p_textures;
        this.scaling = p_scaling;
    } 

    public CharacterNumberRelation() : this(new string[0], new Texture[0], Vector2.Zero){}
}
