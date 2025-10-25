using System.Collections.Generic;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using qwcanNutcracker;

namespace qwcannutcracker.patch;

/// <summary>
/// Patch to modify the behavior of the nutcracker. Instead of kicking when touching a player while wandering, it will
/// aggro first, only kicking if it's already aggroed.
/// </summary>
[HarmonyPatch(typeof(NutcrackerEnemyAI))]
public class NutcrackerPatch
{
    /*
     * Pseudocode:
		//ORIGINAL CODE
		base.OnCollideWithPlayer(other);
		if (!isEnemyDead && !(timeSinceHittingPlayer < 1f) && !(stunNormalizedTimer >= 0f))
		{
			PlayerControllerB playerControllerB = MeetsStandardPlayerCollisionConditions(other, reloadingGun || aimingGun);
			if (playerControllerB != null)
			{
				timeSinceHittingPlayer = 0f;
				//BEGIN OUR CODE
				if not aggro'd:
					SeeMovingThreatServerRpc((int)playerControllerB.playerClientId);
					return
				else:
				//END OUR CODE
					LegKickPlayerServerRpc((int)playerControllerB.playerClientId);
			}
		}
		
     */
    
    // Insert code to aggro first instead of kicking
    [HarmonyPatch(nameof(NutcrackerEnemyAI.OnCollideWithPlayer))]
    [HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        Plugin.Log.LogInfo("Patching NutcrackerEnemyAI");
        var code = new List<CodeInstruction>(instructions);
        var timeSinceHittingPlayerField = AccessTools.Field(typeof(NutcrackerEnemyAI), "timeSinceHittingPlayer");
        Plugin.Log.LogInfo($"Found timeSinceHittingPlayer field: {timeSinceHittingPlayerField}");



        Label kickLabel = il.DefineLabel();
        
        for (int i = 0; i < code.Count; i++)
        {
            var instr = code[i];
            yield return instr;

            if (instr.StoresField(timeSinceHittingPlayerField))
            {
	            Plugin.Log.LogInfo("Found timeSinceHittingPlayer ldfld, inserting code");
	            //Label the next instruction (should be a ldarg.0) so we can jump to it
	            code[i+1].labels.Add(kickLabel);
                //if not aggro'd:
                yield return new CodeInstruction(OpCodes.Ldarg_0); //This
                yield return new CodeInstruction( OpCodes.Ldfld, AccessTools.Field(typeof(EnemyAI), "currentBehaviourStateIndex") ); //this.currentBehaviourStateIndex
                yield return new CodeInstruction(OpCodes.Ldc_I4_2); //2
                yield return new CodeInstruction(OpCodes.Beq_S, kickLabel); //if( this.currentBehaviourStateIndex == 2 ) goto kickLabel
                
                /*
	            IL_0000: call class [BepInEx]BepInEx.Logging.ManualLogSource qwcan.Plugin::get_Log()
	            IL_0005: ldstr "msg"
	            IL_000a: callvirt instance void [BepInEx]BepInEx.Logging.ManualLogSource::LogInfo(object)
                 */
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Plugin), "get_Log"));
                yield return new CodeInstruction(OpCodes.Ldstr, "Starting aggro (touched while wandering)");
                yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ManualLogSource), "LogInfo", new[] {typeof(object)}));
                
                //Start aggro then return
                // SeeMovingThreatServerRpc((int)playerControllerB.playerClientId);
                yield return new CodeInstruction(OpCodes.Ldarg_0); //this
                yield return new CodeInstruction(OpCodes.Ldloc_0); //this.playerControllerB
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(GameNetcodeStuff.PlayerControllerB), "playerClientId") ); //this.playerControllerB.playerClientId
                yield return new CodeInstruction(OpCodes.Conv_I4); //(int)this.playerControllerB.playerClientId
                yield return new CodeInstruction(OpCodes.Ldc_I4_1); //true
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(NutcrackerEnemyAI), "SeeMovingThreatServerRpc"));//SeeMovingThreatServerRpc((int)this.playerControllerB.playerClientId)
                yield return new CodeInstruction(OpCodes.Ret); //return
            }
        }
    }

}
