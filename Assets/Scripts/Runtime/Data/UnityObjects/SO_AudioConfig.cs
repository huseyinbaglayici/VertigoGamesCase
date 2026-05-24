using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/Audio Config")]
    public class SO_AudioConfig : ScriptableObject
    {
        public AudioClip spinClip;
        public AudioClip spinTickClip;
        public AudioClip rewardClip;
        public AudioClip goldRewardClip;
        public AudioClip bombClip;
        public AudioClip takeRewardClip;
        public AudioClip collectRewardsClip;
        public AudioClip itemPopClip;
    }
}
