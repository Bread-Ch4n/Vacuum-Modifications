using MelonLoader;
using MelonLoader.Utils;

namespace VacuumModifications;

public class Preferences
{
    public static readonly string PreferencePath =
        Path.Combine(MelonEnvironment.UserDataDirectory, "VacuumModifications");

    public static void Init()
    {
        Directory.CreateDirectory(PreferencePath);

        #region Values

        Mod.VacModifications = MelonPreferences.CreateCategory("Vacuum Modification Values");
        Mod.VacModifications.SetFilePath(Path.Combine(PreferencePath, "Values.cfg"));

        Mod.VacShootCooldown =
            Mod.VacModifications!.CreateEntry("Vacuum_Shoot_Cooldown", 0.24,
                "Vacuum Shoot Cooldown (0.24)");
        Mod.PlayerCustomLimit =
            Mod.VacModifications.CreateEntry("Player_Vacuum_Custom_MaxAmount", 100,
                "Player Vacuum Custom Max Item Amount (100)");
        Mod.CollectorCustomLimit =
            Mod.VacModifications.CreateEntry("Collector_Custom_MaxAmount", 100,
                "Collector Custom Max Item Amount (100) [Plort,Elder Collectors]");
        Mod.FeederCustomLimit =
            Mod.VacModifications.CreateEntry("Feeder_Custom_MaxAmount", 100,
                "Feeder Custom Max Item Amount (100)");
        Mod.SiloCustomLimit =
            Mod.VacModifications.CreateEntry("Silo_Custom_MaxAmount", 100,
                "Silo Custom Max Item Amount (100)");
        Mod.HalfForMoreVaccablesModLargos = Mod.IsMoreVaccablesInstalled
            ? Mod.VacModifications.CreateEntry("Half-space for More Vaccables Mod Largos", true,
                "Half-space for More Vaccables Mod Largos (true)")
            : null;

        #endregion

        #region Toggles

        Mod.VacModificationsToggles = MelonPreferences.CreateCategory("Vacuum Modification Toggles");
        Mod.VacModificationsToggles.SetFilePath(Path.Combine(PreferencePath, "Toggles.cfg"));

        Mod.VacShootCooldownToggle =
            Mod.VacModificationsToggles!.CreateEntry("Toggle Vacuum Shoot Cooldown", true,
                "Vacuum Shoot Cooldown (true)");
        Mod.PlayerCustomToggle =
            Mod.VacModificationsToggles.CreateEntry("Player Vacuum Custom Item Limit", true,
                "Toggle Player Vacuum Custom Item Limit (true)");
        Mod.CollectorCustomToggle =
            Mod.VacModificationsToggles.CreateEntry("Plort Collector Custom Item Limit", true,
                "Toggle Plort Collector Custom Item Limit (true)");
        Mod.FeederCustomToggle =
            Mod.VacModificationsToggles.CreateEntry("Toggle Feeder Custom Item Limit", true,
                "Toggle Feeder Custom Item Limit (true)");
        Mod.SiloCustomToggle =
            Mod.VacModificationsToggles.CreateEntry("Toggle Silo Custom Item Limit", true,
                "Toggle Silo Custom Item Limit (true)");

        #endregion

        Mod.VacModifications.SaveToFile();
        Mod.VacModificationsToggles.SaveToFile();
    }
}