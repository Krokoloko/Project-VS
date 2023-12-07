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
	private NodePath target_nodes;


	private int targets_alive;

	[Export]
	private NodePath timer_node;
	private Timer game_timer;

	[Export]
	private NodePath player_root_node;
	private Player player;

	[Export]
	private float bottom_blast_zone;
	private GameState state;

	[Export]
	private NodePath winscreen_node;
	private Control winscreen_ui;

	[Export]
	private NodePath losescreen_node;
	private Control losescreen_ui;

	[Export]
	private NodePath timeupscreen_node;
	private Control timeupscreen_ui;

	[Export]
	private NodePath buttons_node;
	private Control buttons_ui;

	public override void _Ready()
	{
		state = GameState.PLAYING;

		Node root = GetNode<Node>(player_root_node);
		for (int i = 0; i < root.GetChildCount(); i++)
		{
			string name = root.GetChild(i).Name;
			if(root.GetChild(i).Name == "Player")
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
	}

	private void TriggerLoseState()
	{
		if(state != GameState.PLAYING) return;
		state = GameState.LOST;
		GD.Print("You lose!!");
		timeupscreen_ui.Visible = true;
		buttons_ui.Visible = true;
	}

	private void TargetLost(Node target)
	{
		targets_alive--;
		if(targets_alive == 0 && state == GameState.PLAYING)
		{
			GD.Print("You win!!");
			winscreen_ui.Visible = true;
			buttons_ui.Visible = true;
			state = GameState.WIN;
		}
	}

	public override void _Process(float delta)
	{
		if(player.GlobalTranslation.y < bottom_blast_zone && state == GameState.PLAYING)
		{
			TriggerLoseState();
			losescreen_ui.Visible = true;
			timeupscreen_ui.Visible = false;
		}
	}
}
