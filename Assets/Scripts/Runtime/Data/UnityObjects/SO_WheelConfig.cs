using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_WheelConfig", menuName = "Config/WheelConfig")]
    public class SO_WheelConfig : ScriptableObject
    {
        public RewardSet[] rewardSets;
    }

    [System.Serializable]
    public class RewardSet
    {
        public RewardEntry[] rewards;
    }

    [System.Serializable]
    public class RewardEntry
    {
        public SO_ItemConfig item;
        public int minAmount;
        public int maxAmount;
    }
}