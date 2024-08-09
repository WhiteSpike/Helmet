using HarmonyLib;
using Helmet.Behaviour;
using Unity.Netcode;
using UnityEngine;

namespace Helmet.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal static class GameNetworkManagerPatcher
    {
        [HarmonyPatch(nameof(GameNetworkManager.Disconnect))]
        [HarmonyPrefix]
        static void DisconnectPrefix(GameNetworkManager __instance) 
        {
            if (!__instance.isHostingGame) return;
            if (!HelmetManager.Instance.wearingHelmet) return;
            GameObject dropHelmet = GameObject.Instantiate(Plugin.helmet, __instance.localPlayerController.transform.position, Quaternion.identity);
            dropHelmet.GetComponent<NetworkObject>().Spawn();
        }
    }
}
