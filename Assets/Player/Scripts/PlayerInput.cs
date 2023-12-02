using Godot;
using System;

public partial class PlayerController : Resource
{
    [Export]
    public int player_number;
    
    [Export]
    public InputEvent left {get; set;}

    [Export]
    public InputEvent right {get; set;}

    [Export]
    public InputEvent up {get; set;}

    [Export]
    public InputEvent down {get; set;}

    [Export]
    public InputEvent attack {get; set;}

    [Export]
    public InputEvent special {get; set;}
    
    [Export]
    public InputEvent shield{get; set;}

    public PlayerController() : this(0,null,null,null,null,null,null,null){}
    public PlayerController(int a_player_number,InputEvent a_left, InputEvent a_right, InputEvent a_up, InputEvent a_down, InputEvent a_attack, InputEvent a_special, InputEvent a_shield)
    {
        player_number = a_player_number;
        left = a_left;
        right = a_right;
        up = a_up;
        down = a_down;
        attack = a_attack;
        special = a_special;
        shield = a_shield;
    }

    public void SetPlayerControllerInputBindings()
    {
        //string 
    }

}   