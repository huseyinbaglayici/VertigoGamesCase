using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [System.Serializable]
    public class RewardEntry
    {
        public SO_ItemConfig item;
        [Min(0)] public int minAmount;
        [Min(0)] public int maxAmount;
    }
}