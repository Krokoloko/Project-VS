using Godot;
using System;

public enum PLAYER_CURSOR
{
    NONE = -1,
    P1 = 0,
    P2 = 1,
    P3 = 2,
    P4 = 3,
    COUNT = 4
}
public enum CURSOR_CLICK_STATE
{
    NONE,
    START,
    HOLD,
    EXIT
}
public class UICursor : Node2D
{
    [Export]
    private string player_icon_folder;

    private Texture player_icon;

    [Export]
    private NodePath player_number_icon_node;

    [Export]
    public Vector2 spawn_location;

    [Export]
    public Vector2 select_point;
    
    [Export]
    public PLAYER_CURSOR player;
    
    [Export]
    public float cursor_move_speed;

    private CURSOR_CLICK_STATE click_state;
    private bool revert_click_bool;
    private Vector2 velocity;
    public override void _Ready()
    {
        SpawnCursor();
        revert_click_bool = false;
        click_state = CURSOR_CLICK_STATE.NONE;
        Directory directory = new Directory();
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
        GetNode<Sprite>(player_number_icon_node).Texture = player_icon;
    }
    public override void _Process(float delta)
    {
        Position += velocity * delta;
        if(click_state == CURSOR_CLICK_STATE.EXIT)
        {
            click_state = CURSOR_CLICK_STATE.NONE;
        }
        if(revert_click_bool)
        {
            if(click_state == CURSOR_CLICK_STATE.START)
            {
                revert_click_bool = false;
                click_state = CURSOR_CLICK_STATE.HOLD;
            }
        }
        
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
        velocity = move_to * cursor_move_speed;
    }
    public void Click()
    {
        revert_click_bool = false;
        click_state = CURSOR_CLICK_STATE.START;
    }
    public void StopClick()
    {
        click_state = CURSOR_CLICK_STATE.EXIT;
    }
    public CURSOR_CLICK_STATE GetClickState()
    {
        revert_click_bool = true;
        return click_state;
    }
}
