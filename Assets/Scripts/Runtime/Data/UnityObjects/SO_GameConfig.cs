using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(menuName = "Config/GameConfig")]
    public class SO_GameConfig : ScriptableObject
    {
        public int silverZoneInterval = 5;
        public int goldZoneInterval = 30;
        public SO_WheelConfig normalWheel;
        public SO_WheelConfig silverWheel;
        public SO_WheelConfig goldWheel;
        public SO_StageConfig stageConfig;
    }
}