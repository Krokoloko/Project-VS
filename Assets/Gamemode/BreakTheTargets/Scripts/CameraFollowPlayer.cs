using Godot;
using System;

public class CameraFollowPlayer : Spatial
{
    private Vector3 start;
    
    [Export]
    public Vector3 camera_offset;

    [Export]
    private NodePath player_root_node;
    private Player player;
    public override void _Ready()
    {
        start = GlobalTranslation;
        
        Node root = GetNode<Node>(player_root_node);
        for (int i = 0; i < root.GetChildCount(); i++)
        {
            if(root.GetChild(i).Name == "Player")
            {
                player = root.GetChild<Player>(i);
            }
        }
    }

    public override void _Process(float delta)
    {
        Translation = player.GlobalTranslation + camera_offset;
    }
}
