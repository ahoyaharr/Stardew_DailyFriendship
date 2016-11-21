using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace SDV_AdditionalDailyFriendship
{
    public class SDV_AdditionalDailyFriendship : Mod
    {
        public static GeneralConfig ModConfig
        {
            get;
            private set;
        }

        public static Farmer Player => Game1.player;
        public static Game1 TheGame => Program.gamePtr;
        public static Random rnd = new Random();
        public static Dictionary<string, int> prevFriends;

        public override void Entry(ModHelper helper)
        {
            ModConfig = helper.ReadConfig<GeneralConfig>();
            GameEvents.OneSecondTick += new EventHandler(this.on_each_second);
            TimeEvents.OnNewDay += new EventHandler<EventArgsNewDay>(this.on_new_day);

            Log.Info($"SDV_AdditionalDailyFriendship... loaded!");
        }

        private void on_each_second(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame)
            {
                return;
            }
            prevent_friendship_decay();
        }

        private void on_new_day(object sender, EventArgsNewDay e)
        {
            if (!Game1.hasLoadedGame)
            {
                return;
            }
            add_friendship(sender, e);
        }

        private static void prevent_friendship_decay()
        {
            if (ModConfig.enabled && ModConfig.noFriendshipDecay)
            {
                if (prevFriends == null)
                {
                    SerializableDictionary<string, int[]> serializableDictionary = Player.friendships;
                    prevFriends = serializableDictionary.ToDictionary((KeyValuePair<string, int[]> p) => p.Key.ToString(), (KeyValuePair<string, int[]> p) => p.Value[0]);
                }
                foreach (KeyValuePair<string, int[]> friendship in Player.friendships)
                {
                    foreach (KeyValuePair<string, int> prevFriend in prevFriends)
                    {
                        if (!friendship.Key.Equals(prevFriend.Key) || friendship.Value[0] >= prevFriend.Value)
                        {
                            continue;
                        }
                        friendship.Value[0] = prevFriend.Value;
                    }
                }
                SerializableDictionary<string, int[]> serializableDictionary1 = Player.friendships;
                prevFriends = serializableDictionary1.ToDictionary((KeyValuePair<string, int[]> p) => p.Key.ToString(), (KeyValuePair<string, int[]> p) => p.Value[0]);
            }
        }

        private static void add_friendship(object sender, EventArgsNewDay e)
        {
            if (!ModConfig.enabled || !e.IsNewDay)
            {
                return;
            }

            List<SpecificConfig> SpecificConfigs = ModConfig.individualConfigs;
            SortedDictionary<string, SpecificConfig> npcConfigsMap = new SortedDictionary<string, SpecificConfig>();

            /*Log.Debug("Loading configurations...");
            foreach (SpecificConfig SpecificConfig in SpecificConfigs)
            {
                npcConfigsMap.Add(SpecificConfig.name, SpecificConfig);
            }*/

            if (!npcConfigsMap.ContainsKey("Default"))
            {
                npcConfigsMap.Add("Default", new SpecificConfig("Default", 5, 8, 2500));
            }
            
            string[] npcNames = Player.friendships.Keys.ToArray();
            if (!ModConfig.noPassiveIncrease)
            {
                Log.Info($"=== Adding Friendship ===");
                foreach (string npcName in npcNames)
                {
                    SpecificConfig config = npcConfigsMap.ContainsKey(npcName) ? npcConfigsMap[npcName] : npcConfigsMap["Default"];
                    int[] friendshipParams = Player.friendships[npcName];
                    int friendshipValue = friendshipParams[0];

                    int friendship_increase = 
                        friendshipValue > config.max 
                            ? 0 
                            : ModConfig.randomizeIncrease
                                    ? Player.hasPlayerTalkedToNPC(npcName)
                                        ? (rnd.Next(1, config.baseIncrease * 2) * Player.LuckLevel) + rnd.Next(1, config.talkIncrease * 2)
                                        : (rnd.Next(1, config.baseIncrease) * Player.LuckLevel) + rnd.Next(1, config.talkIncrease)
                                    : Player.hasPlayerTalkedToNPC(npcName)
                                        ? config.talkIncrease * 2
                                        : config.baseIncrease * 2;

                    friendshipValue += friendship_increase;
                    Log.Info($"{npcName} friendship level has increased by {friendship_increase} points. (Current = {friendshipValue}.)");
                    Player.friendships[npcName][0] = friendshipValue;
                }
                Log.Info($"=== Finished Adding Friendship ===");
            }
        }
    }
}