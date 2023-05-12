using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FrenemiesProject
{
    public static class Settings
    {
        public static ConfigEntry<bool> EnemiesNeutralToVulnerableEmotes;
        public static ConfigEntry<bool> EnemiesNeutralWhenVulnerableEmoting;
        public static ConfigEntry<bool> EnemiesCanJoinPlayer;
        public static ConfigEntry<bool> EnemiesCanJoinJoinSpots;
        public static ConfigEntry<bool> PartyCrashers;
        public static ConfigEntry<bool> EnemiesCanSpawnEmoting;
        public static ConfigEntry<float> EnemiesSpawnEmoteChance;
        public static ConfigEntry<bool> EnemiesCanJoinEnemies;
        public static ConfigEntry<float> EnemiesJoinEnemiesChance;
        public static ConfigEntry<bool> EnemiesWillNeverAttackWhileEmoting;

        internal static void RunAll()
        {
            ModSettingsManager.SetModIcon(Assets.Load<Sprite>("@ExampleEmotePlugin_example_emotes:assets/icon.png"));
            SetupConfig();
            SetupROO();
        }
        internal static void SetupConfig()
        {
            EnemiesNeutralToVulnerableEmotes = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Are Neutral To Vulerable Players", true, "Enemies will be neutral to players who are performing emotes that make them vulnerable, I.E.: enmotes that lock movement. Vulnerable emotes have to be labeled by emote creators");
            EnemiesNeutralWhenVulnerableEmoting = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Are Neutral While Performing Vulnerable Emotes", true, "When an enemy is performing an emote that is marked as vulnerable they will be neutral");
            EnemiesWillNeverAttackWhileEmoting = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Wont Attack During Emotes", false, "Similar to above, but applies to ALL emotes.");
            EnemiesCanJoinPlayer = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Can Join Players", true, "Enemies will join their target if the target is performing an emote that can sync");
            EnemiesCanJoinJoinSpots = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Can Use JoinSpots", true, "If an enemy touches a JoinSpot they will use it");
            EnemiesCanJoinEnemies = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Can Join Enemies", true, "Enemies will periodically try to join a nearby enemy if said nearby enemy is performing an emote that can sync");
            EnemiesJoinEnemiesChance = FrenemiesPlugin.instance.Config.Bind<float>("Frenemies", "Enemies Join Enemies Chance", 20, "The percentage chance that enemies will join other nearby enemies");
            PartyCrashers = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Party Crashers", false, "Some enemies might just not be feeling it today");
            EnemiesCanSpawnEmoting = FrenemiesPlugin.instance.Config.Bind<bool>("Frenemies", "Enemies Can Spawn Emoting", true, "Chance that enemies will perform a random emote upon spawning. Mod creators can blacklist their emote from being in this list.");
            EnemiesSpawnEmoteChance = FrenemiesPlugin.instance.Config.Bind<float>("Frenemies", "Enemies Spawn Emoting Chance", 5, "The actual percentage value for enemies to spawn emoting.");
        }
        internal static void SetupROO()
        {
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesNeutralToVulnerableEmotes, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesNeutralWhenVulnerableEmoting, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesWillNeverAttackWhileEmoting, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesCanJoinPlayer, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesCanJoinJoinSpots, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesCanJoinEnemies, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(EnemiesJoinEnemiesChance, new SliderConfig() { restartRequired = false, checkIfDisabled = CheckEnemies }));
            ModSettingsManager.AddOption(new CheckBoxOption(PartyCrashers, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new CheckBoxOption(EnemiesCanSpawnEmoting, new CheckBoxConfig() { restartRequired = false }));
            ModSettingsManager.AddOption(new SliderOption(EnemiesSpawnEmoteChance, new SliderConfig() { restartRequired = false, checkIfDisabled = CheckSpawn }));
        }

        private static bool CheckEnemies()
        {
            return !EnemiesCanJoinEnemies.Value;
        }
        private static bool CheckSpawn()
        {
            return !EnemiesCanSpawnEmoting.Value;
        }
    }

}
