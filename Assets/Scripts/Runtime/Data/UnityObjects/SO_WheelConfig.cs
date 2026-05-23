using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_WheelConfig", menuName = "Config/WheelConfig")]
    public class SO_WheelConfig : ScriptableObject
    {
        public Sprite baseSprite;
        public Sprite indicatorSprite;
        public RewardSet[] rewardSets;
    }
}