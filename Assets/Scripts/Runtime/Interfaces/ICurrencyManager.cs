using System;

namespace Runtime.Interfaces
{
    public interface ICurrencyManager
    {
        int Currency { get; }
        event Action<int> OnCurrencyChanged;
        void Collect(int amount);
        bool TrySpend(int amount);
    }
}