using UnityEngine;

namespace Runtime.Utility
{
    public class FrameRateSetter : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 90;
        }
    }
}