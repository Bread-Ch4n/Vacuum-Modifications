using Il2Cpp;
using MelonLoader;

namespace VacuumModifications;

public class AmmoManager
{
    public static int CalculateMaxAmmoAmount(AmmoSlotDefinition? ammoSlot, IdentifiableType? id)
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
        var rules = new (string Key, Func<bool?> Toggle, Func<int?> Limit)[]
        {
            ("AmmoSlot", () => Mod.PlayerCustomToggle?.Value, () => Mod.PlayerCustomLimit?.Value),
            (
                "ElderCollector",
                () => Mod.ElderCollectorCustomToggle?.Value,
                () => Mod.ElderCollectorCustomLimit?.Value
            ),
            (
                "PlortCollector",
                () => Mod.PlortCollectorCustomToggle?.Value,
                () => Mod.PlortCollectorCustomLimit?.Value
            ),
            ("Feeder", () => Mod.FeederCustomToggle?.Value, () => Mod.FeederCustomLimit?.Value),
            ("Silo", () => Mod.SiloCustomToggle?.Value, () => Mod.SiloCustomLimit?.Value)
        };

        foreach (var (key, toggle, limit) in rules)
            if (
                ammoSlot.name.Contains(key, StringComparison.OrdinalIgnoreCase)
                || ammoSlot.name == key
            )
                return toggle() == true ? limit() ?? -1 : -1;

        MelonLogger.Msg($"Unknown Ammo Slot {ammoSlot.name}");
        return -1;
    }
}
