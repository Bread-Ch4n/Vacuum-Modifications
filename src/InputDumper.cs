using MelonLoader;
using MelonLoader.Utils;
using UnityEngine.InputSystem;

namespace VacuumModifications;

public static class InputDumper
{
    private static readonly string OutputPath =
        Path.Combine(MelonEnvironment.UserDataDirectory, "VacuumModifications", "InputBindingsDump.txt");

    public static void DumpAllBindings()
    {
        const string warning =
            "WARNING: Ensure that keyboard, gamepad, and mouse are connected before launching the game, " +
            "otherwise some bindings may not be detected.";
        using (var writer = new StreamWriter(OutputPath, false))
        {
            // Warning at the top
            writer.WriteLine(warning);
            writer.WriteLine(new string('=', warning.Length));
            writer.WriteLine();

            // Keyboard
            writer.WriteLine("=== Keyboard Keys ===");
            if (Keyboard.current != null && Keyboard.current.allKeys.Count > 0)
                foreach (var key in Keyboard.current.allKeys)
                {
                    var path = AddAngleBrackets(key.path);
                    writer.WriteLine(path); // e.g., <Keyboard>/a
                }
            else
                writer.WriteLine("Keyboard not detected.");

            writer.WriteLine();
            writer.WriteLine("=== Gamepad Controls ===");
            if (Gamepad.all.Count > 0)
                foreach (var gamepad in Gamepad.all)
                {
                    writer.WriteLine($"--- {gamepad.displayName} ---");
                    foreach (var control in gamepad.allControls)
                    {
                        var path = AddAngleBrackets(control.path);
                        writer.WriteLine(path); // e.g., <Gamepad>/buttonSouth
                    }
                }
            else
                writer.WriteLine("No gamepads detected.");

            writer.WriteLine();
            writer.WriteLine("=== Mouse Buttons ===");
            if (Mouse.current != null && Mouse.current.allControls.Count > 0)
                foreach (var control in Mouse.current.allControls)
                {
                    var path = AddAngleBrackets(control.path);
                    writer.WriteLine(path); // e.g., <Mouse>/leftButton, <Mouse>/forwardButton
                }
            else
                writer.WriteLine("Mouse not detected.");
        }

        MelonLogger.Msg($"Input bindings dumped to: {OutputPath}");
    }

    private static string AddAngleBrackets(string path)
    {
        if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
            return path;

        var slashIndex = path.IndexOf('/', 1);
        if (slashIndex <= 1) return $"<{path[1..]}>";
        var device = path[1..slashIndex];
        var control = path[slashIndex..];
        return $"<{device}>{control}";
    }
}