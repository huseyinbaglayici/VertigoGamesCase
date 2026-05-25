using UnityEngine;
using Zenject;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(menuName = "Config/GameConfig")]
    public class SO_GameConfig : ScriptableObjectInstaller
    {
        public int silverZoneInterval = 5;
        public int goldZoneInterval = 30;
        public int continueCost = 500;
        public SO_WheelConfig normalWheel;
        public SO_WheelConfig silverWheel;
        public SO_GoldWheelConfig goldWheel;
        public SO_StageConfig stageConfig;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
    }
}