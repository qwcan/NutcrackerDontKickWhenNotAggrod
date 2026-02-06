using System.Runtime.Serialization;
using BepInEx.Configuration;
using CSync.Lib;
using BepInEx.Configuration;
using CSync.Extensions;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using Unity.Netcode;
using UnityEngine;

namespace qwcanBarber;

public class BarberConfig : SyncedConfig2<BarberConfig>
{
    
    [SyncedEntryField] public SyncedEntry<float> minFadingDistance;
    [SyncedEntryField] public SyncedEntry<float> maxFadingDistance;

    
    public ConfigEntry<float> localMinFadingDistance;
    public ConfigEntry<float> localMaxFadingDistance;


    private static CanModifyResult IsHostCallback()
    {
        var startOfRound = StartOfRound.Instance;
        if (!startOfRound || startOfRound.IsHost)
        {
            return CanModifyResult.True(); // Main menu or hosting
        }
        return (false, "This setting can only be modified by the host.");

    }

    public BarberConfig(ConfigFile configFile) : base("BarberVisibilityTweaks")
    {
        
        minFadingDistance = configFile.BindSyncedEntry("Fading", "Minimum Fading Distance", 5f, "Distance at which the barber will be fully opaque" );
        maxFadingDistance = configFile.BindSyncedEntry("Fading", "Maximum Fading Distance", 7f, "Distance at which the barber will start fading in" );
        
        var minInput = new FloatStepSliderConfigItem(minFadingDistance.Entry, new FloatStepSliderOptions()
        {
            Min = 0,
            Max = 100,
            Step = 0.5f,
            RequiresRestart = false,
            CanModifyCallback = IsHostCallback
        });
        LethalConfigManager.AddConfigItem(minInput);
        var maxInput = new FloatStepSliderConfigItem(maxFadingDistance.Entry, new FloatStepSliderOptions()
        {
            Min = 0,
            Max = 100,
            Step = 0.5f,
            RequiresRestart = false,
            CanModifyCallback = IsHostCallback
        });
        LethalConfigManager.AddConfigItem(maxInput);

        minFadingDistance.Changed += (sender, args) =>
        {
            Plugin.Log.LogInfo($"minFadingDistance changed from {args.OldValue} to {args.NewValue}");
        };
        maxFadingDistance.Changed += (sender, args) =>
        {
            Plugin.Log.LogInfo($"maxFadingDistance changed from {args.OldValue} to {args.NewValue}");
        };
        
        ConfigManager.Register(this); 
    }
}