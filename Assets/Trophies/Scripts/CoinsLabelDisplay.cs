using Godot;
using System;

public class CoinsLabelDisplay : Label
{
    private TrophySystem trophySystem;
    public override void _Ready()
    {
        trophySystem = GetNode<TrophySystem>("/root/TrophySystem");
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        Text = "Total Coins: " + trophySystem.coins_display;
    }
}
