using GameNetcodeStuff;
using HarmonyLib;
using Helmet.Behaviour;

namespace Helmet.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal static class PlayerControllerBPatcher
    {

        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerControllerB.DamagePlayer))]
        static bool DamagePlayerPrefix(ref PlayerControllerB __instance, ref int damageNumber)
        {
            if (!__instance.IsOwner || __instance.isPlayerDead || !__instance.AllowPlayerDeath()) return true;
            if (HelmetManager.Instance.wearingHelmet)
            {
                HelmetBehaviour.ExecuteHelmetDamageMitigation(ref __instance, ref damageNumber);
            }
            return damageNumber != 0;
        }
    }
}
