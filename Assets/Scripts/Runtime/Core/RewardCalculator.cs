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
            int divisor = _gameConfig.goldZoneInterval - 1;
            if (divisor <= 0) return maxAmount;
            float t = Math.Clamp((float)(currentZone - 1) / divisor, 0f, 1f);
            return (int)Math.Round(minAmount + (maxAmount - minAmount) * t);
        }
    }
}