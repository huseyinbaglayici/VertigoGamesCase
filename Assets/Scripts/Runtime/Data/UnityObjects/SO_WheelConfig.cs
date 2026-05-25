using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_WheelConfig", menuName = "Config/WheelConfig")]
    public class SO_WheelConfig : ScriptableObject
    {
        public Sprite baseSprite;
        public Sprite indicatorSprite;
        public RewardSet[] rewardSets;
        public SO_ItemConfig specialItem;

#if UNITY_EDITOR
        [Header("Auto Generation")]
        [Tooltip("Slot düzenini ve baz miktarları buradan ayarla. Bomb slotlarının miktarları korunmaz (0 kalır).")]
        public RewardEntry[] preset;
        public int rewardSetCount = 5;
        [Min(1f)] public float maxMultiplier = 3f;

        [ContextMenu("Generate Reward Sets")]
        private void GenerateRewardSets()
        {
            if (preset == null || preset.Length == 0)
            {
                Debug.LogError("Preset boş.", this);
                return;
            }

            rewardSets = new RewardSet[rewardSetCount];

            for (int s = 0; s < rewardSetCount; s++)
            {
                float t = rewardSetCount > 1 ? (float)s / (rewardSetCount - 1) : 0f;
                float multiplier = Mathf.Lerp(1f, maxMultiplier, t);

                var entries = new RewardEntry[preset.Length];
                for (int i = 0; i < preset.Length; i++)
                {
                    bool isBomb = preset[i].item != null && preset[i].item.isBomb;
                    entries[i] = new RewardEntry
                    {
                        item = preset[i].item,
                        minAmount = isBomb ? 0 : Mathf.RoundToInt(preset[i].minAmount * multiplier),
                        maxAmount = isBomb ? 0 : Mathf.RoundToInt(preset[i].maxAmount * multiplier),
                    };
                }

                rewardSets[s] = new RewardSet { rewards = entries };
            }

            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log($"{rewardSetCount} reward set üretildi → {name}", this);
        }
#endif
    }
}
