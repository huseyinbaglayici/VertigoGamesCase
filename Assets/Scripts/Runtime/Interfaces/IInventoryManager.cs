using System;
using System.Collections.Generic;
using Runtime.Data.ValueObjects;

namespace Runtime.Interfaces
{
    public interface IInventoryManager
    {
        event Action<ItemData, bool> OnItemAdded;
        void AddItem(ItemData item);
        IReadOnlyList<ItemData> GetItems();
        int GetTotalCurrency();
    }
}