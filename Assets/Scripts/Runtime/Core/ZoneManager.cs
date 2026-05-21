using System;
using Runtime.Data.UnityObjects;
using Runtime.Enums;
using Runtime.Interfaces;

namespace Runtime.Core
{
    public class ZoneManager : IZoneManager
    {
        private readonly SO_GameConfig _gameConfig;
        private int _currentZone = 1;

        public int CurrentZone => _currentZone;
        public event Action<int> OnZoneChanged;

        public ZoneManager(SO_GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
        }

        public ZoneType GetZoneType(int zone)
        {
            if (zone % _gameConfig.goldZoneInterval == 0) return ZoneType.Gold;
            if (zone == 1 || zone % _gameConfig.silverZoneInterval == 0) return ZoneType.Silver;
            return ZoneType.Normal;
        }

        public SO_WheelConfig GetCurrentWheelConfig()
        {
            return GetZoneType(_currentZone) switch
            {
                ZoneType.Gold => _gameConfig.goldWheel,
                ZoneType.Silver => _gameConfig.silverWheel,
                _ => _gameConfig.normalWheel
            };
        }

        public void AdvanceZone()
        {
            _currentZone++;
            OnZoneChanged?.Invoke(_currentZone);
        }
    }
}