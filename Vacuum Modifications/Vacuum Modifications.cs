#nullable enable
using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using MelonLoader;
using UnityEngine;

namespace Vacuum_Modifications;

public class VacuumModifications : MelonMod
{
    private static PlayerState? _player;

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
        MelonLogger.Msg("MoreVaccablesMod is " + (IsMoreVaccablesInstalled ? "Installed" : "Not Installed") + "!");
        PreferenceManager.Init();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "PlayerCore") _player = SRSingleton<SceneContext>.Instance.PlayerState;
        LargoGroup = Utils.Get<IdentifiableTypeGroup>("LargoGroup");
    }

    [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel), typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet))]
    private static class WeaponVacuumExpelPatch
    {
        private static void Postfix()
        {
            if (VacShootCooldownToggle!.Value)
            {
                _player!.VacuumItem.ShootCooldown = (float)VacShootCooldown!.Value;
            }
        }
    }

    [HarmonyPatch(typeof(AmmoModel), "GetSlotMaxCount")]
    private static class AmmoModelGetSlotMaxCountPatch
    {
        public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
        {
            if (id != null) __result = AmmoManager.CalculateMaxAmmoAmount(__instance, index, id);
        }
    }
}