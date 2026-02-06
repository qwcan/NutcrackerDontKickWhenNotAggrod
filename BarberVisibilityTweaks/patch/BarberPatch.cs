using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using qwcanBarber;
using UnityEngine;
using LethalConfig;
using Unity.Netcode;

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
        //Debug.Log($"Min: {__instance.minDistance}");
        //Debug.Log($"Max: {__instance.maxDistance}");
        __instance.minDistance = Plugin.Config.minFadingDistance.Value;
        __instance.maxDistance = Plugin.Config.maxFadingDistance.Value;

    }
    
    
    
    [HarmonyPatch(typeof(NetworkManager))]
    internal static class NetworkPrefabPatch2
    {
        private static readonly string MOD_GUID = "BarberVisibilityTweaks";

        [HarmonyPostfix]
        [HarmonyPatch(nameof(NetworkManager.SetSingleton))]
        private static void RegisterPrefab()
        {
            var prefab = new GameObject(MOD_GUID + " Prefab");
            prefab.hideFlags |= HideFlags.HideAndDontSave;
            Object.DontDestroyOnLoad(prefab);
            var networkObject = prefab.AddComponent<NetworkObject>();
            var fieldInfo = typeof(NetworkObject).GetField("GlobalObjectIdHash", BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo!.SetValue(networkObject, GetHash(MOD_GUID));

            NetworkManager.Singleton.PrefabHandler.AddNetworkPrefab(prefab);
            return;

            static uint GetHash(string value)
            {
                return value?.Aggregate(17u, (current, c) => unchecked((current * 31) ^ c)) ?? 0u;
            }
        }
    }
}
