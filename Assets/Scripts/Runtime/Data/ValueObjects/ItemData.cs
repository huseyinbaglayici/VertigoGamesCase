using Runtime.Data.UnityObjects;

namespace Runtime.Data.ValueObjects
{
    public class ItemData
    {
        public SO_ItemConfig Config { get; }
        public int Amount { get; set; }

        public ItemData(SO_ItemConfig config, int amount)
        {
            Config = config;
            Amount = amount;
        }
    }
}