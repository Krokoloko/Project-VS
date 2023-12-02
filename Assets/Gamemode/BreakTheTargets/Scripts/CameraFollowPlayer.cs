using Godot;
using System;

public class CameraFollowPlayer : Spatial
{
    private Vector3 start;
    
    [Export]
    public Vector3 camera_offset;

    [Export]
    private NodePath player_node;
    private Player player;
    public override void _Ready()
    {
        start = GlobalTranslation;
        player = GetNode<Player>(player_node);
    }

    public override void _Process(float delta)
    {
        Translation = player.GlobalTranslation + camera_offset;
    }
}
