using Godot;
using System;

public class CharacterSlot : Node
{
    [Signal]
    public delegate void OnHover(CharacterResources character_info);

    [Signal]
    public delegate void HoverExited(CharacterResources character_info);

    [Export]
    private CharacterResources character_resource;

    public override void _Ready()
    {
        GetChild<Area2D>(0).Connect("area_entered", this, "CharacterIsHovered");   
        GetChild<Area2D>(0).Connect("area_exited", this, "CharacterHoverExit");
    }

    private void CharacterHoverExit(Area2D area)
    {
        EmitSignal("HoverExited", character_resource);
    }
    private void CharacterIsHovered(Area2D area)
    {
        EmitSignal("OnHover", character_resource);
    }
}
