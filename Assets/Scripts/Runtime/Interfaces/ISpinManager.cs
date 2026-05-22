using System;
using Runtime.Data.UnityObjects;

namespace Runtime.Interfaces
{
    public interface ISpinManager
    {
        event Action<int, RewardEntry> OnSpinDecision;
        event Action<RewardEntry> OnSpinCompleted;
        event Action OnBombHit;
        event Action OnGameResumed;
        void Spin();
        void CommitSpinResult();
        void Continue();
    }
}
