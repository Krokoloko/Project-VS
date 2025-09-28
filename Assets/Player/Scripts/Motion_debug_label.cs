using Godot;
using System;

public class Motion_debug_label : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    [Export]
    private PLAYER_CURSOR id;

    [Export]
    private NodePath player_container;

    private Player player;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        for(int i = 0; i < GetNode(player_container).GetChildCount(); i++)
        {
            player = GetNode(player_container).GetChildOrNull<Player>(i);
            if(player != null)
            {
                if(player.GetPlayerID() == id)
                {
                    break;
                }                
            }
        } 
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        var motion = player.GetMotion();
        Text = motion.ToString();
    }
}
