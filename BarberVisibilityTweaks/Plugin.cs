using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using qwcanBarber.patch;


namespace qwcanBarber;


[BepInPlugin("BarberVisibilityTweaks", "BarberVisibilityTweaks", "1.0.0")]
[BepInDependency("ainavt.lc.lethalconfig")]
[BepInDependency("com.sigurd.csync", "5.0.1")] 
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance { get; set; }

    public static ManualLogSource Log => Instance.Logger;

    private readonly Harmony _harmony = new("BarberVisibilityTweaks");
    
    internal new static BarberConfig Config; 
    
    public Plugin()
    {
        Instance = this;
    }

    private void Awake()
    {
        Config = new BarberConfig(base.Config); 
        
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