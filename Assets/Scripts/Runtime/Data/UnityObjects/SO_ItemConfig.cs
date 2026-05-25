using Runtime.Enums;
using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_ItemConfig", menuName = "Config/ItemConfig")]
    public class SO_ItemConfig : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public ItemType itemType;

        public bool IsBomb => itemType == ItemType.Bomb;
        public bool IsCurrency => itemType == ItemType.Currency;
    }
}