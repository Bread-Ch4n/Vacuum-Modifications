using Il2Cpp;

namespace VacuumModifications;

public class AmmoManager
{
    public static int CalculateMaxAmmoAmount(AmmoSlotDefinition? ammoSlot, IdentifiableType id)
    {
        if (ammoSlot == null || id == null) return -1;

        var isLargo = Mod.LargoGroup?.IsMember(id) == true
                      || id.referenceId == "SlimeDefinition.Tarr";

        var half = isLargo && Mod.HalfForMoreVaccablesModLargos?.Value == true;

        var maxCount = ammoSlot.MaxAmount;

        maxCount = GetCustomLimit(ammoSlot, maxCount);

        return half ? maxCount / 2 : maxCount;
    }

    private static int GetCustomLimit(AmmoSlotDefinition ammoSlot, int fallback)
    {
        var overrides = new Dictionary<string, (Func<bool?> toggle, Func<int?> limit)>
        {
            ["AmmoSlot"] = (
                () => Mod.PlayerCustomToggle?.Value,
                () => Mod.PlayerCustomToggle?.Value == true
                    ? Mod.PlayerCustomLimit?.Value
                    : CalculatePlayerUpgradeLimit()
            ),
            ["Collector"] = (() => Mod.CollectorCustomToggle?.Value,
                () => Mod.CollectorCustomLimit?.Value),
            ["Feeder"] = (() => Mod.FeederCustomToggle?.Value, () => Mod.FeederCustomLimit?.Value),
            ["Silo"] = (() => Mod.SiloCustomToggle?.Value, () => Mod.SiloCustomLimit?.Value)
        };

        foreach (var kvp in overrides)
            if (ammoSlot.name.Contains(kvp.Key) && kvp.Value.toggle() == true)
                return kvp.Value.limit() ?? fallback;

        return fallback;
    }

    private static int CalculatePlayerUpgradeLimit()
    {
        var upgrade = Mod.Player?._model?.upgradeModel?.upgradeDefinitions.items?
            .Find(new Func<UpgradeDefinition, bool>(u => u.name == "AmmoCapacity"));

        if (upgrade == null) return Mod.PlayerCustomLimit != null ? Mod.PlayerCustomLimit.Value : 30;

        return 10 * (upgrade.LevelCount + 3);
    }
}