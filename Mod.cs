using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Player;
using Il2CppMonomiPark.SlimeRancher.Player.PlayerItems;
using Il2CppMonomiPark.UnitPropertySystem;
using MelonLoader;
using VacuumModifications;

[assembly:
    MelonInfo(typeof(Mod), "Vacuum Modifications", "2.0.1", "Bread-Chan",
        "https://www.nexusmods.com/slimerancher2/mods/45")]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]

namespace VacuumModifications;

public class Mod : MelonMod
{
    #region Patches

    [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Start))]
    [HarmonyPrefix]
    private static void VacuumItem_Start(VacuumItem __instance)
    {
        if (PlayerCustomToggle!.Value)
            foreach (var ammoSlot in Player!.Ammo.Slots)
                ammoSlot._maxCountValue = new NullableFloatProperty(PlayerCustomLimit!.Value);

        if (VacShootCooldownToggle!.Value) __instance.ShootCooldown = (float)VacShootCooldown!.Value;
    }

    [HarmonyPatch(typeof(AmmoSlot), nameof(AmmoSlot.Setup))]
    [HarmonyPrefix]
    public static void AmmoSlot_Setup(AmmoSlot __instance)
    {
        var ammoSlot = __instance.Definition;
        if (ammoSlot == null) return;
        var res = AmmoManager.CalculateMaxAmmoAmount(ammoSlot, __instance.Id);
        if (res == -1) return;
        ammoSlot._maxAmount = res;
    }

    #endregion

    #region Variables

    public static PlayerState? Player;

    #region MoreVaccables Compatibility Variables

    public static readonly bool IsMoreVaccablesInstalled = FindMelon("MoreVaccablesMod", "Atmudia") != null;
    public static IdentifiableTypeGroup? LargoGroup;
    public static MelonPreferences_Entry<bool>? HalfForMoreVaccablesModLargos;

    #endregion

    #endregion

    #region Preference Variables

    public static MelonPreferences_Category? VacModifications;
    public static MelonPreferences_Entry<double>? VacShootCooldown;
    public static MelonPreferences_Entry<int>? PlayerCustomLimit;
    public static MelonPreferences_Entry<int>? CollectorCustomLimit;
    public static MelonPreferences_Entry<int>? FeederCustomLimit;
    public static MelonPreferences_Entry<int>? SiloCustomLimit;

    public static MelonPreferences_Category? VacModificationsToggles;
    public static MelonPreferences_Entry<bool>? VacShootCooldownToggle;
    public static MelonPreferences_Entry<bool>? PlayerCustomToggle;
    public static MelonPreferences_Entry<bool>? CollectorCustomToggle;
    public static MelonPreferences_Entry<bool>? FeederCustomToggle;
    public static MelonPreferences_Entry<bool>? SiloCustomToggle;

    #endregion

    #region Setup

    public override void OnInitializeMelon()
    {
        Preferences.Init();
        var h = new HarmonyLib.Harmony("com.bread-chan.vacuum_modifications");
        h.PatchAll(typeof(Mod));
        MelonLogger.Msg("Initialized.");
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "PlayerCore") Player = SRSingleton<SceneContext>.Instance.PlayerState;
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