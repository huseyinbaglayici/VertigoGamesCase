using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "SO_GoldWheelConfig", menuName = "Config/GoldWheelConfig")]
    public class SO_GoldWheelConfig : SO_WheelConfig
    {
        public SO_ItemConfig specialItem;
    }
}