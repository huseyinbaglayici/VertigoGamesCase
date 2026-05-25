using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_ItemConfig", menuName = "Config/ItemConfig")]
    public class SO_ItemConfig : ScriptableObject
    {
        public string itemName;
        public Sprite icon;
        public bool isBomb;
        public bool isCurrency;
        public bool isSpecial;
    }
}