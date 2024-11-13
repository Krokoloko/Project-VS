using Godot;
using System;

public class ButtonTransition : Node
{
    [Signal]
    public delegate void OnHover();

    [Signal]
    public delegate void HoverExited();

    [Export]
    private string scene_file = "";

    [Export]
    private NodePath scene_root = "";

    [Export]
    private NodePath cursor_manager = "";

    [Export]
    public NodePath cursors_node = "";

    public override void _Ready()
    {
        this.Connect("pressed", this, "TransitionToScene");
    }

    public void TransitionToScene()
    {
        Node scene_instance = GD.Load<PackedScene>(scene_file).Instance<Node>();
        Node scene_node = GetNode<Node>(scene_root);
        Node root_node = scene_node.GetNode<Node>("../");
        scene_node.QueueFree();
        root_node.AddChild(scene_instance);
    }
}
