using Godot;
using System;

public class TrophiesUnlockedLabelDisplay : Label
{
    private TrophySystem trophySystem;
    public override void _Ready()
    {
        trophySystem = GetNode<TrophySystem>("/root/TrophySystem");
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        int number = trophySystem.TotalTrophiesUnlocked();
        Text = "Tophies Unlocked " + trophySystem.TotalTrophiesUnlocked() + "/" + trophySystem.GetTotalUnlockableTrophies();
    }
}
