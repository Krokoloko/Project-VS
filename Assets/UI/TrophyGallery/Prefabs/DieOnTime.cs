using Godot;
using System;

public class DieOnTime : Node2D
{
    [Export]
    private float time = 0.0f;
    private float accumalator = 0.0f;

    public override void _Process(float delta)
    {
        accumalator += delta;
        if(accumalator >= time)
        {
            QueueFree();
        }
    }
}
