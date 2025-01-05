using Godot;
using System;

public class ToggleControlsGraphic : TextureRect
{

    const string KEYBOARD_MODE = "keyboard";
    const string CONTROLLER_MODE = "controller";
    const string NONE_MODE = "none";

    [Export]
    private Texture controller_graphic;

    [Export]
    private Texture keyboard_graphic;
    
    private string toggleType = "none";
    public override void _Ready()
    {
        if(keyboard_graphic != null)
        {
            toggleType = KEYBOARD_MODE;
        }
        if(controller_graphic != null)
        {
            toggleType = CONTROLLER_MODE;
        }
        SetTexture(toggleType);
    }
    public override void _Process(float delta)
    {
        if(controller_graphic != null && keyboard_graphic != null)
        {
            if(Input.IsActionJustReleased("Toggle_Controls_UI"))
            {
                switch(toggleType)
                {
                    case KEYBOARD_MODE:
                        toggleType = NONE_MODE;
                        break;

                    case CONTROLLER_MODE:
                        toggleType = KEYBOARD_MODE;
                        break;

                    case "none":
                        toggleType = CONTROLLER_MODE;
                        break;

                    default:
                        break;
                }
                SetTexture(toggleType);
            }
        }        
    }
    private void SetTexture(string mode)
    {
        switch(mode)
        {
            case KEYBOARD_MODE:
                Texture = keyboard_graphic;
                break;

            case CONTROLLER_MODE:
                Texture = controller_graphic;
                break;

            default:
                Texture = null;
                break;
        }
    }
}