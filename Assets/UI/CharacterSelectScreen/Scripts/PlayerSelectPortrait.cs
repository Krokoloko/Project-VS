using Godot;
using System;

public class PlayerSelectPortrait : Node
{
    [Export]
    private PLAYER_CURSOR player;
    [Export]
    private NodePath character_label;
    private Label character_name;
    [Export]
    private NodePath player_label;
    private Label player_name;
    [Export]
    private NodePath portrait_node;
    private Sprite character_portrait;
    [Export]
    private NodePath shutteranimation_node;
    private AnimationPlayer shutter;
    private CharacterResources selected_character;
    private bool open;

    public PLAYER_CURSOR GetPlayer()
    {
        return player;
    }

    public bool IsSelected()
    {
        return selected_character != null;
    }

    public override void _Ready()
    {
        open = false;
        character_name = GetNode<Label>(character_label);
        player_name = GetNode<Label>(player_label);
        character_portrait = GetNode<Sprite>(portrait_node);
        shutter = GetNode<AnimationPlayer>(shutteranimation_node);
        selected_character = null;
        shutter.Play("Shutter_Close");

        string name = "Player " + ((int)player).ToString();
        player_name.Text = name; 
        character_name.Text = "???";
    }

    private void ToggleShutter()
    {
        open = !open;
        if(open)
        {
            shutter.Play("Shutter_Open");
        }else
        {
            shutter.Play("Shutter_Close");
        }
    }

    public void SetCharacter(CharacterResources character)
    {
        selected_character = character;
        ToggleShutter();
    }

    public CharacterResources GetCharacter()
    {
        return selected_character;
    }

    public override void _Process(float delta)
    {
        if(!shutter.IsPlaying() && selected_character != null && !open)
        {
            ToggleShutter();
            character_portrait.Texture = null;
        }
        if(!shutter.IsPlaying() && selected_character != null && open)
        {
            character_portrait.Texture = selected_character.portrait;
            character_portrait.Scale = selected_character.portrait_scale;
        }   
    }
}
