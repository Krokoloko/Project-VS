using Godot;
using System;
using System.Collections.Generic;

public class MusicAndSoundManager : Node
{
    public struct BGM_Props
    {
        public String source;
        public float volume;
    }

    private Dictionary<String, BGM_Props> BGM_LIST; 

    const string CONFIRM_SFX_PATH = "res://Assets/SFX/UI/SND_SE_SYSTEM_FIXED_L.wav";
    private AudioStream confirm_sfx;
    const string SELECT_SFX_PATH = "res://Assets/SFX/UI/snd_se_system_fixed.wav";
    private AudioStream select_sfx;
    const string DENY_SFX_PATH = "res://Assets/SFX/UI/snd_se_system_cancel.wav";
    private AudioStream deny_sfx;
    const string HOVER_SFX_PATH = "res://Assets/SFX/UI/snd_se_system_cursor.wav";
    private AudioStream hover_sfx;
    const string SCROLL_BUTTON_SFX_PATH = "res://Assets/SFX/UI/snd_se_system_Page_Change.wav";
    private AudioStream scroll_sfx;
    public enum SFX
    {
        CONFIRM,
        SELECT,
        DENY,
        HOVER,
        SCROLL_BUTTON,

    }
    private AudioStreamPlayer ui_sfx;
    private bool can_interupt;
    private AudioStreamPlayer bgm;
    public string Currently_playing {get{ return currently_playing; }}
    private string currently_playing;
    public override void _Ready()
    {
        can_interupt = true;

        confirm_sfx = GD.Load<AudioStream>(CONFIRM_SFX_PATH);
        deny_sfx = GD.Load<AudioStream>(DENY_SFX_PATH);
        hover_sfx = GD.Load<AudioStream>(HOVER_SFX_PATH);
        select_sfx = GD.Load<AudioStream>(SELECT_SFX_PATH);
        scroll_sfx = GD.Load<AudioStream>(SCROLL_BUTTON_SFX_PATH);

        BGM_LIST = new Dictionary<string, BGM_Props>();

        BGM_LIST.Add("MenuTheme", new BGM_Props { source = "res://Assets/UI/MainMenu/[Music] Kirby Super Star - Game Select [aStpcM86jpc].mp3", volume = -20.0f });
        BGM_LIST.Add("CharacterSelect", new BGM_Props { source = "res://Assets/UI/CharacterSelectScreen/Kirby 64： The Crystal Shards - Enemy Card Index Theme.mp3", volume = -28.0f });
        //BGM_LIST.Add("TrophyLottery", new BGM_Props { source = "", volume = -25 });
        BGM_LIST.Add("TrophyGallery", new BGM_Props { source = "res://Assets/UI/TrophyGallery/Music/Trophy Gallery (Brawl) - Super Smash Bros. Wii U.mp3", volume = -25 });
        BGM_LIST.Add("TargetsTest", new BGM_Props { source = "res://Assets/Gamemode/BreakTheTargets/Sounds/TargetsTestSong.mp3", volume = -20 });
        BGM_LIST.Add("CrescentMoon", new BGM_Props { source = "res://Assets/Gamemode/Multiplayer/Maps/Songs/CrescentMoon.mp3", volume = -24.0f });
        BGM_LIST.Add("ForestArea", new BGM_Props { source = "res://Assets/Gamemode/Multiplayer/Maps/Songs/Forest Area.mp3", volume = -27.0f });
        BGM_LIST.Add("DistantSpring", new BGM_Props { source = "res://Assets/Gamemode/Multiplayer/Maps/Songs/DistantSpring.mp3", volume = -17.0f });
        BGM_LIST.Add("CinnabarIsland", new BGM_Props { source = "res://Assets/Gamemode/Multiplayer/Maps/Songs/Cinnabar Island.mp3", volume = -37.23f });

        ui_sfx = new AudioStreamPlayer();
        ui_sfx.VolumeDb = -28.8f;
        ui_sfx.Connect("finished", this, "UntoggleInterupting");
        AddChild(ui_sfx, true);

        bgm = new AudioStreamPlayer();
        bgm.VolumeDb = -28.8f;
        AddChild(bgm, true);
    }

    private void UntoggleInterupting()
    {
        can_interupt = true;
    }

    public void StopBGM()
    {
        currently_playing = "none";
        bgm.Stop();
    }

    public void PlayBGM(String BGM_Name)
    {
        var bgm_song = GD.Load<AudioStream>(BGM_LIST[BGM_Name].source);
        currently_playing = BGM_Name;
        bgm.Stop();
        bgm.Stream = bgm_song;
        bgm.VolumeDb = BGM_LIST[BGM_Name].volume;
        bgm.Play();
    }

    public void PlayBGM_Custom(AudioStream stream, float volume, string name)
    {
        currently_playing = name;
        bgm.Stop();
        bgm.Stream = stream;
        bgm.VolumeDb = volume;
        bgm.Play();
    }

    public void PlaySFX(SFX sfx, bool reserve = false, bool could_interupt = false)
    {
        if (can_interupt || (!can_interupt && could_interupt))
        {
            can_interupt = !reserve;
            switch (sfx)
            {
                case SFX.CONFIRM:
                    ui_sfx.Stop();
                    ui_sfx.Stream = confirm_sfx;
                    ui_sfx.Play();
                    break;
                case SFX.DENY:
                    ui_sfx.Stop();
                    ui_sfx.Stream = deny_sfx;
                    ui_sfx.Play();
                    break;
                case SFX.HOVER:
                    ui_sfx.Stop();
                    ui_sfx.Stream = hover_sfx;
                    ui_sfx.Play();
                    break;
                case SFX.SCROLL_BUTTON:
                    ui_sfx.Stop();
                    ui_sfx.Stream = scroll_sfx;
                    ui_sfx.Play();
                    break;
                case SFX.SELECT:
                    ui_sfx.Stop();
                    ui_sfx.Stream = select_sfx;
                    ui_sfx.Play();
                    break;
                default:
                    break;
            }
        }
    }
}
