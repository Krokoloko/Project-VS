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

		game_timer = GetNode<Timer>(timer_node);
		game_timer.Connect("timeout", this, "TriggerLoseState");

		Node targets_node = GetNode<Node>(target_nodes);
		
		targets_alive = targets_node.GetChildCount();

		for (int i = 0; i < targets_alive; i++)
		{
			targets_node.GetChild<Target>(i).Connect("OnDestroy", this, "TargetLost");
		}
	}

	private void TriggerLoseState()
	{
		if(state != GameState.PLAYING) return;
		state = GameState.LOST;
		GD.Print("You lose!!");
	}

	private void TargetLost(Node target)
	{
		targets_alive--;
		if(targets_alive == 0 && state == GameState.PLAYING)
		{
			GD.Print("You win!!");
			state = GameState.WIN;
		}
	}

	public override void _Process(float delta)
	{
		if(player.GlobalTranslation.y < bottom_blast_zone)
		{
			TriggerLoseState();
		}
	}
}
