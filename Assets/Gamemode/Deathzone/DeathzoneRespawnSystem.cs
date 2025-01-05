using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DeathzoneRespawnSystem : Node
{
    [Export]
    public NodePath[] spawn_nodes;
    
    [Export]
    public NodePath[] deathzone_nodes;
    
    private Spatial[] spawn_locations;

    private Area[] deathzones;
    public override void _Ready()
    {
        deathzones = new Area[deathzone_nodes.Count()];
        spawn_locations = new Spatial[spawn_nodes.Count()];
        for(int i = 0; i < deathzone_nodes.Count(); i++)
        {
            var area = GetNodeOrNull<Area>(deathzone_nodes[i]);
            if(area != null)
            {
                if(i == 0)
                {
                    area.Connect("body_entered", this, "OnDetectTopblastzone");
                }else
                {
                    area.Connect("body_entered", this, "OnDetect");
                }
                deathzones[i] = area;
            }
        }
        for(int i = 0; i < spawn_nodes.Count(); i++)
        {
            var spawn = GetNodeOrNull<Spatial>(spawn_nodes[i]);
            if(spawn != null)
            {
                spawn_locations[i] = spawn;
            }
        }
    }

    private void OnDetect(Node body)
    {
        var player = GetNodeOrNull<Player>(body.GetPath());
        if(player != null)
        {
            player.SubtractStock();
            if(player.GetStockCount() > 0)
            {
                Spatial spawn = spawn_locations[(int)player.GetPlayerID()];
                Vector2 spawn_location = new Vector2(spawn.Translation.x, spawn.Translation.y);
                player.RespawnToLocation(spawn_location);
            }
        }
    }

    private void OnDetectTopblastzone(Node body)
    {
        var player = GetNodeOrNull<Player>(body.GetPath());
        if(player != null)
        {
            if(player.IsLaunchState())
            {
                player.SubtractStock();
                if(player.GetStockCount() > 0)
                {
                    Spatial spawn = spawn_locations[(int)player.GetPlayerID()];
                    Vector2 spawn_location = new Vector2(spawn.Translation.x, spawn.Translation.y);
                    player.RespawnToLocation(spawn_location);
                }
            }
        }
    }
}
