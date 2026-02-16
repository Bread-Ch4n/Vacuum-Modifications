using MelonLoader;
using MelonLoader.Utils;

namespace VacuumModifications;

public class Preferences
{
    private static readonly string PreferencePath = Path.Combine(
        MelonEnvironment.UserDataDirectory, "VacuumModifications"
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

        Mod.InstaVacpackPreferenceEntry = Mod.PlayerPreferences.CreateEntry(
            "Insta_Vacpack", new Mod.InstaVacpackEntry(false, ["<Keyboard>/leftCtrl"]),
            "Insta Vacpack"
        );

        #endregion

        #region Vacpack Shoot Cooldown

        Mod.VacShootCooldown = Mod.PlayerPreferences.CreateEntry(
            "Vacuum_Shoot_Cooldown",
            new Mod.VacpackCooldownEntry(false, 0.24),
            "Vacuum Shoot Cooldown"
        );

        #endregion

        #region Custom MaxAmount

        Mod.PlayerPreferenceEntry = Mod.PlayerPreferences.CreateEntry(
            "Player_Item_Limit", new Mod.LimitEntry(false, 100),
            "Player Item Limit"
        );

        #endregion

        #endregion

        #region Collectors

        Mod.CollectorsPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Collectors_Preferences",
            "Collectors"
        );
        Mod.CollectorsPreferences.SetFilePath(configFile, true, false);


        Mod.PlortCollector = Mod.CollectorsPreferences.CreateEntry(
            "Plort_Collector_Limit",
            new Mod.LimitEntry(false, 100),
            "Plort Collector Limit"
        );

        Mod.ElderCollector = Mod.CollectorsPreferences.CreateEntry(
            "Elder_Collector_Limit",
            new Mod.LimitEntry(false, 100),
            "Elder Collector Limit"
        );

        #endregion

        #region Feeder

        Mod.FeederPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Feeder_Preferences",
            "Feeder"
        );
        Mod.FeederPreferences.SetFilePath(configFile, true, false);

        Mod.Feeder = Mod.FeederPreferences.CreateEntry(
            "Feeder_Limit",
            new Mod.LimitEntry(false, 100),
            "Feeder Limit"
        );

        #endregion

        #region Silo

        Mod.SiloPreferences = MelonPreferences.CreateCategory(
            "Vacuum_Modification_Silo_Preferences",
            "Silo"
        );
        Mod.SiloPreferences.SetFilePath(configFile, true, false);

        Mod.Silo = Mod.SiloPreferences.CreateEntry(
            "Silo_Custom_Limit",
            new Mod.LimitEntry(false, 100),
            "Silo Custom Limit"
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