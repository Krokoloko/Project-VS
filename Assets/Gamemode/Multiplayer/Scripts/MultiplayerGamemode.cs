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

    Player winner = null;

    bool gamemode_finished = false;

    public override void _Ready()
    {
        win_state_UI = GetNode<Control>(win_state_UI_path);
        win_state_UI.Visible = false;
        win_label = GetNode<Label>(win_label_path);

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
            for(int i = 0; i < player_container.GetChildCount(); i++)
            {
                Player p = player_container.GetChildOrNull<Player>(i);
                if(p != null)
                {
                    if(p.GetStockCount() == 0)
                    {
                        winner = p;
                        break;
                    }    
                }
            }
            
            if(winner != null)
            {
                win_state_UI.Visible = true;
                gamemode_finished = true;
                win_state_UI.Visible = true;

                win_label.Text = "Player " + winner.GetPlayerID().ToString() + " Lost!!! L + Bozo + Ratiod + Be better + Skill issue";
                
                TrophySystem trophySystem = GetNode<TrophySystem>("/root/TrophySystem");
                trophySystem.AddCoins(5);
            }
        }
    }
}
