using UnityEngine;
using Zenject;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "AudioConfig", menuName = "Game/Audio Config")]
    public class SO_AudioConfig : ScriptableObjectInstaller
    {
        public AudioClip spinTickClip;
        public AudioClip rewardClip;
        public AudioClip goldRewardClip;
        public AudioClip bombClip;
        public AudioClip takeRewardClip;
        public AudioClip collectRewardsClip;
        public AudioClip itemPopClip;
        public AudioClip specialItemOpenClip;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
    }
}