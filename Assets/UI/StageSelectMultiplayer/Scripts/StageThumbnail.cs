using Godot;
using System;

public class StageThumbnail : Control
{
    [Signal]
    public delegate void OnHover(LevelResource resource); 

    [Signal]
    public delegate void UnHover(LevelResource resource);

    [Export]
    private NodePath clickDetector;

    [Export]
    private NodePath thumbnailNode;
    private TextureRect thumbnail;
    private Area2D detector;

    public LevelResource resource;

    public override void _Ready()
    {
        detector = GetNode<Area2D>(clickDetector);

        thumbnail = GetNode<TextureRect>(thumbnailNode);
        thumbnail.Texture = resource.thumbnail;

        detector.Connect("area_entered", this, "ActivateHover");
        detector.Connect("area_exited", this, "ActivateUnHover");
    }

    private void ActivateHover(Area2D area)
    {
        EmitSignal("OnHover", resource);
    }

    private void ActivateUnHover(Area2D area)
    {
        EmitSignal("UnHover", resource);
    }
}
