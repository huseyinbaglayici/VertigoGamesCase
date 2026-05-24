using System;
using Runtime.Interfaces;

namespace Runtime.Core
{
    public class CurrencyManager : ICurrencyManager
    {
        public int Currency { get; private set; }
        public event Action<int> OnCurrencyChanged;

        public void Collect(int amount)
        {
            Currency += amount;
            OnCurrencyChanged?.Invoke(Currency);
        }

        public bool TrySpend(int amount)
        {
            if (Currency < amount) return false;
            Currency -= amount;
            OnCurrencyChanged?.Invoke(Currency);
            return true;
        }
    }
}
