using BepInEx;
using BepInEx.Configuration;
using EmotesAPI;
using R2API;
using R2API.Utils;
using RiskOfOptions;
using RiskOfOptions.Options;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FrenemiesProject
{
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI")]
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class FrenemiesPlugin : BaseUnityPlugin
    {
        public const string PluginGUID = "com.weliveinasociety.Frenemies";
        public const string PluginName = "Frenemies";
        public const string PluginVersion = "1.0.0";
        public static FrenemiesPlugin instance;
        public static List<string> blacklistedSpawnEmotes = new List<string>();
        public void Awake()
        {
            instance = this;
            Assets.PopulateAssets();
            Settings.RunAll();
            FriendlyComponent.FriendlySetup();
            CustomEmotesAPI.boneMapperEnteredJoinSpot += CustomEmotesAPI_boneMapperEnteredJoinSpot;
        }

        private void CustomEmotesAPI_boneMapperEnteredJoinSpot(BoneMapper mover, BoneMapper joinSpotOwner)
        {
            if (Settings.EnemiesCanJoinJoinSpots.Value && mover.mapperBody.teamComponent.teamIndex != TeamIndex.Player && mover.mapperBody.GetComponent<HealthComponent>().timeSinceLastHit > 5 && mover.mapperBody.GetComponent<FriendlyComponent>().friendly)
            {
                mover.JoinEmoteSpot();
            }
        }

        internal void PlayAfterSecondsNotIEnumerator(BoneMapper mapper, string animName, float seconds)
        {
            StartCoroutine(PlayAfterSeconds(mapper, animName, seconds));
        }
        internal IEnumerator PlayAfterSeconds(BoneMapper mapper, string animName, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            CustomEmotesAPI.PlayAnimation(animName, mapper);
            //DebugClass.Log($"playing {animName} on {mapper} after {seconds} seconds have passed");
        }
    }
}
