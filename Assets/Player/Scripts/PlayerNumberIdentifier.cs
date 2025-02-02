using Godot;
using System;
using System.Collections.Generic;

public class PlayerNumberIdentifier : Sprite3D
{
    [Export]
    private NodePath player;
    private Player player_node;
    [Export]
    //highlight when initialised
    private bool show;
    [Export]
    //how long it should wait to show up
    private float timer;
    private float accumalator = 0;
    
    [Export]
    private string player_icon_folder;

    private Texture player_id;
    private string player_id_string;

    public override void _Ready()
    {
        player_node = GetNode<Player>(player);
        player_id = GetCursorSprite(player_node.GetPlayerID());
        player_id_string = ((int)player_node.GetPlayerID()+1).ToString();
        if(show)
        {
            Texture = player_id;
        }else
        {
            accumalator = timer;
        }
    }

    private Texture GetCursorSprite(PLAYER_CURSOR player)
    {
        Directory directory = new Directory();
        Texture player_icon = null;
        if(directory.Open(player_icon_folder) == Error.Ok)
        {
            directory.ListDirBegin(true, true);
            string file_name = "";
            int loop = (int)player+1;
            for(int i = 0; i < loop; i++)
            {
                file_name = directory.GetNext();
                if(file_name.Contains(".import"))
                {
                    i--;
                }
            }
            player_icon = GD.Load<Texture>(player_icon_folder + file_name);
            directory.ListDirEnd();
        }
        return player_icon;
    }

    public bool AnyPlayerInput()
    {
        return Input.IsActionPressed("Player_Left_" + player_id_string) || Input.IsActionPressed("Player_Right_" + player_id_string)
        || Input.IsActionPressed("Player_Down_" + player_id_string) || Input.IsActionPressed("Player_Up_" + player_id_string)
        || Input.IsActionPressed("Player_Attack_" + player_id_string) || Input.IsActionPressed("Player_Special_" + player_id_string)
        || Input.IsActionPressed("Player_Jump_" + player_id_string);
    }

    public override void _Process(float delta)
    {
        bool input = AnyPlayerInput();
        if(!show)
        {
            accumalator -= delta;
            if(accumalator <= 0)
            {
                show = true;
                Texture = player_id;
            }
        }
        //reset timer
        if(input)
        {
            show = false;
            Texture = null;
            accumalator = timer;
        }
    }
}
