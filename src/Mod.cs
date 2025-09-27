using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;
using VacuumModifications;

[assembly: MelonInfo(
    typeof(Mod),
    "Vacuum Modifications",
    "2.1.1",
    "Bread-Chan",
    "https://www.nexusmods.com/slimerancher2/mods/45"
)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]

namespace VacuumModifications;

public class Mod : MelonMod
{
    #region Patches

    private class VacuumCooldown
    {
        [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Start))]
        [HarmonyPrefix]
        private static void VacuumItem_Start(VacuumItem __instance)
        {
            if (VacShootCooldownToggle!.Value)
                __instance.ShootCooldown = (float)VacShootCooldown!.Value;
        }
    }

    private class CustomItemLimits
    {
        [HarmonyPatch(typeof(AmmoSlot), nameof(AmmoSlot.MaxCount), MethodType.Getter)]
        [HarmonyPostfix]
        [HarmonyPriority(HarmonyLib.Priority.Last)]
        private static void AmmoSlot_MaxCount(AmmoSlot __instance, ref int __result)
        {
            var res = AmmoManager.CalculateMaxAmmoAmount(__instance.Definition, __instance.Id);
            if (res == -1)
                return;
            __instance.Definition._maxAmount = res;
            __result = res;
        }

        // Just in case
        [HarmonyPatch(typeof(AmmoSlot), nameof(AmmoSlot.Setup))]
        [HarmonyPrefix]
        public static void AmmoSlot_Setup(AmmoSlot __instance)
        {
            var ammoSlot = __instance.Definition;
            if (ammoSlot == null)
                return;
            var res = AmmoManager.CalculateMaxAmmoAmount(ammoSlot, __instance.Id);
            if (res == -1)
                return;
            ammoSlot._maxAmount = res;
        }
    }

    private class InstaVacpack
    {
        #region Expel

        [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(float))]
        [HarmonyPrefix]
        private static bool WeaponVacuumExpelPatch(VacuumItem __instance)
        {
            if (!Keyboard.current.leftCtrlKey.isPressed)
                return true;
            if (Player == null || !Player.Ammo.HasSelectedAmmo() || Player.VacuumItem == null)
                return true;
            var sourceSlot = Player.Ammo.Slots[Player.Ammo._selectedAmmoIdx];
            if (sourceSlot == null || sourceSlot.Id == null)
                return true;

            var pointedAt = Utils.tryGetPointedObject(Player.VacuumItem);
            if (!pointedAt || pointedAt?.name != "triggerDeposit")
                return true;

            var container = Utils.tryGetContainer(pointedAt, sourceSlot.Id);

            switch (container)
            {
                case Containers.MarketplaceContainer marketplaceContainer:
                    if (marketplaceContainer.Add(sourceSlot.Count))
                        __instance.ShootEffect(.5f);
                    return false;
                case Containers.SiloContainer siloContainer:
                    if (siloContainer.Add(sourceSlot.Count))
                        __instance.ShootEffect(.5f);
                    return false;
                case Containers.RefineryContainer refineryContainer:
                    if (refineryContainer.Add(sourceSlot.Count))
                        __instance.ShootEffect(.5f);
                    return false;
                case Containers.FeederContainer feederContainer:
                    if (feederContainer.Add(sourceSlot.Count))
                        __instance.ShootEffect(.5f);
                    return false;
                default:
                    return true;
            }
        }

        #endregion

        #region Consume

        [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Consume))]
        [HarmonyPrefix]
        private static bool InstaGrab(VacuumItem __instance, HashSet<GameObject> inVac)
        {
            if (!Keyboard.current.leftCtrlKey.isPressed)
                return true;
            if (Player == null || Player.VacuumItem == null)
                return true;

            var pointedAt = Utils.tryGetPointedObject(Player.VacuumItem);
            if (!pointedAt || pointedAt?.name != "triggerDeposit")
                return true;

            var container = Utils.tryGetContainer(pointedAt);

            switch (container)
            {
                case Containers.SiloContainer siloContainer:
                {
                    var targetSlot = Utils.FindTargetSlot(siloContainer.Id);
                    if (targetSlot == null)
                        return true;
                    var transferSucceeded = Utils.tryTransferMaxAmount(
                        siloContainer.AmmoSlot,
                        targetSlot
                    );
                    if (transferSucceeded)
                        __instance.CaptureEffect();
                    return !transferSucceeded;
                }
                case Containers.FeederContainer feederContainer:
                {
                    var targetSlot = Utils.FindTargetSlot(feederContainer.Id);
                    if (targetSlot == null)
                        return true;
                    var transferSucceeded = Utils.tryTransferMaxAmount(
                        feederContainer.AmmoSlot,
                        targetSlot
                    );
                    if (transferSucceeded)
                        __instance.CaptureEffect();
                    return !transferSucceeded;
                }
                case Containers.PlortCollector plortCollector:
                {
                    var targetSlot = Utils.FindTargetSlot(plortCollector.Id);
                    if (targetSlot == null)
                        return true;
                    var transferSucceeded = Utils.tryTransferMaxAmount(
                        plortCollector.AmmoSlot,
                        targetSlot
                    );
                    if (transferSucceeded)
                        __instance.CaptureEffect();
                    return !transferSucceeded;
                }
                default:
                    return true;
            }
        }

        #endregion
    }

    #endregion

    #region Variables

    public static PlayerState? Player;

    #region MoreVaccables Compatibility Variables

    public static readonly bool IsMoreVaccablesInstalled =
        FindMelon("MoreVaccablesMod", "Atmudia") != null;

    public static IdentifiableTypeGroup? LargoGroup;

    #endregion

    #endregion

    #region Preference Variables

    public static MelonPreferences_Category? PlayerPreferences;
    public static MelonPreferences_Entry<bool>? InstaVacpackToggle;
    public static MelonPreferences_Entry<bool>? VacShootCooldownToggle;
    public static MelonPreferences_Entry<bool>? PlayerCustomToggle;
    public static MelonPreferences_Entry<double>? VacShootCooldown;
    public static MelonPreferences_Entry<int>? PlayerCustomLimit;

    public static MelonPreferences_Category? CollectorsPreferences;
    public static MelonPreferences_Entry<bool>? PlortCollectorCustomToggle;
    public static MelonPreferences_Entry<bool>? ElderCollectorCustomToggle;
    public static MelonPreferences_Entry<int>? PlortCollectorCustomLimit;
    public static MelonPreferences_Entry<int>? ElderCollectorCustomLimit;

    public static MelonPreferences_Category? FeederPreferences;
    public static MelonPreferences_Entry<bool>? FeederCustomToggle;
    public static MelonPreferences_Entry<int>? FeederCustomLimit;

    public static MelonPreferences_Category? SiloPreferences;
    public static MelonPreferences_Entry<bool>? SiloCustomToggle;
    public static MelonPreferences_Entry<int>? SiloCustomLimit;

    public static MelonPreferences_Category? CompatibilityPreferences;
    public static MelonPreferences_Entry<bool>? HalfForMoreVaccablesModLargos;

    #endregion

    #region Setup

    public override void OnInitializeMelon()
    {
        Preferences.Init();
        var h = new HarmonyLib.Harmony("com.bread-chan.vacuum_modifications");

        if (VacShootCooldownToggle!.Value)
            h.PatchAll(typeof(VacuumCooldown));
        if (InstaVacpackToggle!.Value)
            h.PatchAll(typeof(InstaVacpack));
        if (
            PlayerCustomToggle!.Value
            || PlortCollectorCustomToggle!.Value
            || ElderCollectorCustomToggle!.Value
            || FeederCustomToggle!.Value
            || SiloCustomToggle!.Value
        )
            h.PatchAll(typeof(CustomItemLimits));

        MelonLogger.Msg("Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "PlayerCore")
            Player = SRSingleton<SceneContext>.Instance.PlayerState;
        LargoGroup = Utils.Get<IdentifiableTypeGroup>("LargoGroup");
    }

    [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
    [HarmonyPrefix]
    public static bool PatchIl2CppDetourMethodPatcher(Exception ex)
    {
        MelonLogger.Error("During invoking native->managed trampoline", ex);
        return false;
    }

    #endregion
}
