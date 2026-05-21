using System;
using Runtime.Data.UnityObjects;

namespace Runtime.Interfaces
{
    public interface ISpinManager
    {
        event Action<RewardEntry> OnSpinCompleted;
        event Action OnBombHit;
        void Spin();
    }
}