using Godot;
using System;
using System.Collections.Generic;

public class ButtonRestartLevel : Node
{
    [Export]
    private LevelResource level = null;

    [Export]
    private NodePath players_nodepath = "";

    [Export]
    private NodePath scene_root = "";

    public override void _Ready()
    {
        this.Connect("pressed", this, "ReplayLevel");
    }

    public void ReplayLevel()
    {
        Node scene_instance = GD.Load<PackedScene>(level.level_source).Instance<Node>();
        Node scene_node = GetNode<Node>(scene_root);
        Node root_node = scene_node.GetNode<Node>("../");
        Node player_container = GetNode<Node>(players_nodepath);

        Node instance_container = scene_instance.GetNode<Node>("./PlayerContainer");
        for(int i = 0; i < player_container.GetChildCount(); i++)
        {
            Player current_player = player_container.GetChild<Player>(i);
            Player player = GD.Load<PackedScene>(current_player.resource_reference.prefab).Instance<Player>();
            player.SetPlayerID(current_player.GetPlayerID());
            player.SetStockCount(3);
            player.SetPlayerName(current_player.GetPlayerName());
            player.Translation = new Vector3(level.player_spawn_positions[i].x, level.player_spawn_positions[i].y, 0.0f);
            instance_container.AddChild(player);
        }

        scene_node.QueueFree();
        root_node.AddChild(scene_instance);
    }
}
