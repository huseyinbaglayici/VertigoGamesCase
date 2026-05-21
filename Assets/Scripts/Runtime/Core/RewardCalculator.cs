using System;
using Runtime.Data.UnityObjects;

namespace Runtime.Core
{
    public class RewardCalculator
    {
        private readonly SO_GameConfig _gameConfig;

        public RewardCalculator(SO_GameConfig gameConfig)
        {
            _gameConfig = gameConfig;
        }

        public int Calculate(int minAmount, int maxAmount, int currentZone)
        {
            float t = (float)(currentZone - 1) / (_gameConfig.goldZoneInterval - 1);
            t = Math.Clamp(t, 0, 1);
            return (int)Math.Round(minAmount + (maxAmount - minAmount) * t);
        }
    }
}