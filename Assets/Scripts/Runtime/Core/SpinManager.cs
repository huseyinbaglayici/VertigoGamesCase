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

        private RewardEntry _pendingResult;

        public event Action<int, RewardEntry> OnSpinDecision;
        public event Action<RewardEntry> OnSpinCompleted;
        public event Action OnBombHit;
        public event Action OnGameResumed;

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
            int index = _random.Next(0, rewardSet.rewards.Length);
            _pendingResult = rewardSet.rewards[index];
            OnSpinDecision?.Invoke(index, _pendingResult);
        }

        public void Continue() => OnGameResumed?.Invoke();

        public void CommitSpinResult()
        {
            if (_pendingResult == null) return;

            var result = _pendingResult;
            _pendingResult = null;

            if (result.item.isBomb)
            {
                OnBombHit?.Invoke();
                return;
            }

            var amount = _rewardCalculator.Calculate(result.minAmount, result.maxAmount, _zoneManager.CurrentZone);
            _inventoryManager.AddItem(new ItemData(result.item, amount));
            _zoneManager.AdvanceZone();
            OnSpinCompleted?.Invoke(result);
        }
    }
}
