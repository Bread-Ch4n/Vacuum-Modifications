#nullable enable
using System;
using System.Linq;
using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Vacuum_Modifications;

public class VacuumModifications : MelonMod
{
    private static PlayerState? _player;
    private static MelonPreferences_Category? _vacModifications;
    private static MelonPreferences_Entry<double>? _vacShootCooldown;
    private static MelonPreferences_Entry<int>? _playerCustomLimit;
    private static MelonPreferences_Entry<int>? _plortCollectorCustomLimit;
    private static MelonPreferences_Entry<int>? _feederCustomLimit;
    private static MelonPreferences_Entry<int>? _siloCustomLimit;
    private static readonly bool IsMoreVaccablesInstalled = FindMelon("MoreVaccablesMod", "KomiksPL") != null;
    private static IdentifiableTypeGroup? _largoGroup;
    private static MelonPreferences_Entry<bool>? _halfForMoreVaccablesModLargos;

    public override void OnInitializeMelon()
    {
        MelonLogger.Msg("MoreVaccablesMod is " + (IsMoreVaccablesInstalled ? "Installed" : "Not Installed") + "!");
        _vacModifications = MelonPreferences.CreateCategory("Vac Modifications");
        _vacShootCooldown =
            _vacModifications.CreateEntry("Vacuum Shoot Cooldown", 0.24, "Vacuum Shoot Cooldown (0.24)");
        _playerCustomLimit = _vacModifications.CreateEntry("Player Vacuum Custom Item Limit", 100,
            "Player Vacuum Custom Item Limit (100)");
        _plortCollectorCustomLimit = _vacModifications.CreateEntry("Plort Collector Custom Item Limit", 100,
            "Plort Collector Custom Item Limit (100)");
        _feederCustomLimit =
            _vacModifications.CreateEntry("Feeder Custom Item Limit", 100, "Feeder Custom Item Limit (100)");
        _siloCustomLimit = _vacModifications.CreateEntry("Silo Custom Item Limit", 100, "Silo Custom Item Limit (100)");
        _halfForMoreVaccablesModLargos = IsMoreVaccablesInstalled
            ? _vacModifications.CreateEntry("Half space for More Vaccables Mod Largos", true,
                "Half space for More Vaccables Mod Largos (true)")
            : null;
    }

    private static T Get<T>(string name) where T : Object =>
        Resources.FindObjectsOfTypeAll<T>().FirstOrDefault((Func<T, bool>)(x => x.name == name))!;

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "PlayerCore") _player = SRSingleton<SceneContext>.Instance.PlayerState;
        _largoGroup = Get<IdentifiableTypeGroup>("LargoGroup");
    }

    private static int AmmoAmount(AmmoModel ammoModel, int index, bool isLargo)
    {
        var name = ammoModel.slots[index].Definition.name;
        var half = isLargo && _halfForMoreVaccablesModLargos!.Value;
        switch (name)
        {
            case "AmmoSlot":
                return half ? _playerCustomLimit!.Value : _playerCustomLimit!.Value / 2;
            case "PlortCollector":
                return _plortCollectorCustomLimit!.Value;
            case "Feeder":
                return _feederCustomLimit!.Value;
            case "Silo":
                return half ? _siloCustomLimit!.Value : _siloCustomLimit!.Value / 2;
            default:
                return _playerCustomLimit!.Value;
        }
    }

    private static int CalculateMaxAmmo(AmmoModel ammoModel, int index, IdentifiableType id) =>
        IsMoreVaccablesInstalled && (_largoGroup!.IsMember(id) || id.referenceId.Equals("SlimeDefinition.Tarr"))
            ? AmmoAmount(ammoModel, index, true)
            : AmmoAmount(ammoModel, index, false);

    [HarmonyPatch(typeof(VacuumItem), nameof(VacuumItem.Expel),
        new System.Type[]
        {
            typeof(GameObject), typeof(bool), typeof(float), typeof(SlimeAppearance.AppearanceSaveSet)
        })]
    private static class WeaponVacuumExpelPatch
    {
        private static void Postfix() => _player!.VacuumItem.ShootCooldown = (float)_vacShootCooldown!.Value;
    }

    [HarmonyPatch(typeof(AmmoModel), "GetSlotMaxCount")]
    private static class AmmoModelGetSlotMaxCountPatch
    {
        public static void Postfix(AmmoModel __instance, IdentifiableType id, int index, ref int __result)
        {
            if (id != null) __result = CalculateMaxAmmo(__instance, index, id);
        }
    }
}