using Helmet.Behaviour;
namespace Helmet.Misc.Util
{
    internal static class Constants
    {
        internal const string ITEM_SCAN_NODE_KEY_FORMAT = "Enable scan node of {0}";
        internal const bool ITEM_SCAN_NODE_DEFAULT = true;
        internal const string ITEM_SCAN_NODE_DESCRIPTION = "Shows a scan node on the item when scanning";

        internal const string HELMET_PRICE_KEY = $"{HelmetBehaviour.ITEM_NAME} price";
        internal const int HELMET_PRICE_DEFAULT = 300;
        internal const string HELMET_PRICE_DESCRIPTION = $"Price for {HelmetBehaviour.ITEM_NAME}.";

        internal const string HELMET_WEIGHT_KEY = "Item weight";
        internal const int HELMET_WEIGHT_DEFAULT = 65;
        internal const string HELMET_WEIGHT_DESCRIPTION = "Weight (in lbs)";

        internal const string HELMET_CONDUCTIVE_KEY = "Conductive";
        internal const bool HELMET_CONDUCTIVE_DEFAULT = true;
        internal const string HELMET_CONDUCTIVE_DESCRIPTION = "Wether it attracts lightning to the item or not. (Or other mechanics that rely on item being conductive)";

        internal const string HELMET_DROP_AHEAD_PLAYER_KEY = "Drop ahead of player when dropping";
        internal const bool HELMET_DROP_AHEAD_PLAYER_DEFAULT = false;
        internal const string HELMET_DROP_AHEAD_PLAYER_DESCRIPTION = "If on, the item will drop infront of the player. Otherwise, drops underneath them and slightly infront.";

        internal const string HELMET_GRABBED_BEFORE_START_KEY = "Grabbable before game start";
        internal const bool HELMET_GRABBED_BEFORE_START_DEFAULT = false;
        internal const string HELMET_GRABBED_BEFORE_START_DESCRIPTION = "Allows wether the item can be grabbed before hand or not";

        internal const string HELMET_HIGHEST_SALE_PERCENTAGE_KEY = "Highest Sale Percentage";
        internal const int HELMET_HIGHEST_SALE_PERCENTAGE_DEFAULT = 50;
        internal const string HELMET_HIGHEST_SALE_PERCENTAGE_DESCRIPTION = "Maximum percentage of sale allowed when this item is selected for a sale.";

        internal const string HELMET_AMOUNT_OF_HITS_KEY = $"Amount of hits blocked by {HelmetBehaviour.ITEM_NAME}";
        internal const int HELMET_AMOUNT_OF_HITS_DEFAULT = 3;
        internal const string HELMET_AMOUNT_OF_HITS_DESCRIPTION = "Only valid when TotalPerHit damage mitigation mode is selected.";

        internal const string HELMET_DAMAGE_REDUCTION_KEY = $"Damage reduction (%) when wearing a {HelmetBehaviour.ITEM_NAME}";
        internal const int HELMET_DAMAGE_REDUCTION_DEFAULT = 20;
        internal const string HELMET_DAMAGE_REDUCTION_DESCRIPTION = "Only valid when PartialTilLowHealth damage mitigation mode is selected.";

        internal const string HELMET_DAMAGE_MITIGATION_MODE_KEY = $"Damage mitigation mode selected when wearing a {HelmetBehaviour.ITEM_NAME}";
        internal const string HELMET_DAMAGE_MITIGATION_MODE_DEFAULT = "TotalPerHit";
        internal const string HELMET_DAMAGE_MITIGATION_MODE_DESCRIPTION = "Accepted Values: TotalPerHit (full immunity and helmet takes hits on durability), PartialTilLowHealth (partial immunity and helmet breaks when low health)";

        internal static readonly string HELMET_SCAN_NODE_KEY = string.Format(ITEM_SCAN_NODE_KEY_FORMAT, HelmetBehaviour.ITEM_NAME);
    }
}
