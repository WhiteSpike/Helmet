using BepInEx;
using BepInEx.Logging;
using Helmet.Behaviour;
using Helmet.Misc;
using HarmonyLib;
using LethalLib.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Helmet.Patches;
namespace Helmet
{
    [BepInPlugin(Metadata.GUID,Metadata.NAME,Metadata.VERSION)]
    [BepInDependency("com.sigurd.csync")]
    [BepInDependency("evaisa.lethallib")]
    public class Plugin : BaseUnityPlugin
    {
        internal static readonly Harmony harmony = new(Metadata.GUID);
        internal static readonly ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(Metadata.NAME);
        internal static GameObject helmetModel;
        internal static GameObject helmet;
        internal static GameObject networkPrefab;
        internal static AudioClip breakSFX;
        internal static AudioClip damageSFX;

        public new static PluginConfig Config;

        void Awake()
        {
            Config = new PluginConfig(base.Config);

            // netcode patching stuff
            IEnumerable<Type> types;
            try
            {
                types = Assembly.GetExecutingAssembly().GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
            string assetDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "whitespike.helmet");
            AssetBundle bundle = AssetBundle.LoadFromFile(assetDir);
            string root = "Assets/Helmet/";

            helmetModel = bundle.LoadAsset<GameObject>(root + "HelmetModel.prefab");
            breakSFX = bundle.LoadAsset<AudioClip>(root + "breakWood.mp3");
            damageSFX = bundle.LoadAsset<AudioClip>(root + "bonk.mp3");

            Item helmetItem = ScriptableObject.CreateInstance<Item>();
            helmetItem.name = "HelmetItemProperties";
            helmetItem.allowDroppingAheadOfPlayer = Config.DROP_AHEAD_PLAYER;
            helmetItem.canBeGrabbedBeforeGameStart = Config.GRABBED_BEFORE_START;
            helmetItem.canBeInspected = false;
            helmetItem.creditsWorth = Config.PRICE;
            helmetItem.restingRotation = new Vector3(0f, 0f, 0f);
            helmetItem.rotationOffset = new Vector3(90f, 90f, 0f);
            helmetItem.positionOffset = new Vector3(-0.25f, 0f, 0f);
            helmetItem.weight = 1f + (Config.WEIGHT / 100f);
            helmetItem.twoHanded = false;
            helmetItem.itemIcon = bundle.LoadAsset<Sprite>(root + "Icon.png");
            helmetItem.spawnPrefab = bundle.LoadAsset<GameObject>(root + "Helmet.prefab");
            helmetItem.highestSalePercentage = Config.HIGHEST_SALE_PERCENTAGE;
            helmetItem.itemName = HelmetBehaviour.ITEM_NAME;
            helmetItem.itemSpawnsOnGround = true;
            helmetItem.isConductiveMetal = Config.CONDUCTIVE;
            helmetItem.requiresBattery = false;
            helmetItem.batteryUsage = 0f;
            helmetItem.syncInteractLRFunction = false;
            helmetItem.syncGrabFunction = false;
            helmetItem.syncUseFunction = false;
            helmetItem.syncDiscardFunction = false;

            HelmetBehaviour grabbableObject = helmetItem.spawnPrefab.AddComponent<HelmetBehaviour>();
            grabbableObject.itemProperties = helmetItem;
            grabbableObject.grabbable = true;
            grabbableObject.grabbableToEnemies = true;
            NetworkPrefabs.RegisterNetworkPrefab(helmetItem.spawnPrefab);
            helmet = helmetItem.spawnPrefab;

            TerminalNode infoNode = SetupInfoNode();
            Items.RegisterShopItem(shopItem: helmetItem, itemInfo: infoNode, price: helmetItem.creditsWorth);

            networkPrefab = LethalLib.Modules.NetworkPrefabs.CreateNetworkPrefab("Helmet Manager");
            networkPrefab.AddComponent<HelmetManager>();
            PatchMainVersion();
            mls.LogInfo($"{Metadata.NAME} {Metadata.VERSION} has been loaded successfully.");
        }
        internal static TerminalNode SetupInfoNode()
        {
            TerminalNode infoNode = ScriptableObject.CreateInstance<TerminalNode>();
            infoNode.displayText += GetDisplayInfo() + "\n";
            infoNode.clearPreviousText = true;
            return infoNode;
        }
        public static string GetDisplayInfo()
        {
            return string.Format("Blocks all incoming damage, has {0} durability.\n\n", Config.HITS_BLOCKED.Value);
        }
        internal static void PatchMainVersion()
        {
            PatchVitalComponents();
        }
        static void PatchVitalComponents()
        {
            harmony.PatchAll(typeof(StartOfRoundPatcher));
            harmony.PatchAll(typeof(PlayerControllerBPatcher));
            harmony.PatchAll(typeof(GameNetworkManagerPatcher));
            mls.LogInfo("Game managers have been patched");
        }
    }   
}
