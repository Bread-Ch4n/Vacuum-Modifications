using Il2Cpp;
using MelonLoader;

namespace VacuumModifications;

public class AmmoManager
{
    public static int CalculateMaxAmmoAmount(AmmoSlotDefinition ammoSlot, IdentifiableType id)
    {
        if (ammoSlot == null)
            return -1;

        #region MoreVaccables Compatibility

        var isLargo =
            id != null && (Mod.LargoGroup?.IsMember(id) == true || id.referenceId == "SlimeDefinition.Tarr");
        var half = isLargo && Mod.HalfForMoreVaccablesModLargos?.Value == true;

        #endregion

        var maxCount = GetCustomLimit(ammoSlot);
        if (maxCount == -1)
            return -1;
        return half ? maxCount / 2 : maxCount;
    }

    private static int GetCustomLimit(AmmoSlotDefinition ammoSlot)
    {
        var rules = new (string Key, Mod.LimitEntry)[]
        {
            ("AmmoSlot", Mod.PlayerPreferenceEntry!.Value),
            (
                "ElderCollector",
                Mod.ElderCollector!.Value
            ),
            (
                "PlortCollector",
                Mod.PlortCollector!.Value
            ),
            ("Feeder", Mod.Feeder!.Value),
            ("Silo", Mod.Silo!.Value)
        };

        foreach (var (key, limitEntry) in rules)
            if (
                ammoSlot.name.Contains(key, StringComparison.OrdinalIgnoreCase)
                || ammoSlot.name == key
            )
                return limitEntry.Enabled ? limitEntry.Limit : -1;

        MelonDebug.Msg($"Unknown Ammo Slot {ammoSlot.name}");
        return -1;
    }
}