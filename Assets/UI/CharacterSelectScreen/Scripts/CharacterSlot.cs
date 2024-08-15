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

    private bool connected = false;

    public override void _Ready()
    {
        GetChild<Area2D>(0).Connect("area_entered", this, "CharacterIsHovered");   
        GetChild<Area2D>(0).Connect("area_exited", this, "CharacterHoverExit");
        connected = true;
    }

    public override void _ExitTree()
    {
        GetChild<Area2D>(0).Disconnect("area_entered", this, "CharacterIsHovered");
        GetChild<Area2D>(0).Disconnect("area_exited", this, "CharacterHoverExit");
        connected = false;
    }

    private void CharacterHoverExit(Area2D area)
    {
        if(connected)
        {
            GD.Print(this.Name + "Entered");
            EmitSignal("HoverExited", character_resource);
        }
    }
    private void CharacterIsHovered(Area2D area)
    {
        if(connected)
        {
            GD.Print(this.Name + "Exited");
            EmitSignal("OnHover", character_resource);
        }
    }
}
