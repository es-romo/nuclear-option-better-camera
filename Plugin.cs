using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace BetterCamera;

[BepInPlugin("sh.romo.BetterCamera", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("NuclearOption.exe")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    private static Plugin instance;
    private readonly Harmony harmony = new("sh.romo.BetterCamera");

    private ConfigEntry<string> configCameraUp;

    public Plugin()
    {
        configCameraUp = Config.Bind("Keybinds",      // The section under which the option is shown
                             "CameraUp",  // The key of the configuration option in the configuration file
                             "Space", // The default value
                             "Moves the camera up"); // Description of the option to show in the config file

    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        if (instance == null)
        {
            instance = this;
        }

        harmony.PatchAll();
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
