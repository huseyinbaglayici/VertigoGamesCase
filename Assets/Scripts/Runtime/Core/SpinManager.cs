using System;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;

namespace Runtime.Core
{
    public class SpinManager : ISpinManager
    {
        private readonly IZoneManager _zoneManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly System.Random _random;
        private readonly RewardCalculator _rewardCalculator;

        public event Action<RewardEntry> OnSpinCompleted;
        public event Action OnBombHit;

        public SpinManager(IZoneManager zoneManager, IInventoryManager inventoryManager, System.Random random,
            RewardCalculator rewardCalculator)
        {
            _zoneManager = zoneManager;
            _inventoryManager = inventoryManager;
            _random = random;
            _rewardCalculator = rewardCalculator;
        }

        public void Spin()
        {
            var config = _zoneManager.GetCurrentWheelConfig();
            var rewardSet = _zoneManager.GetCurrentRewardSet(config);
            var result = rewardSet.rewards[_random.Next(0, rewardSet.rewards.Length)];

            if (result.item.isBomb)
            {
                OnBombHit?.Invoke();
                return;
            }

            var amount = _rewardCalculator.Calculate(result.minAmount, result.maxAmount, _zoneManager.CurrentZone);
            _inventoryManager.AddItem(new ItemData(result.item, amount));
            OnSpinCompleted?.Invoke(result);
        }

    }
}