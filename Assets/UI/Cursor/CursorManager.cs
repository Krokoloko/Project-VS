using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

public class CursorManager : Node
{
    const int MAX_PLAYERS = 2;
    private Dictionary<int, string> index_string;
    private UICursor[] cursors;
    [Export]
    private NodePath world_node;
    [Export]
    private string previous_menu_file;
    private PackedScene previous_menu;
    public override void _Ready()
    {
        index_string = new Dictionary<int, string>();
        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            index_string.Add(i, (i+1).ToString());
        }
        int child_count = GetChildCount();
        cursors = new UICursor[child_count];
        for(int i = 0; i < child_count; i++)
        {
            UICursor cursor = GetChild<UICursor>(i);
            if(cursor != null)
            {
                cursors[i] = cursor;
            }
        }
    }

    public override void _Process(float delta)
    {
        for(int i = 0; i < cursors.Length; i++)
        {
            Vector2 move_to = Vector2.Zero;
            bool unclick = false;
            bool click = false;
            if(Input.IsActionPressed("Player_Down_" + index_string[i]))
            {
                move_to += Vector2.Down;
            }
            if(Input.IsActionPressed("Player_Up_" + index_string[i]))
            {
                move_to += Vector2.Up;
            }
            if(Input.IsActionPressed("Player_Left_" + index_string[i]))
            {
                move_to += Vector2.Left;
            }
            if(Input.IsActionPressed("Player_Right_" + index_string[i]))
            {
                move_to += Vector2.Right;
            }
            if(Input.IsActionJustPressed("Player_Attack_" + index_string[i]))
            {
                click = true;
            }
            if(Input.IsActionJustReleased("Player_Attack_" + index_string[i]))
            {
                unclick = true;
            }
            if(Input.IsActionJustPressed("Player_Special_" + index_string[i]))
            {
                GoToPreviousMenu();
            }
            cursors[i].MoveCursor(move_to * delta * cursors[i].cursor_move_speed);
            if(click)cursors[i].Click();
            if(unclick)cursors[i].StopClick();
        }
    }

    private void GoToPreviousMenu()
    {
        if(previous_menu_file != null)
        {
            previous_menu = GD.Load<PackedScene>(previous_menu_file);
            Node menu = previous_menu.Instance<Node>();
            Node world = GetNode<Node>(world_node);
            Node root = world.GetParent();
            world.QueueFree();
            root.AddChild(menu);
        }
    }

    public UICursor[] GetUICursors()
    {
        return cursors;
    }
}
