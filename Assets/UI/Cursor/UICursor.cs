using Godot;
using System;

public class UICursor : Node2D
{
    public enum PLAYER_CURSOR
    {
        P1,
        P2,
        P3,
        P4
    }
    [Export]
    public Vector2 spawn_location;

    [Export]
    public Vector2 select_point;
    
    [Export]
    public PLAYER_CURSOR player;
    
    private bool has_clicked;

    public override void _Ready()
    {
        SpawnCursor();
        has_clicked = false;
    }
    public override void _Process(float delta)
    {
        
    }
    public void MakeCursorVisible(bool is_visible)
    {
        Visible = is_visible;
    }
    public void SpawnCursor()
    {
        Position = spawn_location;
    }
    public void MoveCursor(Vector2 move_to)
    {
        Position += move_to;
    }
    public void Click()
    {
        has_clicked = true;
    }
    public void StopClick()
    {
        has_clicked = false;
    }
    public bool GetClickState()
    {
        return has_clicked;
    }
}
