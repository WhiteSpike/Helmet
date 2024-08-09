using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using Helmet.Behaviour;
using Helmet.Misc.Util;
using System.Runtime.Serialization;

namespace Helmet.Misc
{
    [DataContract]
    public class PluginConfig : SyncedConfig2<PluginConfig>
    {
        [field: SyncedEntryField] public SyncedEntry<bool> SCAN_NODE { get; set; }
        [field: SyncedEntryField] public SyncedEntry<int> PRICE { get; set; }
        [field: SyncedEntryField] public SyncedEntry<int> WEIGHT { get; set; }
        [field: SyncedEntryField] public SyncedEntry<bool> TWO_HANDED { get; set; }
        [field: SyncedEntryField] public SyncedEntry<bool> DROP_AHEAD_PLAYER { get; set; }
        [field: SyncedEntryField] public SyncedEntry<bool> GRABBED_BEFORE_START { get; set; }
        [field: SyncedEntryField] public SyncedEntry<bool> CONDUCTIVE { get; set; }
        [field: SyncedEntryField] public SyncedEntry<int> HIGHEST_SALE_PERCENTAGE { get; set; }
        [field: SyncedEntryField] public SyncedEntry<int> DAMAGE_REDUCTION { get; set; }
        [field: SyncedEntryField] public SyncedEntry<string> DAMAGE_MITIGATION_MODE { get; set; }
        [field: SyncedEntryField] public SyncedEntry<int> HITS_BLOCKED { get; set; }
        public PluginConfig(ConfigFile cfg) : base(Metadata.GUID)
        {
            string topSection = HelmetBehaviour.ITEM_NAME;

            PRICE = cfg.BindSyncedEntry(topSection, Constants.HELMET_PRICE_KEY, Constants.HELMET_PRICE_DEFAULT, Constants.HELMET_PRICE_DESCRIPTION);
            WEIGHT = cfg.BindSyncedEntry(topSection, Constants.HELMET_WEIGHT_KEY, Constants.HELMET_WEIGHT_DEFAULT, Constants.HELMET_WEIGHT_DESCRIPTION);
            TWO_HANDED = cfg.BindSyncedEntry(topSection, Constants.HELMET_TWO_HANDED_KEY, Constants.HELMET_TWO_HANDED_DEFAULT, Constants.HELMET_TWO_HANDED_DESCRIPTION);
            SCAN_NODE = cfg.BindSyncedEntry(topSection, Constants.HELMET_SCAN_NODE_KEY, Constants.ITEM_SCAN_NODE_DEFAULT, Constants.ITEM_SCAN_NODE_DESCRIPTION);
            DROP_AHEAD_PLAYER = cfg.BindSyncedEntry(topSection, Constants.HELMET_DROP_AHEAD_PLAYER_KEY, Constants.HELMET_DROP_AHEAD_PLAYER_DEFAULT, Constants.HELMET_DROP_AHEAD_PLAYER_DESCRIPTION);
            CONDUCTIVE = cfg.BindSyncedEntry(topSection, Constants.HELMET_CONDUCTIVE_KEY, Constants.HELMET_CONDUCTIVE_DEFAULT, Constants.HELMET_CONDUCTIVE_DESCRIPTION);
            GRABBED_BEFORE_START = cfg.BindSyncedEntry(topSection, Constants.HELMET_GRABBED_BEFORE_START_KEY, Constants.HELMET_GRABBED_BEFORE_START_DEFAULT, Constants.HELMET_GRABBED_BEFORE_START_DESCRIPTION);
            HIGHEST_SALE_PERCENTAGE = cfg.BindSyncedEntry(topSection, Constants.HELMET_HIGHEST_SALE_PERCENTAGE_KEY, Constants.HELMET_HIGHEST_SALE_PERCENTAGE_DEFAULT, Constants.HELMET_HIGHEST_SALE_PERCENTAGE_DESCRIPTION);
            HITS_BLOCKED = cfg.BindSyncedEntry(topSection, Constants.HELMET_AMOUNT_OF_HITS_KEY, Constants.HELMET_AMOUNT_OF_HITS_DEFAULT);
            DAMAGE_REDUCTION = cfg.BindSyncedEntry(topSection, Constants.HELMET_DAMAGE_REDUCTION_KEY, Constants.HELMET_DAMAGE_REDUCTION_DEFAULT);
            DAMAGE_MITIGATION_MODE = cfg.BindSyncedEntry(topSection, Constants.HELMET_DAMAGE_MITIGATION_MODE_KEY, Constants.HELMET_DAMAGE_MITIGATION_MODE_DEFAULT, Constants.HELMET_DAMAGE_MITIGATION_MODE_DESCRIPTION);

            ConfigManager.Register(this);
        }
    }
}
