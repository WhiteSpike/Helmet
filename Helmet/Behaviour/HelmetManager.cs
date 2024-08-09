using GameNetcodeStuff;
using LethalLib.Modules;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Helmet.Behaviour
{
    internal class HelmetManager : NetworkBehaviour
    {
        internal Dictionary<ulong, bool> clientWearingHelmet = [];
        internal bool wearingHelmet;
        internal int helmetHits;
        internal static HelmetManager Instance;

        void Start()
        {
            Instance = this;
        }
        public void DestroyHelmet(ulong id)
        {
            if (StartOfRound.Instance.allPlayerObjects.Length <= (int)id)
            {
                Plugin.mls.LogError($"ID: {id} can not be used to index allPlayerObjects! (Length: {StartOfRound.Instance.allPlayerObjects.Length})");
                return;
            }

            GameObject player = StartOfRound.Instance.allPlayerObjects[(int)id];
            if (player == null)
            {
                Plugin.mls.LogError("Player is with helmet is null!");
                return;
            }
            if (player.GetComponent<PlayerControllerB>().IsOwner) return; // owner doesn't have model

            Transform helmet = player.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2).Find("HelmetModel(Clone)");
            if (helmet == null)
            {
                Plugin.mls.LogError("Unable to find 'HelmetModel(Clone) on player!");
                return;
            }
            Destroy(helmet.gameObject); // this isn't a netobj hence the client side destruction
            if (IsServer)
            {
                PlayerControllerB playerController = player.GetComponent<PlayerControllerB>();
                clientWearingHelmet[playerController.actualClientId] = true;
            }
        }
        [ClientRpc]
        public void DestroyHelmetClientRpc(ulong id)
        {
            DestroyHelmet(id);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReqDestroyHelmetServerRpc(ulong id)
        {
            Plugin.mls.LogInfo($"Instructing clients to destroy helmet of player : {id}");
            DestroyHelmetClientRpc(id);
        }

        [ClientRpc]
        internal void SpawnAndMoveHelmetClientRpc(NetworkObjectReference netRef, ulong id)
        {
            netRef.TryGet(out NetworkObject obj);
            if (obj == null || obj.IsOwner)
            {
                if (obj == null) Plugin.mls.LogError("Failed to resolve network object ref in SpawnAndMoveHelmetClientRpc!");
                else Plugin.mls.LogInfo("This client owns the helmet, skipping cosmetic instantiation.");
                return;
            }
            Transform head = obj.transform.GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(2); // phenomenal
            GameObject go = Instantiate(Plugin.helmetModel, head);
            go.transform.localPosition = new Vector3(0.01f, 0.1f, 0.08f);
            Plugin.mls.LogInfo($"Successfully spawned helmet cosmetic for player: {id}");
            if (IsServer)
            {
                PlayerControllerB player = obj.GetComponent<PlayerControllerB>();
                clientWearingHelmet[player.actualClientId] = true;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void ReqSpawnAndMoveHelmetServerRpc(NetworkObjectReference netRef, ulong id)
        {
            Plugin.mls.LogInfo($"Instructing clients to spawn helmet on player : {id}");
            netRef.TryGet(out NetworkObject obj);
            if (obj == null || obj.IsOwner)
            {
                if (obj == null) Plugin.mls.LogError("Failed to resolve network object ref in SpawnAndMoveHelmetClientRpc!");
                else Plugin.mls.LogInfo("This client owns the helmet, skipping cosmetic instantiation.");
                return;
            }
            PlayerControllerB player = obj.GetComponent<PlayerControllerB>();
            clientWearingHelmet[player.actualClientId] = true;
            SpawnAndMoveHelmetClientRpc(netRef, id);
        }

        [ClientRpc]
        internal void PlayAudioOnPlayerClientRpc(NetworkBehaviourReference netRef, bool breakHelmet)
        {
            netRef.TryGet(out PlayerControllerB player);
            if (player == null)
            {
                Plugin.mls.LogError("Unable to resolve network behaviour reference in PlayAudioOnPlayerClientRpc!");
                return;
            }
            player.GetComponentInChildren<AudioSource>().PlayOneShot(breakHelmet ? Plugin.breakSFX : Plugin.damageSFX);
            if (breakHelmet && IsServer) clientWearingHelmet[player.actualClientId] = false;
        }

        [ServerRpc(RequireOwnership = false)]
        internal void ReqPlayAudioOnPlayerServerRpc(NetworkBehaviourReference netRef, bool breakHelmet)
        {
            PlayAudioOnPlayerClientRpc(netRef, breakHelmet);
        }
    }
}
