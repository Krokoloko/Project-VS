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
    private Texture notUnlockedSprite = null;
    private int index;

    [Export]
    private NodePath descriptionLabelNode;
    private Label description;

    [Export]
    private string goBackSceneFile;
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

    private List<KeyValuePair<int, TrophyData>> trophies;
    private MusicAndSoundManager audio_manager;
    private int totalObtainableTrophies;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        audio_manager = GetNode<MusicAndSoundManager>("/root/AudioManager");
        audio_manager.PlayBGM("TrophyGallery");
        goBackScene = GD.Load<PackedScene>(goBackSceneFile);
        
        var trophySystem = GetNode<TrophySystem>("/root/TrophySystem");    
        trophies = trophySystem.GetAllUnlockedTrophies();
        totalObtainableTrophies = trophySystem.GetTotalUnlockableTrophies();

        sfx = GetNode<AudioStreamPlayer>(sfxNode);
        description = GetNode<Label>(descriptionLabelNode);

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
            audio_manager.PlaySFX(MusicAndSoundManager.SFX.SCROLL_BUTTON);
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
            if(trophies[index].Value != null)
            {
                if(!easterEggActivated)
                {
                    if(trophies[index].Value.easter_egg_sprite != null)
                    {
                        easterEggActivated = true;
                        highlightTexture.Texture = trophies[index].Value.easter_egg_sprite;
                    }
                    if (trophies[index].Value.easter_egg_sound != null)
                    {
                        easterEggActivated = true;                    
                        audio_manager.PlayBGM_Custom(trophies[index].Value.easter_egg_sound, -15, trophies[index].Value.easter_egg_sound.ResourceName);
                    }
                }else
                {
                    if(trophies[index].Value.easter_egg_sprite != null)
                    {
                        easterEggActivated = false;
                        highlightTexture.Texture = trophies[index].Value.sprite;
                    }
                    if(trophies[index].Value.easter_egg_sound != null)
                    {
                        easterEggActivated = false;
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
        trophyNumberLabel.Text = "#" + (trophies[index].Value.ID).ToString();

        if (audio_manager.Currently_playing != "TrophyGallery")
        {
            audio_manager.PlayBGM("TrophyGallery");
        }
        easterEggActivated = false;

        if(trophies[previous_trophy].Value.sprite != null)
        {
            previousTexture.Texture = trophies[previous_trophy].Value.sprite;
            previousLabel.Text = trophies[previous_trophy].Value.name;
        }else
        {   
            previousTexture.Texture = notUnlockedSprite;
            previousLabel.Text = "???"; 
        }
        if(trophies[index].Value.sprite != null)
        {
            highlightTexture.Texture = trophies[index].Value.sprite;
            highlightLabel.Text = trophies[index].Value.name;
            description.Text = trophies[index].Value.description;
        }else
        {   
            highlightTexture.Texture = notUnlockedSprite;
            highlightLabel.Text = "???"; 
            description.Text = "";
            trophyNumberLabel.Text = "#???";
        }
        if(trophies[next_trophy].Value.sprite != null)
        {
            nextTexture.Texture = trophies[next_trophy].Value.sprite;
            nextLabel.Text = trophies[next_trophy].Value.name;
        }else
        {   
            nextTexture.Texture = notUnlockedSprite;
            nextLabel.Text = "???"; 
        }
    }
}
