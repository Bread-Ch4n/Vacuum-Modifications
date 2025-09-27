using MelonLoader;
using MelonLoader.Utils;

namespace VacuumModifications;

public class Preferences
{
    private static readonly string PreferencePath = Path.Combine(
        MelonEnvironment.UserDataDirectory
    );

    public static void Init()
    {
        Directory.CreateDirectory(PreferencePath);

        var configFile = Path.Combine(PreferencePath, "VacuumModifications.cfg");

        #region Player

        Mod.PlayerPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Player_Preferences",
            "Player"
        );
        Mod.PlayerPreferences.SetFilePath(configFile, true, false);

        #region Insta Vacpack

        Mod.InstaVacpackToggle = Mod.PlayerPreferences.CreateEntry(
            "Insta_Vacpack_Toggle",
            false,
            "Insta Vacpack Toggle"
        );

        #endregion

        #region Vacpack Shoot Cooldown

        Mod.VacShootCooldownToggle = Mod.PlayerPreferences.CreateEntry(
            "Vacuum_Shoot_Cooldown_Toggle",
            false,
            "Vacuum Shoot Cooldown Toggle"
        );

        Mod.VacShootCooldown = Mod.PlayerPreferences.CreateEntry(
            "Vacuum_Shoot_Cooldown",
            0.24,
            "Vacuum Shoot Cooldown (0.24)"
        );

        #endregion

        #region Custom MaxAmount

        Mod.PlayerCustomToggle = Mod.PlayerPreferences.CreateEntry(
            "Player_Vacuum_Custom_MaxAmount_Toggle",
            false,
            "Player Vacuum Custom MaxAmount Toggle"
        );

        Mod.PlayerCustomLimit = Mod.PlayerPreferences.CreateEntry(
            "Player_Vacuum_Custom_MaxAmount",
            100,
            "Player Vacuum Custom Max Item Amount (100)"
        );

        #endregion

        #endregion

        #region Collectors

        Mod.CollectorsPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Collectors_Preferences",
            "Collectors"
        );
        Mod.CollectorsPreferences.SetFilePath(configFile, true, false);

        #region Plort

        Mod.PlortCollectorCustomToggle = Mod.CollectorsPreferences.CreateEntry(
            "Plort_Collector_Custom_MaxAmount_Toggle",
            false,
            "Plort Collector Custom MaxAmount Toggle"
        );

        Mod.PlortCollectorCustomLimit = Mod.CollectorsPreferences.CreateEntry(
            "Plort_Collector_Custom_MaxAmount",
            100,
            "Plort Collector Custom MaxAmount (100)"
        );

        #endregion

        #region Elder

        Mod.ElderCollectorCustomToggle = Mod.CollectorsPreferences.CreateEntry(
            "Elder_Collector_Custom_MaxAmount_Toggle",
            false,
            "Elder Collector Custom MaxAmount Toggle"
        );

        Mod.ElderCollectorCustomLimit = Mod.CollectorsPreferences.CreateEntry(
            "Elder_Collector_Custom_MaxAmount",
            100,
            "Elder Collector Custom Max Item Amount (100)"
        );

        #endregion

        #endregion

        #region Feeder

        Mod.FeederPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Feeder_Preferences",
            "Feeder"
        );
        Mod.FeederPreferences.SetFilePath(configFile, true, false);

        Mod.FeederCustomToggle = Mod.FeederPreferences.CreateEntry(
            "Feeder_Custom_MaxAmount_Toggle",
            false,
            "Feeder Custom MaxAmount Toggle"
        );

        Mod.FeederCustomLimit = Mod.FeederPreferences.CreateEntry(
            "Feeder_Custom_MaxAmount",
            100,
            "Feeder Custom MaxAmount (100)"
        );

        #endregion

        #region Silo

        Mod.SiloPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Silo_Preferences",
            "Silo"
        );
        Mod.SiloPreferences.SetFilePath(configFile, true, false);

        Mod.SiloCustomToggle = Mod.SiloPreferences.CreateEntry(
            "Silo_Custom_MaxAmount_Toggle",
            false,
            "Silo Custom_MaxAmount_Toggle"
        );

        Mod.SiloCustomLimit = Mod.SiloPreferences.CreateEntry(
            "Silo_Custom_MaxAmount",
            100,
            "Silo Custom MaxAmount (100)"
        );

        #endregion

        #region Compatability

        Mod.CompatibilityPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Compatibility_Preferences",
            "Compatibility"
        );
        Mod.CompatibilityPreferences.SetFilePath(configFile, true, false);

        Mod.HalfForMoreVaccablesModLargos = Mod.IsMoreVaccablesInstalled
            ? Mod.CompatibilityPreferences.CreateEntry(
                "Half-space_for_More_Vaccables_Mod_Largos",
                false,
                "Half-space for More Vaccables Mod Largos"
            )
            : null;

        #endregion

        MelonLogger.Msg("Preferences Loaded!");
        Mod.PlayerPreferences.SaveToFile(false);
        Mod.CollectorsPreferences.SaveToFile(false);
        Mod.FeederPreferences.SaveToFile(false);
        Mod.SiloPreferences.SaveToFile(false);
        Mod.CompatibilityPreferences.SaveToFile(false);
        MelonLogger.Msg("Preferences Saved!");
    }
}
