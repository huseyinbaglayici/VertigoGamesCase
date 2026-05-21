using System;
using Runtime.Data.UnityObjects;
using Runtime.Enums;

namespace Runtime.Interfaces
{
    public interface IZoneManager
    {
        int CurrentZone { get; }
        ZoneType GetZoneType(int zone);

        event Action<int> OnZoneChanged;

        SO_WheelConfig GetCurrentWheelConfig();
        void AdvanceZone();
    }
}