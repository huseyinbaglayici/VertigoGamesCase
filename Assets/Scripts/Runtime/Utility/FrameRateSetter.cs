using UnityEngine;

namespace Runtime.Utility
{
    public class FrameRateSetter : MonoBehaviour
    {
        private const int TargetFrameRate = 90;

        private void Awake()
        {
            Application.targetFrameRate = TargetFrameRate;
        }
    }
}
