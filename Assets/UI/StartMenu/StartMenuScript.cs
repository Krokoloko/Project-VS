using Godot;
using System;

public class StartMenuScript : Label
{
    [Export]
    public float interval = 1.0f;
    private float timer = 0.0f;
    private Label this_label;
    private Color color;
    [Export]
    private Color alt_color;
    private bool visible;

    private string text;

    private const string easter_text = "Please Don't Sue Us";

    public override void _Ready()
    {
        this_label = GetNode<Label>(this.GetPath());
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
        if(Input.IsActionPressed("ui_accept"))
        {
            GetParent<Node2D>().QueueFree();
            Node2D instance = GD.Load<PackedScene>("res://Assets/UI/CharacterSelectScreen/CharacterSelectScene.tscn").Instance<Node2D>();

            GetNode<Node>("../../").AddChild(instance);
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
