using UnityEngine;

namespace Runtime.Utility
{
    public static class HapticFeedback
    {
        public enum HapticType
        {
            Light,
            Medium,
            Heavy
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private const int MinApiLevelForVibrationEffect = 26;

        private const int LightDurationMs = 12;
        private const int MediumDurationMs = 40;
        private const int HeavyDurationMs = 80;

        private const int LightAmplitude = 35;
        private const int MediumAmplitude = 150;
        private const int HeavyAmplitude = 255;

        private const int UninitializedApiLevel = -1;

        private static AndroidJavaObject _vibrator;
        private static int _apiLevel = UninitializedApiLevel;

        private static AndroidJavaObject Vibrator
        {
            get
            {
                if (_vibrator != null) return _vibrator;
                using var player = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                return _vibrator;
            }
        }

        private static int ApiLevel
        {
            get
            {
                if (_apiLevel >= 0) return _apiLevel;
                using var version = new AndroidJavaClass("android.os.Build$VERSION");
                _apiLevel = version.GetStatic<int>("SDK_INT");
                return _apiLevel;
            }
        }
#endif

        public static void Play(HapticType type = HapticType.Medium)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            PlayAndroid(type);
#elif UNITY_EDITOR
            Debug.Log($"[HapticFeedback] {type}");
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static void PlayAndroid(HapticType type)
        {
            int durationMs = type switch
            {
                HapticType.Light => LightDurationMs,
                HapticType.Heavy => HeavyDurationMs,
                _                => MediumDurationMs
            };
            int amplitude = type switch
            {
                HapticType.Light => LightAmplitude,
                HapticType.Heavy => HeavyAmplitude,
                _                => MediumAmplitude
            };

            try
            {
                var vib = Vibrator;
                if (vib == null) return;

                if (ApiLevel >= MinApiLevelForVibrationEffect)
                {
                    using var effectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    using var effect = effectClass.CallStatic<AndroidJavaObject>(
                        "createOneShot", (long)durationMs, amplitude);
                    vib.Call("vibrate", effect);
                }
                else
                {
                    vib.Call("vibrate", (long)durationMs);
                }
            }
            catch { }
        }
#endif
    }
}