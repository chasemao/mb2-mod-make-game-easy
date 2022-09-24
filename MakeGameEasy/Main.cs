using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.WeaponDesign;
using System.Collections.ObjectModel;
using TaleWorlds.Library;
using TaleWorlds.Engine;
using Newtonsoft.Json;

namespace MakeGameEasy
{
    public class Main : MBSubModuleBase
    {
        private Harmony harmonyKit;
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            this.harmonyKit = new Harmony("HugeWeapon.harmony");
            this.harmonyKit.PatchAll();
            InformationManager.DisplayMessage(new InformationMessage("HugeWeapon loaded"));
        }
        public override void OnGameLoaded(Game game, object initializerObject)
        {
            InformationManager.DisplayMessage(new InformationMessage("HugeWeapon loaded"));
        }

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            if (!(game.GameType is Campaign))
                return;
            CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this,
                (Action)(() => DailyTick()));
        }
        public static void DailyTick()
        {
            foreach (Clan clan in Clan.All)
            {
                GiveGoldAction.ApplyBetweenCharacters(null, clan.Leader, 5000);
            }
        }
    }

    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "FocusPointsPerLevel", MethodType.Getter)]
    public class PatchFocusPointsPerLevelConst
    {
        public static void Postfix(ref int __result)
        {
            __result = 8;
        }
    }

    [HarmonyPatch(typeof(DefaultCharacterDevelopmentModel), "LevelsPerAttributePoint", MethodType.Getter)]
    public class PatchLevelsPerAttributePointConst
    {
        public static void Postfix(ref int __result)
        {
            __result = 1;
        }
    }

    [HarmonyPatch(typeof(DefaultNotificationsCampaignBehavior), "OnHeroGainedSkill")]
    public class PatchOnHeroGainedSkill
    {
        public static void Prefix(Hero hero, ref bool shouldNotify)
        {
            if (hero != Hero.MainHero)
                shouldNotify = false;
        }
    }

    [HarmonyPatch(typeof(Hero), "AddSkillXp")]
    public class PatcheAddSkillXp
    {
        public static void Prefix(Hero __instance, SkillObject skill, ref float xpAmount)
        {
            if (__instance == null || skill == null || __instance.HeroDeveloper == null || skill.GetName() == null || Hero.MainHero == null)
                return;
            xpAmount *= 30;
        }
    }

    [HarmonyPatch(typeof(DefaultCharacterStatsModel), "MaxHitpoints")]
    public class PatchMaxHitpoints
    {
        public static void Postfix(CharacterObject character, ref ExplainedNumber __result)
        {
            if (character.IsHero && character.HeroObject == Hero.MainHero)
                __result.Add(99999, new TextObject("我很牛逼"));
        }
    }

    [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetDailyHealingHpForHeroes")]
    public class PatchGetDailyHealingHpForHeroes
    {
        public static void Postfix(MobileParty party, ref ExplainedNumber __result)
        {
            if (party == MobileParty.MainParty)
                __result.Add(999, new TextObject("我很牛逼"));
        }
    }

    [HarmonyPatch(typeof(DefaultPartyHealingModel), "GetDailyHealingForRegulars")]
    public class PatchGetDailyHealingForRegulars
    {
        public static void Postfix(MobileParty party, ref ExplainedNumber __result)
        {
            if (party == MobileParty.MainParty)
                __result.Add(999, new TextObject("我很牛逼"));
        }
    }

    [HarmonyPatch(typeof(DefaultArmyManagementCalculationModel), "CalculateCohesionChange")]
    public class PatchCalculateCohesionChange
    {
        public static void Postfix(Army army, ref ExplainedNumber __result)
        {
            if (army.LeaderParty == MobileParty.MainParty)
            {
                __result.Add(100, new TextObject("我很牛逼"));
            }
        }
    }
}
