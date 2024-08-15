using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Serialization;

public class TrophySystem : Node
{
    private const string TROPHY_ROOT = "res://Assets/Trophies/TrophyData";
    private List<string> unlockable_trophies = new List<string>();
    private List<string> unlocked_trophies = new List<string>();
    private int coins = 0;
    public string coins_display = "0";
    public override void _EnterTree()
    {
        Directory directory = new Directory();
        if(directory.Open(TROPHY_ROOT) == Error.Ok)
        {
            directory.ListDirBegin(true, true);
            string file_name = directory.GetNext();
            bool is_dir = directory.CurrentIsDir();
            while(file_name != "" || is_dir)
            {
                if(file_name.Contains(".tres"))
                {
                    unlockable_trophies.Add(directory.GetCurrentDir() + "/" + file_name);
                }
                file_name = directory.GetNext();
            }
            directory.ListDirEnd();
        }
    }

    public TrophyData GetRandomNewTrophy()
    {
        if(unlockable_trophies.Count == unlocked_trophies.Count)
        {
            return null;
        }
        while(true)
        {
            int i = (int)(GD.Randi()/2) % unlockable_trophies.Count;
            if(!unlocked_trophies.Contains(unlockable_trophies[i]))
            {
                unlocked_trophies.Add(unlockable_trophies[i]);
                return (TrophyData)GD.Load<Resource>(unlockable_trophies[i]);
            }
        }
    }
    public int TotalTrophiesUnlocked()
    {
        return unlocked_trophies.Count;
    }
    public int GetCoins() { return coins; }
    public bool SubtractCoins(int sub_coins)
    {
        if(sub_coins > coins)
        {
            return false;
        }
        coins -= sub_coins;
        coins_display = coins.ToString();
        return true;
    }
    public void AddCoins (int add_coins)
    {
        coins += add_coins;
        coins_display = coins.ToString();
    }

}
