using Godot;
using System;
using System.Data.SqlTypes;

public class RotateAndDrop : Sprite
{
    // Called when the node enters the scene tree for the first time.
    [Export]
    private string trophy_physics_drop;
    [Export]
    private string display_coins_path;
    private CoinsLabelDisplay display_coins;
    [Export]
    private string display_trophiesunlocked_display_path;
    private TrophiesUnlockedLabelDisplay display_tropghiesunlocked;
    [Export]
    private string display_sprite_path;
    private Sprite display_sprite; 
    [Export]
    private string display_name_path;
    private Label display_name;
    private Node world;
    private PackedScene trophy_node;
    private Transform2D default_transform;
    private bool rotating;
    private float amount_rotated = 0.0f;
    private TrophyData drop_trophy;
    public override void _Ready()
    {
        display_tropghiesunlocked = GetNode<TrophiesUnlockedLabelDisplay>("../"+display_trophiesunlocked_display_path);
        display_coins = GetNode<CoinsLabelDisplay>("../"+display_coins_path);
        display_name = GetNode<Label>("../"+display_name_path);
        display_sprite = GetNode<Sprite>("../"+display_sprite_path);
        default_transform = Transform;
        rotating = false;
        trophy_node = GD.Load<PackedScene>(trophy_physics_drop);
        world = GetNode("../");
    }
    public override void _Process(float delta)
    {
        if(rotating)
        {
            amount_rotated += Mathf.Deg2Rad(360)*delta;
            Transform = Transform.Rotated(Mathf.Deg2Rad(360)*delta);
            display_tropghiesunlocked.RefreshDisplay();
            display_coins.RefreshDisplay();
            if(amount_rotated > 2*Mathf.Pi)
            {
                Transform = default_transform;
                rotating = false;
                Node trophy_instance = trophy_node.Instance<Node>();
                var type = trophy_instance.GetType();
                ((DieOnTime)trophy_instance).Translate(Transform.origin-new Vector2(0,-50));                
                trophy_instance.GetNode<Sprite>("Sprite").Texture = drop_trophy.sprite;
                display_sprite.Texture = drop_trophy.sprite; 
                display_name.Text = drop_trophy.name + "!!!";
                world.AddChild(trophy_instance);
            }
        }
        else
        {
            if(Input.IsActionJustPressed("ui_cancel"))
            {
                GetParent<Node2D>().QueueFree();
                Node2D instance = GD.Load<PackedScene>("res://Assets/UI/CharacterSelectScreen/CharacterSelectScene.tscn").Instance<Node2D>();

                GetNode<Node>("../../").AddChild(instance);
            }
            if(Input.IsActionJustPressed("Player_Jump"))
            {
                var trophy_system = GetNode<TrophySystem>("/root/TrophySystem");
                if(trophy_system.GetCoins() > 0)
                {
                    drop_trophy = trophy_system.GetRandomNewTrophy();
                    if(drop_trophy == null)
                    {
                        display_name.Text = "You've already unlocked everything!";
                    }
                    else
                    {
                        amount_rotated = 0;
                        trophy_system.SubtractCoins(1);
                        rotating = true;
                        display_name.Text = "You just got???";
                    }
                }
            }            
        }
    }
}
