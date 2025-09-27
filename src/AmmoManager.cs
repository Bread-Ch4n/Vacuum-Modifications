using Il2Cpp;
using MelonLoader;

namespace VacuumModifications;

public class AmmoManager
{
    public static int CalculateMaxAmmoAmount(AmmoSlotDefinition? ammoSlot, IdentifiableType id)
    {
        if (ammoSlot == null || id == null)
            return -1;

        #region MoreVaccables Compatibility

        var isLargo =
            Mod.LargoGroup?.IsMember(id) == true || id.referenceId == "SlimeDefinition.Tarr";
        var half = isLargo && Mod.HalfForMoreVaccablesModLargos?.Value == true;

        #endregion

        var maxCount = GetCustomLimit(ammoSlot, ammoSlot.MaxAmount);
        return half ? maxCount / 2 : maxCount;
    }

    private static int GetCustomLimit(AmmoSlotDefinition ammoSlot, int fallback)
    {
        var rules = new (string Key, Func<bool?> Toggle, Func<int?> Limit, Func<int>? Default)[]
        {
            (
                "AmmoSlot",
                () => Mod.PlayerCustomToggle?.Value,
                () => Mod.PlayerCustomLimit?.Value,
                null
            ),
            (
                "ElderCollector",
                () => Mod.ElderCollectorCustomToggle?.Value,
                () => Mod.ElderCollectorCustomLimit?.Value,
                null
            ),
            (
                "PlortCollector",
                () => Mod.PlortCollectorCustomToggle?.Value,
                () => Mod.PlortCollectorCustomLimit?.Value,
                null
            ),
            (
                "Feeder",
                () => Mod.FeederCustomToggle?.Value,
                () => Mod.FeederCustomLimit?.Value,
                null
            ),
            ("Silo", () => Mod.SiloCustomToggle?.Value, () => Mod.SiloCustomLimit?.Value, null),
        };

        foreach (var (key, toggle, limit, def) in rules)
            if (
                ammoSlot.name.Contains(key, StringComparison.OrdinalIgnoreCase)
                || ammoSlot.name == key
            )
                return toggle() == true
                    ? limit() ?? (def?.Invoke() ?? fallback)
                    : def?.Invoke() ?? fallback;

        MelonLogger.Msg($"Unknown Ammo Slot {ammoSlot.name}");
        return fallback;
    }
}
