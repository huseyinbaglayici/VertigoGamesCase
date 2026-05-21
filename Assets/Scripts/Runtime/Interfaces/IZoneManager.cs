using Runtime.Data.UnityObjects;
using Runtime.Enums;

namespace Runtime.Interfaces
{
    public interface IZoneManager
    {
        int CurrentZone { get; }
        ZoneType GetZoneType(int zone);
        SO_WheelConfig GetCurrentWheelConfig();
        void AdvanceZone();
    }
}