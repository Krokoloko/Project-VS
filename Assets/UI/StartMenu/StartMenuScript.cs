using Godot;
using System;

public class StartMenuScript : Label
{
    [Export]
    private NodePath world;

    [Export]
    private NodePath animated_sprites_path;
    private Node2D animated_sprites;

    [Export]
    public float interval = 1.0f;
    private float timer = 0.0f;
    private Label this_label;
    private Color color;
    
    [Export]
    private Color alt_color;

    [Export]
    private string scene_file;
    private PackedScene scene;

    private bool visible;

    private string text;

    private const string easter_text = "Please Don't Sue Us";

    public override void _Ready()
    {
        this_label = GetNode<Label>(this.GetPath());
        animated_sprites = GetNode<Node2D>(animated_sprites_path);
        for(int i = 0; i < animated_sprites.GetChildCount(); i++)
        {
            animated_sprites.GetChild<Sprite>(i).GetChild<AnimationPlayer>(0).Play("Main Menu Loop");
        }
        text = this_label.Text;
        visible = true;
        color = this_label.GetColor("font_color");
    }

    public override void _Input(InputEvent vent)
    {
        if(vent.IsPressed())
        {
			
        }
    }

    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("ui_accept"))
        {
            GetNode<Node>(world).QueueFree();
            scene = GD.Load<PackedScene>(scene_file);
            Control instance = scene.Instance<Control>();
            Node root = GetNode<Node>("../../"); 
            root.AddChild(instance);
        }
        timer += delta;
        if(timer >= interval)
        {
            visible = !visible;
            this_label.Text = (visible) ? text : easter_text; 
            this_label.RemoveColorOverride("font_color");
            this_label.AddColorOverride("font_color", (visible) ? color : alt_color);
            timer -= interval;
        }
    }
}
