using Godot;
using System;
using System.Collections.Generic;

public class TrophyGallery : Node
{
    [Export]
    private float scrollSpeed;
    private float accumalator;

    private const float easterEggTimer = 1.5f;
    private bool easterEggActivated = false;

    [Export]
    private NodePath sfxNode;
    private AudioStreamPlayer sfx;

    [Export]
    private NodePath musicNode;
    private AudioStreamPlayer musicPlayer;

    [Export]
    private Texture notUnlockedSprite = null;
    private int index;

    [Export]
    private NodePath descriptionLabelNode;
    private Label description;

    [Export]
    private PackedScene goBackScene;

    [Export]
    private NodePath highLightNode;
    private TextureRect highlightTexture = null;
    private Label highlightLabel = null;

    [Export]
    private NodePath previousNode;
    private TextureRect previousTexture;
    private Label previousLabel;
    
    [Export]
    private NodePath nextNode;
    private TextureRect nextTexture;
    private Label nextLabel;

    [Export]
    private NodePath trophyNumberNode;
    private Label trophyNumberLabel;

    private Dictionary<int, TrophyData> trophies;
    private int totalObtainableTrophies;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var trophySystem = GetNode<TrophySystem>("/root/TrophySystem");    
        trophies = trophySystem.GetAllUnlockedTrophies();
        totalObtainableTrophies = trophySystem.GetTotalUnlockableTrophies();

        sfx = GetNode<AudioStreamPlayer>(sfxNode);
        description = GetNode<Label>(descriptionLabelNode);

        musicPlayer = GetNode<AudioStreamPlayer>(musicNode);
        musicPlayer.PauseMode = PauseModeEnum.Stop;

        var highlight = GetNode<Control>(highLightNode);
        highlightTexture = highlight.GetChild<Control>(0).GetChild<TextureRect>(0);
        highlightLabel = highlight.GetChild<Control>(1).GetChild<Label>(0);

        var previous = GetNode<Control>(previousNode);
        previousTexture = previous.GetChild<Control>(0).GetChild<TextureRect>(0);
        previousLabel = previous.GetChild<Control>(1).GetChild<Label>(0);

        var next = GetNode<Control>(nextNode);
        nextTexture = next.GetChild<Control>(0).GetChild<TextureRect>(0);
        nextLabel = next.GetChild<Control>(1).GetChild<Label>(0);

        trophyNumberLabel = GetNode<Label>(trophyNumberNode);
        trophyNumberLabel.Text = "#1";

        index = 0;
        accumalator = 0;

        UpdateUI();
    }



    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        bool go_left = Input.IsActionPressed("Player_Left_1");
        bool go_right = Input.IsActionPressed("Player_Right_1");    
        
        if(go_left || go_right)
        {
            accumalator += delta;
        }else
        {
            accumalator = 0;
        }

        if(accumalator >= scrollSpeed)
        {
            accumalator -= scrollSpeed;
            int move = (go_right)?1:0;
            move -= (go_left)?1:0;

            index += move;
            if(index >= totalObtainableTrophies)
            {
                index = 0;
            }
            if(index < 0)
            {
                index = totalObtainableTrophies-1;
            }
            UpdateUI();
        }
        
        if(Input.IsActionJustPressed("Player_Attack_1"))
        {
            if(trophies.ContainsKey(index))
            {
                if(!easterEggActivated)
                {
                    if(trophies[index].easter_egg_sprite != null)
                    {
                        easterEggActivated = true;
                        highlightTexture.Texture = trophies[index].easter_egg_sprite;
                    }
                    if(trophies[index].easter_egg_sound != null)
                    {
                        easterEggActivated = true;
                        sfx.Stream = trophies[index].easter_egg_sound;
                        sfx.Play();

                        musicPlayer.StreamPaused = true;   
                    }
                }else
                {
                    if(trophies[index].easter_egg_sprite != null)
                    {
                        easterEggActivated = false;
                        highlightTexture.Texture = trophies[index].sprite;
                    }
                    if(trophies[index].easter_egg_sound != null)
                    {
                        easterEggActivated = false;
                        sfx.Stream = trophies[index].easter_egg_sound;
                        sfx.Stop();
                        musicPlayer.StreamPaused = false;
                    }
                }
            }
        }
        if(Input.IsActionJustPressed("ui_cancel"))
        {
            this.GetParent().AddChild(goBackScene.Instance<Node>());
            QueueFree();            
        }
    }
    private void UpdateUI()
    {
        int previous_trophy = ((index-1)<0)?totalObtainableTrophies-1:index-1;
        int next_trophy = (index+1)%totalObtainableTrophies;
        trophyNumberLabel.Text = "#" + (index+1).ToString();
        
        if(sfx.Playing)
        {
            sfx.Stop();
            musicPlayer.StreamPaused = false;
        }
        easterEggActivated = false;

        if(trophies.ContainsKey(previous_trophy))
        {
            previousTexture.Texture = trophies[previous_trophy].sprite;
            previousLabel.Text = trophies[previous_trophy].name;
        }else
        {   
            previousTexture.Texture = notUnlockedSprite;
            previousLabel.Text = "???"; 
        }
        if(trophies.ContainsKey(index))
        {
            highlightTexture.Texture = trophies[index].sprite;
            highlightLabel.Text = trophies[index].name;
            description.Text = trophies[index].description;
        }else
        {   
            highlightTexture.Texture = notUnlockedSprite;
            highlightLabel.Text = "???"; 
            description.Text = "";
        }
        if(trophies.ContainsKey(next_trophy))
        {
            nextTexture.Texture = trophies[next_trophy].sprite;
            nextLabel.Text = trophies[next_trophy].name;
        }else
        {   
            nextTexture.Texture = notUnlockedSprite;
            nextLabel.Text = "???"; 
        }
    }
}
