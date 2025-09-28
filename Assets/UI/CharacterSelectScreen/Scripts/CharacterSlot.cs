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
    private MusicAndSoundManager audio_manager;

    public override void _Ready()
    {
        audio_manager = GetNode<MusicAndSoundManager>("/root/AudioManager");

        GetChild<Area2D>(0).Connect("area_entered", this, "CharacterIsHovered");   
        GetChild<Area2D>(0).Connect("area_exited", this, "CharacterHoverExit");
    }

    private void CharacterHoverExit(Area2D area)
    {
        var cursor = area.GetNodeOrNull<UICursor>("../");
        if (cursor != null)
        {
            if ((int)cursor.player >= 0 && (int)cursor.player <= 3)
            {
                GD.Print(this.Name + "Cursor Exited");
                EmitSignal("HoverExited", character_resource, cursor.player);
            }
            else
            {
                GD.PrintErr(this.Name + " has inappropriate cursor ID");   
            }
            
        }
    }
    private void CharacterIsHovered(Area2D area)
    {
        var cursor = area.GetNodeOrNull<UICursor>("../");
        audio_manager.PlaySFX(MusicAndSoundManager.SFX.HOVER);
        if (cursor != null)
        {
            if ((int)cursor.player >= 0 && (int)cursor.player <= 3)
            {
                GD.Print(this.Name + "Cursor Entered");
                EmitSignal("OnHover", character_resource, cursor.player);
            }
            else
            {
                GD.PrintErr(this.Name + " has inappropriate cursor ID");   
            }
        }
    }
}
