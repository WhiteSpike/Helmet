using HarmonyLib;
using Helmet.Behaviour;
using Unity.Netcode;
using UnityEngine;

namespace Helmet.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal static class StartOfRoundPatcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StartOfRound.Awake))]
        static void InitLguStore(StartOfRound __instance)
        {
            Plugin.mls.LogDebug("Initiating components...");
            if (__instance.NetworkManager.IsHost || __instance.NetworkManager.IsServer)
            {
                GameObject behaviour = Object.Instantiate(Plugin.networkPrefab);
                behaviour.hideFlags = HideFlags.HideAndDontSave;
                behaviour.GetComponent<NetworkObject>().Spawn();
                Plugin.mls.LogDebug("Spawned behaviour...");
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StartOfRound.OnPlayerDC))]
        static void OnPlayerDCPostfix(StartOfRound __instance, int playerObjectNumber, ulong clientId)
        {
            if (!__instance.IsServer) return;
            if (!HelmetManager.Instance.clientWearingHelmet.TryGetValue(clientId, out bool wears) || !wears) return;
            GameObject dropHelmet = GameObject.Instantiate(Plugin.helmet, __instance.allPlayerScripts[playerObjectNumber].transform.position, Quaternion.identity);
            dropHelmet.GetComponent<NetworkObject>().Spawn();
            HelmetManager.Instance.DestroyHelmetClientRpc(clientId);
        }
    }
}
