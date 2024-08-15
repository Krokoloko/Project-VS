using Godot;
using System;

public class ButtonTransition : Button
{
    [Export]
    private string scene_file = "";

    [Export]
    private NodePath scene_root = "";

    public override void _Ready()
    {
        this.Connect("pressed", this, "TransitionToScene");
    }

    public void TransitionToScene()
    {
        Node2D scene_instance = GD.Load<PackedScene>(scene_file).Instance<Node2D>();
        Node scene_node = GetNode<Node>(scene_root);
        Node root_node = scene_node.GetNode<Node>("../");
        scene_node.QueueFree();
        root_node.AddChild(scene_instance);
    }
}
