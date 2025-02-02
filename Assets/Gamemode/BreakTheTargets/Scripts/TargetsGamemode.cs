using Godot;
using System;
using System.Linq;

public class TargetsGamemode : Node
{
	public enum GameState
	{
		PLAYING,
		WIN,
		LOST
	}
	[Export]
	private NodePath target_nodes = "";


	private int targets_alive = 0;

	[Export]
	private NodePath timer_node = "";
	private Timer game_timer;

	[Export]
	private NodePath timer_display_node = "";
	private Label timer_display;

	[Export]
	private NodePath player_root_node = "";
	private Player player;

	[Export]
	private float bottom_blast_zone = 0;
	private GameState state;

	[Export]
	private NodePath winscreen_node = "";
	private Control winscreen_ui;

	[Export]
	private NodePath losescreen_node = "";
	private Control losescreen_ui;

	[Export]
	private NodePath timeupscreen_node = "";
	private Control timeupscreen_ui;

	[Export]
	private NodePath buttons_node = "";
	private Control buttons_ui;

	[Export]
	private NodePath focus_node = "";
	private Control focus;

	[Export]
	private NodePath narrator_node = "";
	private AudioStreamPlayer narrator;
	[Export]
	private AudioStream win_sound;
	[Export]
	private AudioStream lose_sound;
	public override void _Ready()
	{
		state = GameState.PLAYING;

		focus = GetNode<Control>(focus_node);

		Node root = GetNode<Node>(player_root_node);
		for (int i = 0; i < root.GetChildCount(); i++)
		{
			string name = root.GetChild(i).Name;
			if(root.GetChild(i).Name.Contains("Player"))
			{
				player = root.GetChild<Player>(i);
			}
		}

		Node targets_node = GetNode<Node>(target_nodes);
		
		targets_alive = targets_node.GetChildCount();

		for (int i = 0; i < targets_alive; i++)
		{
			targets_node.GetChild<Target>(i).Connect("OnDestroy", this, "TargetLost");
		}

		game_timer = GetNode<Timer>(timer_node);
		game_timer.Connect("timeout", this, "TriggerLoseState");

		winscreen_ui = GetNode<Control>(winscreen_node);
		winscreen_ui.Visible = false;

		losescreen_ui = GetNode<Control>(losescreen_node);
		losescreen_ui.Visible = false;

		timeupscreen_ui = GetNode<Control>(timeupscreen_node);
		timeupscreen_ui.Visible = false;

		buttons_ui = GetNode<Control>(buttons_node);
		buttons_ui.Visible = false;

		narrator = GetNode<AudioStreamPlayer>(narrator_node);

		timer_display = GetNode<Label>(timer_display_node);

	}

	private void TriggerLoseState()
	{
		if(state != GameState.PLAYING) return;
		state = GameState.LOST;
		GD.Print("You lose!!");
		timeupscreen_ui.Visible = true;
		buttons_ui.Visible = true;
		narrator.Stream = lose_sound;
		game_timer.Paused = true;
		focus.GrabFocus();
		narrator.Play();
	}

	private void TargetLost(Node target)
	{
		targets_alive--;
		if(targets_alive == 0 && state == GameState.PLAYING)
		{
			GD.Print("You win!!");
			focus.GrabFocus();
			winscreen_ui.Visible = true;
			buttons_ui.Visible = true;
			narrator.Stream = win_sound;
			narrator.Play();
			game_timer.Paused = true;
			state = GameState.WIN;
			TrophySystem trophySystem = GetNode<TrophySystem>("/root/TrophySystem");
			trophySystem.AddCoins(5);
		}
	}

	public override void _Process(float delta)
	{
		float time = game_timer.TimeLeft;
		string minutes = ((int)(time / 60)).ToString();
		string seconds = (((int)time) % 60).ToString();
		string ms = time.ToString();
		ms = ms.Substr(ms.Find('.') + 1, ms.Find('.') + 3);
		timer_display.Text = minutes + ":" + seconds + ":" + ms;
		if(player.GlobalTranslation.y < bottom_blast_zone && state == GameState.PLAYING)
		{
			TriggerLoseState();
			losescreen_ui.Visible = true;
			timeupscreen_ui.Visible = false;
		}
	}
}
