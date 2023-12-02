using Godot;
using System;
using System.Linq;

public class CursorManager : Node
{
    private UICursor[] cursors;
    public override void _Ready()
    {
        
    }

    public override void _Process(float delta)
    {
        
    }

    public UICursor[] GetUICursors()
    {
        return cursors;
    }
}
