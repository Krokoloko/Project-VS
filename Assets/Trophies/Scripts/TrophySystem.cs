using Godot;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Serialization;

public class TrophySystem : Node
{
    private const string TROPHY_ROOT = "res://Assets/Trophies/TrophyData";
    private List<string> unlockable_trophies = new List<string>();
    private Dictionary<int, string> unlocked_trophies = new Dictionary<int, string>();
    private int coins = 0;
    public string coins_display = "0";
    public override void _EnterTree()
    {
        ulong random_time = (ulong)Time.GetTicksMsec();
        GD.Seed(random_time);

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

        //Test Trophy Gallery
        AddCoins(10);
    }

    public int GetTotalUnlockableTrophies()
    {
        return unlockable_trophies.Count;
    }
    public Dictionary<int, TrophyData> GetAllUnlockedTrophies()
    {
        Dictionary<int, TrophyData> current_trophy_list = new Dictionary<int, TrophyData>();

        for(int i = 0; i < unlockable_trophies.Count; i++)
        {
            if(unlocked_trophies.ContainsKey(i))
            {
                TrophyData data = (TrophyData)GD.Load<Resource>(unlocked_trophies[i]);
                current_trophy_list.Add(i, data);
            }
        }
        return current_trophy_list;
    }

    public TrophyData GetRandomNewTrophy()
    {
        if(unlockable_trophies.Count == unlocked_trophies.Count)
        {
            return null;
        }
        var randomiser = new RandomNumberGenerator();
        while(true)
        {
            int i = randomiser.RandiRange(0,unlockable_trophies.Count-1);
            if(!unlocked_trophies.ContainsKey(i))
            {
                unlocked_trophies.Add(i, unlockable_trophies[i]);
                return (TrophyData)GD.Load<Resource>(unlockable_trophies[i]);
            }
        }
    }
    public int TotalTrophiesUnlocked()
    {
        return unlocked_trophies.Count;
    }
    public int GetCoins() 
    { 
        return coins; 
    }
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
