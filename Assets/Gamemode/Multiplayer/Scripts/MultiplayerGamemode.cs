using Godot;
using System;

public class MultiplayerGamemode : Node
{
    [Export]
    private NodePath win_state_UI_path;
    private Control win_state_UI;

    [Export]
    private NodePath win_label_path;
    private Label win_label;

    [Export]
    private NodePath player_container_path;
    private Node player_container;

    [Export]
    private NodePath give_focus_path;
    private Control focus_node;
    private HitboxHandler hitbox_handler;
    Player winner = null;

    bool gamemode_finished = false;

    public override void _Ready()
    {
        win_state_UI = GetNode<Control>(win_state_UI_path);
        win_label = GetNode<Label>(win_label_path);
        focus_node = GetNode<Control>(give_focus_path);
        hitbox_handler = GetNode<HitboxHandler>("/root/HitboxHandler");
        win_state_UI.Visible = false;

        player_container = GetNode<Node>(player_container_path);

        for(int i = 0; i < player_container.GetChildCount(); i++)
        {
            Player p = player_container.GetChildOrNull<Player>(i);
            p?.Connect("OnPlayerDie", this, "CheckGameState");
        }
    }

    public void CheckGameState()
    {
        if(!gamemode_finished)
        {
            int total_losers = 0;
            for(int i = 0; i < player_container.GetChildCount(); i++)
            {
                Player p = player_container.GetChildOrNull<Player>(i);
                if(p != null)
                {
                    if(p.GetStockCount() == 0)
                    {
                        total_losers++;
                    }else
                    {
                        winner = p;
                    }    
                }
            }
            
            if(total_losers == player_container.GetChildCount()-1)
            {
                win_state_UI.Visible = true;
                gamemode_finished = true;
                win_state_UI.Visible = true;
                for(int i = 0; i < player_container.GetChildCount(); i++)
                {
                    Player p = player_container.GetChildOrNull<Player>(i);
                    if(p != null)
                    {
                        p.EndPlayer();
                    }
                }

                win_label.Text = "Player " + winner.GetPlayerID().ToString() + " Wins!";
                focus_node.GrabFocus();
                TrophySystem trophySystem = GetNode<TrophySystem>("/root/TrophySystem");
                trophySystem.AddCoins(5);
            }
        }
    }
}
