using Godot;
using System;

public class MainMenuScript : Node
{
    
    [Export]
    private NodePath animations;
    private MusicAndSoundManager sound_manager;

    public override void _Ready()
    {
        sound_manager = GetNode<MusicAndSoundManager>("/root/AudioManager");
        sound_manager.PlayBGM("MenuTheme");
        var animations_node = GetNode<Node>(animations);  
        for(int i = 0; i < animations_node.GetChildCount(); i++)
        {
            animations_node.GetChild<Sprite>(i).GetChild<AnimationPlayer>(0).Play("Main Menu Loop");
        }
    }

    public override void _Process(float delta)
    {
        
    }
}
