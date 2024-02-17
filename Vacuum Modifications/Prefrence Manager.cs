using MelonLoader;

namespace Vacuum_Modifications;

public abstract class PreferenceManager
{
    public static void Init()
    {
        // Values
        VacuumModifications.VacModifications = MelonPreferences.CreateCategory("Vacuum Modification Values");
        VacuumModifications.VacShootCooldown =
            VacuumModifications.VacModifications!.CreateEntry("Vacuum Shoot Cooldown", 0.24,
                "Vacuum Shoot Cooldown (0.24)");
        VacuumModifications.PlayerCustomLimit =
            VacuumModifications.VacModifications.CreateEntry("Player Vacuum Custom Item Limit", 100,
                "Player Vacuum Custom Item Limit (100)");
        VacuumModifications.PlortCollectorCustomLimit =
            VacuumModifications.VacModifications.CreateEntry("Plort Collector Custom Item Limit", 100,
                "Plort Collector Custom Item Limit (100)");
        VacuumModifications.FeederCustomLimit =
            VacuumModifications.VacModifications.CreateEntry("Feeder Custom Item Limit", 100,
                "Feeder Custom Item Limit (100)");
        VacuumModifications.SiloCustomLimit =
            VacuumModifications.VacModifications.CreateEntry("Silo Custom Item Limit", 100,
                "Silo Custom Item Limit (100)");
        VacuumModifications.HalfForMoreVaccablesModLargos = VacuumModifications.IsMoreVaccablesInstalled
            ? VacuumModifications.VacModifications.CreateEntry("Half space for More Vaccables Mod Largos", true,
                "Half space for More Vaccables Mod Largos (true)")
            : null;

        // Toggles
        VacuumModifications.VacModificationsToggles = MelonPreferences.CreateCategory("Vacuum Modification Toggles");
        VacuumModifications.VacShootCooldownToggle =
            VacuumModifications.VacModificationsToggles!.CreateEntry("Toggle Vacuum Shoot Cooldown", true,
                "Vacuum Shoot Cooldown (true)");
        VacuumModifications.PlayerCustomToggle =
            VacuumModifications.VacModificationsToggles.CreateEntry("Player Vacuum Custom Item Limit", true,
                "Toggle Player Vacuum Custom Item Limit (true)");
        VacuumModifications.PlortCollectorCustomToggle =
            VacuumModifications.VacModificationsToggles.CreateEntry("Plort Collector Custom Item Limit", true,
                "Toggle Plort Collector Custom Item Limit (true)");
        VacuumModifications.FeederCustomToggle =
            VacuumModifications.VacModificationsToggles.CreateEntry("Toggle Feeder Custom Item Limit", true,
                "Toggle Feeder Custom Item Limit (true)");
        VacuumModifications.SiloCustomToggle =
            VacuumModifications.VacModificationsToggles.CreateEntry("Toggle Silo Custom Item Limit", true,
                "Toggle Silo Custom Item Limit (true)");
    }
}