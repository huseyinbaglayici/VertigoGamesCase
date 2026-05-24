using Runtime.Data.UnityObjects;
using UnityEngine;
using Zenject;

namespace Runtime.Audio
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _spinSource;

        private SO_AudioConfig _config;

        [Inject]
        public void Construct(SO_AudioConfig config) => _config = config;

        public void PlaySpinTick()
        {
            if (_config.spinTickClip == null || _spinSource == null) return;
            _spinSource.clip = _config.spinTickClip;
            _spinSource.Play();
        }
        public void PlayBombSfx()       => PlayMain(_config.bombClip);
        public void StopBombSfx()       => _sfxSource?.Stop();
        public void PlayRewardSfx()     => PlayOneShot(_config.rewardClip);
        public void PlayGoldRewardSfx() => PlayOneShot(_config.goldRewardClip);
        public void PlayTakeRewardSfx() => PlayOneShot(_config.takeRewardClip);
        public void PlayCollectSfx()    => PlayOneShot(_config.collectRewardsClip);
        public void PlayItemPopSfx()    => PlayOneShot(_config.itemPopClip);

        // PlayMain: main clip slot — Stop() ile durdurulabilir (PlayOneShot'ları etkilemez)
        private void PlayMain(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.clip = clip;
            _sfxSource.Play();
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip);
        }
    }
}
