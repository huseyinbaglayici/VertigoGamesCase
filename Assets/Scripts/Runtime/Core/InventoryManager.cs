using System;
using System.Collections.Generic;
using System.Linq;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using Zenject;

namespace Runtime.Core
{
    public class InventoryManager : IInventoryManager, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly List<ItemData> _items = new();

        public event Action<ItemData, bool> OnItemAdded;

        public InventoryManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<GameRestartSignal>(OnReset);
        }

        public void Dispose() => _signalBus.Unsubscribe<GameRestartSignal>(OnReset);

        public void AddItem(ItemData item)
        {
            var existingItem = _items.FirstOrDefault(x => x.Config == item.Config);
            if (existingItem != null)
            {
                existingItem.Amount += item.Amount;
                OnItemAdded?.Invoke(existingItem, false);
            }
            else
            {
                _items.Add(item);
                OnItemAdded?.Invoke(item, true);
            }
        }

        public IReadOnlyList<ItemData> GetItems() => _items;

        public int GetTotalCurrency()
        {
            int total = 0;
            foreach (var item in _items)
                if (item.Config.IsCurrency)
                    total += item.Amount;
            return total;
        }

        private void OnReset() => _items.Clear();
    }
}