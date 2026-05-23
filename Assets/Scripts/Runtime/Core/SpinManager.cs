using System;
using System.Collections.Generic;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using Zenject;

namespace Runtime.Core
{
    public class SpinManager : ISpinManager, IDisposable
    {
        private readonly IZoneManager _zoneManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly System.Random _random;
        private readonly RewardCalculator _rewardCalculator;
        private readonly SignalBus _signalBus;

        private RewardEntry _pendingResult;
        private int _pendingSlotIndex;
        private readonly HashSet<int> _excludedSlots = new HashSet<int>();

        public event Action<int, RewardEntry> OnSpinDecision;
        public event Action<RewardEntry> OnSpinCompleted;
        public event Action OnBombHit;
        public event Action OnGameResumed;
        public event Action OnRewardsRequested;

        public SpinManager(IZoneManager zoneManager, IInventoryManager inventoryManager, System.Random random,
            RewardCalculator rewardCalculator, SignalBus signalBus)
        {
            _zoneManager = zoneManager;
            _inventoryManager = inventoryManager;
            _random = random;
            _rewardCalculator = rewardCalculator;
            _signalBus = signalBus;
            _signalBus.Subscribe<GameRestartSignal>(OnReset);
        }

        public void Dispose() => _signalBus.Unsubscribe<GameRestartSignal>(OnReset);

        public void Spin()
        {
            var config = _zoneManager.GetCurrentWheelConfig();
            var rewardSet = _zoneManager.GetCurrentRewardSet(config);

            int index = _excludedSlots.Count > 0
                ? PickIndexExcludingSlots(rewardSet.rewards)
                : _random.Next(0, rewardSet.rewards.Length);

            _pendingSlotIndex = index;
            _pendingResult = rewardSet.rewards[index];
            OnSpinDecision?.Invoke(index, _pendingResult);
        }

        private int PickIndexExcludingSlots(RewardEntry[] rewards)
        {
            var pool = new List<int>(rewards.Length);
            for (int i = 0; i < rewards.Length; i++)
            {
                if (!_excludedSlots.Contains(i)) pool.Add(i);
            }
            return pool[_random.Next(0, pool.Count)];
        }

        public void Continue()
        {
            _excludedSlots.Add(_pendingSlotIndex);
            OnGameResumed?.Invoke();
        }

        public void RequestRewards() => OnRewardsRequested?.Invoke();

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

            _excludedSlots.Clear();
            var amount = _rewardCalculator.Calculate(result.minAmount, result.maxAmount, _zoneManager.CurrentZone);
            _inventoryManager.AddItem(new ItemData(result.item, amount));
            _zoneManager.AdvanceZone();
            OnSpinCompleted?.Invoke(result);
        }

        private void OnReset()
        {
            _pendingResult = null;
            _excludedSlots.Clear();
        }
    }
}