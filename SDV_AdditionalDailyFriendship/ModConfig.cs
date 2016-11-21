using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

public class GeneralConfig
{
    public bool enabled
    {
        get; set;
    }

    public bool noFriendshipDecay
    {
        get; set;
    }

    public bool noPassiveIncrease
    {
        get; set;
    }

    public bool randomizeIncrease
    {
        get; set;
    }

    public List<SpecificConfig> individualConfigs
    {
        get; set;
    }

    public GeneralConfig()
    {
        this.enabled = true;
        this.noFriendshipDecay = true;
        this.noPassiveIncrease = false;
        this.randomizeIncrease = false;
        this.individualConfigs = new List<SpecificConfig>();
        individualConfigs.Add(new SpecificConfig("Default", 5, 8, 2500));
    }
}

public class SpecificConfig
{
    public string name
    {
        get; set;
    }
    public int baseIncrease
    {
        get; set;
    }
    public int talkIncrease
    {
        get; set;
    }
    public int max
    {
        get; set;
    }

    public SpecificConfig(string name, int baseIncrease, int talkIncrease, int max)
    {
        this.name = name;
        this.baseIncrease = baseIncrease;
        this.talkIncrease = talkIncrease;
        this.max = max;
    }
}