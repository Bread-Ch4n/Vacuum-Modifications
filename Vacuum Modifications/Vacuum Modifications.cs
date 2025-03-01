#nullable enable
using System.Collections.Generic;
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using UnityEngine;
using MelonLoader;
using Vacuum_Modifications.containers;

namespace Vacuum_Modifications;

public class VacuumModifications : MelonMod
{
    public static PlayerState? Player;

    public static MelonPreferences_Category? VacModifications;
    public static MelonPreferences_Entry<double>? VacShootCooldown;
    public static MelonPreferences_Entry<int>? PlayerCustomLimit;
    public static MelonPreferences_Entry<int>? PlortCollectorCustomLimit;
    public static MelonPreferences_Entry<int>? FeederCustomLimit;
    public static MelonPreferences_Entry<int>? SiloCustomLimit;

    public static MelonPreferences_Category? VacModificationsToggles;
    public static MelonPreferences_Entry<bool>? VacShootCooldownToggle;
    public static MelonPreferences_Entry<bool>? PlayerCustomToggle;
    public static MelonPreferences_Entry<bool>? PlortCollectorCustomToggle;
    public static MelonPreferences_Entry<bool>? FeederCustomToggle;
    public static MelonPreferences_Entry<bool>? SiloCustomToggle;

    public static readonly bool IsMoreVaccablesInstalled = FindMelon("MoreVaccablesMod", "Atmudia") != null;
    public static IdentifiableTypeGroup? LargoGroup;
    public static MelonPreferences_Entry<bool>? HalfForMoreVaccablesModLargos;

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg($"MoreVaccablesMod is {(IsMoreVaccablesInstalled ? "Installed" : "Not Installed")}!");
        PreferenceManager.Init();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "PlayerCore") Player = SRSingleton<SceneContext>.Instance.PlayerState;
        LargoGroup = Utils.Get<IdentifiableTypeGroup>("LargoGroup");
    }

    [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
    private static class WeaponVacuumExpelPatch
    {
        private static bool Prefix()
        {
            if (VacShootCooldownToggle!.Value) Player!.VacuumItem.ShootCooldown = (float)VacShootCooldown!.Value;

            if (Player == null || Player.VacuumItem == null) return true;

            GameObject pointedAt = Utils.tryGetPointedObject(Player.VacuumItem);
            if (pointedAt == null || pointedAt.name != "triggerDeposit") return true;

            IItemContainer container = Utils.tryGetContainer(pointedAt);
            SiloCatcher silo = pointedAt.GetComponent<SiloCatcher>();

            if (container != null && silo == null && Player.Ammo.HasSelectedAmmo())
            {
                switch (container)
                {
                    case MarketContainer marketContainer:
                        marketContainer.add(Player.Ammo.GetSelectedId(),
                            Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex]!.Count);
                        return false;
                }
            }
            else if (silo != null && container == null && Player.Ammo.HasSelectedAmmo())
            {
                if (!silo._storageSilo.CanAccept(Player.Ammo.GetSelectedId())) return true;
                if (silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo._selectedAmmoIdx]?.Id != null && silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo._selectedAmmoIdx]?.Id != Player.Ammo.GetSelectedId()) return true;
                if (Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex] == null || silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo.SelectedAmmoIndex] == null) return true;
                Utils.tryTransferMaxAmount(Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex],
                    silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo.SelectedAmmoIndex]);
                // if (Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex] == null || silo._storageSilo.LocalAmmo.Slots[silo._storageSilo.LocalAmmo._selectedAmmoIdx] == null) return true;
                // Utils.tryTransferMaxAmount(Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex],
                //     silo._storageSilo.LocalAmmo.Slots[silo._storageSilo.LocalAmmo._selectedAmmoIdx]);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(VacuumItem), "Consume")]
    private static class WeaponVacuumConsumePatch
    {
        private static bool Prefix(VacuumItem __instance, Il2CppSystem.Collections.Generic.HashSet<GameObject> inVac)
        {
            SiloCatcher silo = Utils.tryGetPointedObject<SiloCatcher>(Player!.VacuumItem);
            if (silo == null || silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo.SelectedAmmoIndex]?.Id == null) return true;
            Utils.tryTransferMaxAmount(silo._storageSilo.Ammo.Slots[silo._storageSilo.Ammo.SelectedAmmoIndex], Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex]);

            // if (silo == null || silo._storageSilo.LocalAmmo.Slots[silo._storageSilo.LocalAmmo._selectedAmmoIdx]?.Id == null) return true;
            // Utils.tryTransferMaxAmount(silo._storageSilo.LocalAmmo.Slots[silo._storageSilo.LocalAmmo._selectedAmmoIdx], Player.Ammo.Slots[Player.Ammo.SelectedAmmoIndex]);

            return false;
        }
    }

    // [HarmonyPatch(typeof(SiloCatcher), "Remove")]
    // private static class SiloCatcherDropPatch
    // {
    //     private static bool Prefix(SiloCatcher __instance)
    //     {
    //         MelonLogger.Msg($"SiloCatcher dropped");
    //         return true;
    //     }
    // }

    [HarmonyPatch(typeof(AmmoModel), "GetSlotMaxCount")]
    private static class AmmoModelGetSlotMaxCountPatch
    {
        public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
        {
            if (id != null) __result = AmmoManager.CalculateMaxAmmoAmount(__instance, index, id);
        }
    }

    [HarmonyPatch("Il2CppInterop.HarmonySupport.Il2CppDetourMethodPatcher", "ReportException")]
    public static class PatchIl2CppDetourMethodPatcher
    {
        public static bool Prefix(System.Exception ex)
        {
            MelonLogger.Error("During invoking native->managed trampoline", ex);
            return false;
        }
    }
}