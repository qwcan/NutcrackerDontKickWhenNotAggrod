using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using qwcanBarber.patch;
using BepInEx.Configuration;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;


namespace qwcanBarber;


[BepInPlugin("BarberVisibilityTweaks", "BarberVisibilityTweaks", "1.0.0")]
[BepInDependency("ainavt.lc.lethalconfig")]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; set; }

    public static ManualLogSource Log => Instance.Logger;

    private readonly Harmony _harmony = new("BarberVisibilityTweaks");


    public ConfigEntry<float> minFadingDistance;
    public ConfigEntry<float> maxFadingDistance;
    
    public Plugin()
    {
        Instance = this;
    }

    private void Awake()
    {
        minFadingDistance = Config.Bind("Fading", "Minimum Fading Distance", 5f, "Distance at which the barber will be fully opaque");
        maxFadingDistance = Config.Bind("Fading", "Maximum Fading Distance", 7f, "Distance at which the barber will start fading in");
        var minInput = new FloatStepSliderConfigItem(minFadingDistance, new FloatStepSliderOptions()
        {
            Min = 0,
            Max = 100,
            Step = 0.5f,
            RequiresRestart = false
        });
        LethalConfigManager.AddConfigItem(minInput);
        var maxInput = new FloatStepSliderConfigItem(maxFadingDistance, new FloatStepSliderOptions()
        {
            Min = 0,
            Max = 100,
            Step = 0.5f,
            RequiresRestart = false
        });
        LethalConfigManager.AddConfigItem(maxInput);
        
        Log.LogInfo($"Applying patches...");
        ApplyPluginPatch();
        Log.LogInfo($"Patches applied");
    }

    /// <summary>
    /// Applies the patch to the game.
    /// </summary>
    private void ApplyPluginPatch()
    {
        _harmony.PatchAll(typeof(BarberPatch));
    }
}