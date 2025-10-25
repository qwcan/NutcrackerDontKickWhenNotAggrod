using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using qwcannutcracker.patch;

namespace qwcanNutcracker;

[BepInPlugin("NutcrackerDontKickWhenNotAggrod", "NutcrackerDontKickWhenNotAggrod", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; set; }

    public static ManualLogSource Log => Instance.Logger;

    private readonly Harmony _harmony = new("NutcrackerDontKickWhenNotAggrod");


    public Plugin()
    {
        Instance = this;
    }

    private void Awake()
    {
        Log.LogInfo($"Applying patches...");
        ApplyPluginPatch();
        Log.LogInfo($"Patches applied");
    }

    /// <summary>
    /// Applies the patch to the game.
    /// </summary>
    private void ApplyPluginPatch()
    {
        _harmony.PatchAll(typeof(NutcrackerPatch));
    }
}
