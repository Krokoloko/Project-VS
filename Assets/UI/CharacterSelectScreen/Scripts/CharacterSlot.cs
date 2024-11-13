using Godot;
using System;

public class CharacterSlot : Node
{
    [Signal]
    public delegate void OnHover(CharacterResources character_info, PLAYER_CURSOR player);

    [Signal]
    public delegate void HoverExited(CharacterResources character_info, PLAYER_CURSOR player);

    [Export]
    private CharacterResources character_resource;

    private bool connected = false;

    public override void _Ready()
    {
        GetChild<Area2D>(0).Connect("area_entered", this, "CharacterIsHovered");   
        GetChild<Area2D>(0).Connect("area_exited", this, "CharacterHoverExit");
    }

    private void CharacterHoverExit(Area2D area)
    {
        var cursor = area.GetNodeOrNull<UICursor>("../");
        if(cursor != null)
        {
            GD.Print(this.Name + "Entered");
            EmitSignal("HoverExited", character_resource, cursor.player);
        }
    }
    private void CharacterIsHovered(Area2D area)
    {
        var cursor = area.GetNodeOrNull<UICursor>("../");
        if(cursor != null)
        {
            GD.Print(this.Name + "Entered");
            EmitSignal("OnHover", character_resource, cursor.player);
        }
    }
}
