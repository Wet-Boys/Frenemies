using EmotesAPI;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;

namespace FrenemiesProject
{
    public class FriendlyComponent : MonoBehaviour
    {
        public HealthComponent healthComponent;
        public BoneMapper boneMapper;
        public CharacterBody body;
        public bool friendly = true;
        public float joinTimer = 0;
        BoneMapper nearestMapper = null;
        List<GenericSkill> skillList = new List<GenericSkill>();
        void Update()
        {
            if (body.GetComponent<TeamComponent>().teamIndex != TeamIndex.Player)
            {
                if (boneMapper.currentClipName != "none" && healthComponent.timeSinceLastHit < 5)
                {
                    for (int i = 0; i < skillList.Count; i++)
                    {
                        GenericSkill skill = skillList[i];
                        skill.stock = 0;
                        skill.rechargeStopwatch = 0;
                    }
                    CustomEmotesAPI.PlayAnimation("none", boneMapper);
                }
                else if (boneMapper.currentClipName == "none" && healthComponent.timeSinceLastHit > 5)
                {
                    joinTimer += Time.deltaTime;
                    if (joinTimer > 10)
                    {
                        joinTimer -= 10;
                        if (Settings.EnemiesCanJoinEnemies.Value && UnityEngine.Random.Range(0, 100) > (99 - Settings.EnemiesJoinEnemiesChance.Value))
                        {
                            try
                            {
                                foreach (var mapper in CustomEmotesAPI.GetAllBoneMappers())
                                {
                                    try
                                    {
                                        if (mapper != boneMapper)
                                        {
                                            if (!nearestMapper && (mapper.currentClip.syncronizeAnimation || mapper.currentClip.syncronizeAudio))
                                            {
                                                nearestMapper = mapper;
                                            }
                                            else if (nearestMapper)
                                            {
                                                if ((mapper.currentClip.syncronizeAnimation || mapper.currentClip.syncronizeAudio) && Vector3.Distance(boneMapper.transform.position, mapper.transform.position) < Vector3.Distance(boneMapper.transform.position, nearestMapper.transform.position))
                                                {

                                                    nearestMapper = mapper;
                                                }
                                            }
                                        }
                                    }
                                    catch (System.Exception)
                                    {
                                    }
                                }
                                if (nearestMapper && Vector3.Distance(boneMapper.transform.position, nearestMapper.transform.position) < 25)
                                {
                                    //DebugClass.Log($"playing {nearestMapper.currentClip.clip[0].name} on {boneMapper} because we got lucky");
                                    CustomEmotesAPI.PlayAnimation(nearestMapper.currentClip.clip[0].name, boneMapper);
                                    CustomEmotesAPI.Joined(nearestMapper.currentClip.clip[0].name, boneMapper, nearestMapper);
                                }
                                nearestMapper = null;
                            }
                            catch (System.Exception e)
                            {
                            }
                        }
                    }
                }
            }

        }
        internal static void FriendlySetup()
        {
            CustomEmotesAPI.boneMapperCreated += CustomEmotesAPI_boneMapperCreated;
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        private static void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            FriendlyComponent friend = self.gameObject.GetComponent<FriendlyComponent>();
            TeamComponent t = self.GetComponent<TeamComponent>();
            if (friend && t && t.teamIndex != TeamIndex.Player)
            {
                if (!friend.friendly || (Settings.EnemiesWillNeverAttackWhileEmoting.Value && friend.boneMapper.currentClipName != "none"))
                {
                    orig(self);
                    return;
                }
                try
                {
                    if (friend.boneMapper.currentClipName != "none" && friend.boneMapper.currentClip.vulnerableEmote && Settings.EnemiesNeutralWhenVulnerableEmoting.Value)
                    {
                        for (int i = 0; i < friend.skillList.Count; i++)
                        {
                            GenericSkill skill = friend.skillList[i];
                            skill.DeductStock(100);
                            skill.rechargeStopwatch = 0;

                        }
                        return;
                    }
                }
                catch (Exception)
                {
                }

                if (friend.healthComponent.timeSinceLastHit > 5) //not in combat
                {

                    BaseAI.Target target = self.characterBody.master.GetComponent<BaseAI>().currentEnemy;
                    FriendlyComponent targetFriendlyComponent = target?.characterBody.GetComponent<FriendlyComponent>();

                    if (targetFriendlyComponent) //only if target exists AND friendlycomponent exists on target
                    {

                        try
                        {
                            if (Settings.EnemiesCanJoinPlayer.Value && friend.boneMapper.currentClipName == "none" && targetFriendlyComponent.boneMapper.currentClipName != "none" && (targetFriendlyComponent.boneMapper.currentClip.syncronizeAnimation || targetFriendlyComponent.boneMapper.currentClip.syncronizeAudio))
                            {
                                CustomEmotesAPI.PlayAnimation(targetFriendlyComponent.boneMapper.currentClip.clip[0].name, friend.boneMapper);
                                CustomEmotesAPI.Joined(targetFriendlyComponent.boneMapper.currentClip.clip[0].name, friend.boneMapper, targetFriendlyComponent.boneMapper);
                            }

                        }
                        catch (Exception)
                        {
                        }
                        try
                        {

                            if (Settings.EnemiesNeutralToVulnerableEmotes.Value) //if enemies won't attack vulnerable emotes
                            {
                                if (targetFriendlyComponent.boneMapper.currentClip.vulnerableEmote) //if currentemote on the target is vulnerable
                                {
                                    for (int i = 0; i < friend.skillList.Count; i++)
                                    {
                                        GenericSkill skill = friend.skillList[i];
                                        skill.DeductStock(100);
                                        skill.rechargeStopwatch = 0;
                                    }
                                    return;
                                }
                            }

                        }
                        catch (Exception)
                        {
                        }
                    }

                }
            }
            orig(self);
        }

        // set stock to 0 and then turn off the genericskill

        private static void CustomEmotesAPI_boneMapperCreated(BoneMapper mapper)
        {
            CharacterBody body = mapper.transform.GetComponentInParent<CharacterModel>().body;
            FriendlyComponent f = body.gameObject.AddComponent<FriendlyComponent>();
            f.healthComponent = body.GetComponent<HealthComponent>();
            f.boneMapper = mapper;
            if (!Settings.PartyCrashers.Value)
            {
                f.friendly = true;
            }
            else
            {
                f.friendly = mapper.name != "elderlemurian" && UnityEngine.Random.Range(0, 100) < 94;
            }
            f.body = body;
            if (Settings.EnemiesCanSpawnEmoting.Value && UnityEngine.Random.Range(0, 100) > (99 - Settings.EnemiesJoinEnemiesChance.Value) && f.friendly && body.GetComponent<TeamComponent>().teamIndex != TeamIndex.Player)
            {
                while (true)
                {
                    string emoteToPlay = CustomEmotesAPI.allClipNames[UnityEngine.Random.Range(0, CustomEmotesAPI.allClipNames.Count)];
                    if (!FrenemiesPlugin.blacklistedSpawnEmotes.Contains(emoteToPlay))
                    {
                        FrenemiesPlugin.instance.PlayAfterSecondsNotIEnumerator(mapper, emoteToPlay, 2f);
                        break;
                    }
                }
            }
            foreach (var item in body.GetComponents<GenericSkill>())
            {
                f.skillList.Add(item);
            }
        }
    }
}
