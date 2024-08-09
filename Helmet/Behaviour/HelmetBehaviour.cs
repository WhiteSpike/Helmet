using CustomItemBehaviourLibrary.AbstractItems;
using CustomItemBehaviourLibrary.Misc;
using GameNetcodeStuff;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Helmet.Behaviour
{
    internal class HelmetBehaviour : GrabbableObject
    {
        internal const string ITEM_NAME = "Helmet";

        protected bool KeepScanNode
        {
            get
            {
                return Plugin.Config.SCAN_NODE;
            }
        }
        internal enum DamageMitigationMode
        {
            TotalPerHit,
            PartialTilLowHealth,
        }

        public override void Start()
        {
            base.Start();
            if (!KeepScanNode) Destroy(gameObject.GetComponentInChildren<ScanNodeProperties>());
        }

        public override void ItemActivate(bool used, bool buttonDown = true)
        {
            base.ItemActivate(used, buttonDown);
            if (buttonDown)
            {
                if (HelmetManager.Instance.wearingHelmet) return;
                HelmetManager.Instance.wearingHelmet = true;
                HelmetManager.Instance.helmetHits = Plugin.Config.HITS_BLOCKED.Value;
                if (IsHost) HelmetManager.Instance.SpawnAndMoveHelmetClientRpc(new NetworkObjectReference(playerHeldBy.GetComponent<NetworkObject>()), playerHeldBy.playerSteamId);
                else HelmetManager.Instance.ReqSpawnAndMoveHelmetServerRpc(new NetworkObjectReference(playerHeldBy.GetComponent<NetworkObject>()), playerHeldBy.playerSteamId);
                playerHeldBy.DespawnHeldObject();
            }
        }

        internal static void ExecuteHelmetDamageMitigation(ref PlayerControllerB player, ref int damageNumber)
        {
            DamageMitigationMode pickedMode = DamageMitigationMode.TotalPerHit;
            Enum.TryParse(typeof(DamageMitigationMode), Plugin.Config.DAMAGE_MITIGATION_MODE.Value, out object parsedRestriction);
            if (parsedRestriction == null)
                Plugin.mls.LogError($"An error occured parsing the damage mitigation mode ({Plugin.Config.DAMAGE_MITIGATION_MODE.Value}), defaulting to TotalPerHit");
            else
                pickedMode = (DamageMitigationMode)parsedRestriction;
            switch (pickedMode)
            {
                case DamageMitigationMode.TotalPerHit: ExecuteHelmetCompleteMitigation(ref player, ref damageNumber); break;
                case DamageMitigationMode.PartialTilLowHealth: ExecuteHelmetPartialMitigation(ref player, ref damageNumber); break;
            }
        }

        internal static void ExecuteHelmetCompleteMitigation(ref PlayerControllerB player, ref int damageNumber)
        {
            Plugin.mls.LogDebug($"Player {player.playerUsername} is wearing a helmet, executing helmet logic...");
            HelmetManager.Instance.helmetHits--;
            if (HelmetManager.Instance.helmetHits <= 0)
            {
                Plugin.mls.LogDebug("Helmet has ran out of durability, breaking the helmet...");
                BreakHelmet(ref player);
            }
            else
            {
                Plugin.mls.LogDebug($"Helmet still has some durability ({HelmetManager.Instance.helmetHits}), decreasing it...");
                HitHelmet(ref player);
            }
            damageNumber = 0;
        }

        internal static void ExecuteHelmetPartialMitigation(ref PlayerControllerB player, ref int damageNumber)
        {
            int health = player.health;
            int updatedHealth = health - Mathf.CeilToInt(damageNumber * Mathf.Clamp((100f - Plugin.Config.DAMAGE_REDUCTION) / 100f, 0f, damageNumber));
            if (updatedHealth > 0)
            {
                HitHelmet(ref player);
                damageNumber = Mathf.CeilToInt(damageNumber * ((100f - Plugin.Config.DAMAGE_REDUCTION) / 100f));
            }
            else
            {
                BreakHelmet(ref player);
                damageNumber = 0;
            }
        }
        internal static void HitHelmet(ref PlayerControllerB player)
        {
            if (player.IsHost || player.IsServer) HelmetManager.Instance.PlayAudioOnPlayerClientRpc(new NetworkBehaviourReference(player), breakHelmet: false);
            else HelmetManager.Instance.ReqPlayAudioOnPlayerServerRpc(new NetworkBehaviourReference(player), breakHelmet: false);
        }
        internal static void BreakHelmet(ref PlayerControllerB player)
        {
            HelmetManager.Instance.wearingHelmet = false;
            if (player.IsHost || player.IsServer) HelmetManager.Instance.DestroyHelmetClientRpc(player.playerClientId);
            else HelmetManager.Instance.ReqDestroyHelmetServerRpc(player.playerClientId);
            if (player.IsHost || player.IsServer) HelmetManager.Instance.PlayAudioOnPlayerClientRpc(new NetworkBehaviourReference(player), breakHelmet: true);
            else HelmetManager.Instance.ReqPlayAudioOnPlayerServerRpc(new NetworkBehaviourReference(player), breakHelmet: true);
        }
    }
}
