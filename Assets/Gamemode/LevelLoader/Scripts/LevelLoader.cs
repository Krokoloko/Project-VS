using Godot;
using System;

public class LevelLoader : Node
{
    public void LoadLevel(LevelResource level, CharacterResources[] character)
    {
        GetTree().Root.GetNode<Node>("World").QueueFree();
        GetTree().Root.AddChild(GD.Load<PackedScene>(level.level_source).Instance());
        GetTree().Root.AddChild(GD.Load<PackedScene>(character[0].prefab).Instance());
    }

    public override void _Process(float delta)
    {
        
    }
}
