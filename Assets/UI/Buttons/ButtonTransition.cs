using Godot;
using System;

public class ButtonTransition : Control
{
    [Signal]
    public delegate void OnHover();

    [Signal]
    public delegate void HoverExited();

    [Export]
    private string scene_file;
    private PackedScene scene;

    [Export]
    private NodePath scene_root = "";

    [Export]
    private NodePath cursor_manager = "";
    private CursorManager cursors_manager_node;
    private UICursor[] cursors;

    private uint cursors_hovered = 0;

    private bool transitioning = false;

    public override void _Ready()
    {
        this.Connect("pressed", this, "TransitionToScene");
        cursors_manager_node = GetNodeOrNull<CursorManager>(cursor_manager);
        if(cursors_manager_node != null)
        {
            cursors = cursors_manager_node.GetUICursors();
        }
        GetParent().GetChildOrNull<Area2D>(0)?.Connect("area_entered", this, "IsCursorHoverring");
        GetParent().GetChildOrNull<Area2D>(0)?.Connect("area_exited", this, "RemoveCursorHovered");
    }

    private void IsCursorHoverring(Area2D body)
    {
        UICursor cursor = body.GetParentOrNull<UICursor>();
        if(cursor != null)
        {
            uint shift = (uint)(((int)(cursor.player)+1)>>0);
            cursors_hovered |= shift;
        }
    }

    private void RemoveCursorHovered(Area2D body)
    {
        UICursor cursor = body.GetParentOrNull<UICursor>();
        if(cursor != null)
        {
            uint shift = ~(uint)(((int)(cursor.player)+1)>>0);
            cursors_hovered &= shift;
        }
    }

    public override void _Process(float dt)
    {
        if(cursors != null)
        {
            for(int i = 0; i < cursors.Length; i++)
            {
                if(cursors[i].GetClickState() == CURSOR_CLICK_STATE.START && !transitioning) 
                {
                    if((cursors_hovered & (uint)(((int)(cursors[i].player)+1)>>0)) > 0)
                    {
                        TransitionToScene();
                        transitioning = true;
                    }
                }
            }
        }
    }

    public void TransitionToScene()
    {
        scene = GD.Load<PackedScene>(scene_file);
        Node scene_instance = scene.Instance<Node>();
        Node scene_node = GetNode<Node>(scene_root);
        Node root_node = scene_node.GetNode<Node>("../");

        scene_node.QueueFree();

        root_node.AddChild(scene_instance);
    }
}
