using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace Runtime.Audio
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _spinSource;

        private SO_AudioConfig _audioConfig;
        private SO_GameConfig _gameConfig;
        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SO_AudioConfig audioConfig, SO_GameConfig gameConfig,
            ISpinManager spinManager, IZoneManager zoneManager, SignalBus signalBus)
        {
            _audioConfig = audioConfig;
            _gameConfig = gameConfig;
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _signalBus = signalBus;
        }

        private void Start()
        {
            _spinManager.OnSpinDecision += HandleSpinDecision;
            _spinManager.OnBombHit += HandleBombHit;
            _spinManager.OnSpinCompleted += HandleSpinCompleted;
            _spinManager.OnRewardsRequested += HandleTakeReward;
            _signalBus.Subscribe<RewardCollectedSignal>(HandleCollect);
        }

        private void OnDestroy()
        {
            if (_spinManager != null)
            {
                _spinManager.OnSpinDecision -= HandleSpinDecision;
                _spinManager.OnBombHit -= HandleBombHit;
                _spinManager.OnSpinCompleted -= HandleSpinCompleted;
                _spinManager.OnRewardsRequested -= HandleTakeReward;
            }
            if (_signalBus != null)
                _signalBus.Unsubscribe<RewardCollectedSignal>(HandleCollect);
        }

        private void HandleSpinDecision(int _, RewardEntry __) => PlayLoop(_audioConfig.spinClip);

        private void HandleBombHit()
        {
            StopLoop();
            PlayOneShot(_audioConfig.bombClip);
        }

        private void HandleSpinCompleted(RewardEntry _)
        {
            StopLoop();
            // AdvanceZone fires before OnSpinCompleted, so CurrentZone-1 was the spun zone
            bool wasGoldZone = (_zoneManager.CurrentZone - 1) % _gameConfig.goldZoneInterval == 0;
            PlayOneShot(wasGoldZone ? _audioConfig.goldRewardClip : _audioConfig.rewardClip);
        }

        public void PlaySpinTick() => PlayOneShot(_audioConfig.spinTickClip);

        private void HandleTakeReward() => PlayOneShot(_audioConfig.takeRewardClip);
        private void HandleCollect() => PlayOneShot(_audioConfig.collectClip);

        private void PlayLoop(AudioClip clip)
        {
            if (clip == null || _spinSource == null) return;
            _spinSource.clip = clip;
            _spinSource.loop = true;
            _spinSource.Play();
        }

        private void StopLoop()
        {
            if (_spinSource == null) return;
            _spinSource.Stop();
            _spinSource.loop = false;
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip);
        }
    }
}
