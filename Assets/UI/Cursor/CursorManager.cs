using Godot;
using System;
using System.Linq;

public class CursorManager : Node
{
    private UICursor[] cursors;
    public override void _Ready()
    {
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
        Vector2 move_to = Vector2.Zero;
        bool unclick = false;
        bool click = false;
        if(Input.IsActionPressed("Player_Down"))
        {
            move_to += Vector2.Down;
        }
        if(Input.IsActionPressed("Player_Up"))
        {
            move_to += Vector2.Up;
        }
        if(Input.IsActionPressed("Player_Left"))
        {
            move_to += Vector2.Left;
        }
        if(Input.IsActionPressed("Player_Right"))
        {
            move_to += Vector2.Right;
        }
        if(Input.IsActionJustPressed("Player_Jump"))
        {
            click = true;
        }
        if(Input.IsActionJustReleased("Player_Jump"))
        {
            unclick = true;
        }
        for(int i = 0; i < cursors.Length; i++)
        {
            cursors[i].MoveCursor(move_to * delta);
            if(click)cursors[i].Click();
            if(unclick)cursors[i].StopClick();
        }
    }

    public UICursor[] GetUICursors()
    {
        return cursors;
    }
}
