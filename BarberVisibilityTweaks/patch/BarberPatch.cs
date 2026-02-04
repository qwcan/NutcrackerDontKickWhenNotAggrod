using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using qwcanBarber;
using UnityEngine;
using LethalConfig;

namespace qwcanBarber.patch;

/// <summary>
/// Patch to modify the behavior of the claysurgeon/barber. It won't kill players unless it's moving, and will optionally do damage instead.
/// </summary>
[HarmonyPatch(typeof(ClaySurgeonAI))]
public class BarberPatch
{

    /*
    [HarmonyPatch(nameof(ClaySurgeonAI.OnCollideWithPlayer))]
    [HarmonyPrefix]
    private static bool OnCollideWithPlayer(ref ClaySurgeonAI __instance, ref Collider other)
    {
        Debug.Log($"Min: {__instance.minDistance}");
        Debug.Log($"Max: {__instance.maxDistance}");
        if (!__instance.isJumping)
        {
            //Debug.Log( "Not jumping, not killing player");
            return false;
        }

        return true;
    }
    */
    
    [HarmonyPatch(typeof(ClaySurgeonAI), "SetVisibility")]
    [HarmonyPrefix]
    private static void SetVisibility(ref ClaySurgeonAI __instance)
    {
        Debug.Log($"Min: {__instance.minDistance}");
        Debug.Log($"Max: {__instance.maxDistance}");
        __instance.minDistance = Plugin.Instance.minFadingDistance.Value;
        __instance.maxDistance = Plugin.Instance.maxFadingDistance.Value;

    }
}
