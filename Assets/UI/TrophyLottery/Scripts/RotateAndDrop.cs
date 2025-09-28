using Godot;
using System;
using System.Data.SqlTypes;

public class RotateAndDrop : Sprite
{
    // Called when the node enters the scene tree for the first time.
    [Export]
    private string trophy_node_file;
    private PackedScene trophy_node;

    [Export]
    private string back_menu_file;
    private PackedScene back_menu;
    [Export]
    private NodePath display_coins_path;
    private CoinsLabelDisplay display_coins;
    [Export]
    private NodePath display_trophiesunlocked_display_path;
    private TrophiesUnlockedLabelDisplay display_tropghiesunlocked;
    [Export]
    private NodePath display_sprite_path;
    private Sprite display_sprite; 
    [Export]
    private NodePath display_name_path;
    private Label display_name;
    private Node world;
    private Transform2D default_transform;
    private bool rotating;
    private float amount_rotated = 0.0f;
    private TrophyData drop_trophy;
    private MusicAndSoundManager audio_manager;
    public override void _Ready()
    {
        audio_manager = GetNode<MusicAndSoundManager>("/root/AudioManager");
        display_tropghiesunlocked = GetNode<TrophiesUnlockedLabelDisplay>(display_trophiesunlocked_display_path);
        display_coins = GetNode<CoinsLabelDisplay>(display_coins_path);
        display_name = GetNode<Label>(display_name_path);
        display_sprite = GetNode<Sprite>(display_sprite_path);
        trophy_node = GD.Load<PackedScene>(trophy_node_file);
        default_transform = Transform;
        rotating = false;
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
                float trophy_scale = drop_trophy.scale/4;
                Transform = default_transform;
                rotating = false;
                Node trophy_instance = trophy_node.Instance<Node>();
                var type = trophy_instance.GetType();
                ((DieOnTime)trophy_instance).Translate(Transform.origin-new Vector2(0,-50));                
                trophy_instance.GetNode<Sprite>("Sprite").Texture = drop_trophy.sprite;
                trophy_instance.GetNode<Sprite>("Sprite").Scale = new Vector2(trophy_scale, trophy_scale);
                display_sprite.Texture = drop_trophy.sprite;
                display_sprite.Scale = new Vector2(trophy_scale*2, trophy_scale*2);
                display_name.Text = drop_trophy.name + "!!!";
                world.AddChild(trophy_instance);
            }
        }
        else
        {
            if(Input.IsActionJustPressed("ui_cancel"))
            {
                GetParent<Node2D>().QueueFree();
                back_menu = GD.Load<PackedScene>(back_menu_file);
                Control instance = back_menu.Instance<Control>();
                //audio_manager.PlaySFX();
                GetNode<Node>("../../").AddChild(instance);
            }
            if(Input.IsActionJustPressed("Player_Jump_1"))
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
                        //audio_manager.PlaySFX();
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
