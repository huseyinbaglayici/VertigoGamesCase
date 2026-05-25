using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class RewardEffectView : MonoBehaviour
    {
        [SerializeField] private Image[] _particles;

        private const float FullRotation = 360f;

        private SO_RewardAnimationConfig.ParticleScatterFly _cfg;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, SO_RewardAnimationConfig animConfig)
        {
            _signalBus = signalBus;
            _cfg = animConfig.particleScatterFly;
            _signalBus.Subscribe<RewardReadyToFlySignal>(OnRewardReadyToFly);
            _signalBus.Subscribe<ScrollToItemCompleteSignal>(OnScrollComplete);
        }

        private void OnDestroy()
        {
            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<RewardReadyToFlySignal>(OnRewardReadyToFly);
                _signalBus.Unsubscribe<ScrollToItemCompleteSignal>(OnScrollComplete);
            }

            foreach (var p in _particles) p.transform.DOKill();
        }

        private void OnRewardReadyToFly(RewardReadyToFlySignal signal) => Play(signal.SlotTransform, signal.Entry);
        private void OnScrollComplete(ScrollToItemCompleteSignal signal) => FlyTo(signal.WorldPosition);

        private void Play(Transform slotTransform, RewardEntry entry)
        {
            gameObject.SetActive(true);

            float angleStep = FullRotation / _particles.Length;
            Vector3 slotLocal = transform.InverseTransformPoint(slotTransform.position);

            for (int i = 0; i < _particles.Length; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 scatterLocal = slotLocal + new Vector3(
                    Mathf.Cos(angle) * _cfg.spreadRadius,
                    Mathf.Sin(angle) * _cfg.spreadRadius, 0f);

                _particles[i].transform.DOKill();
                _particles[i].sprite = entry.item.icon;
                _particles[i].transform.localPosition = slotLocal;

                _particles[i].transform.DOLocalMove(scatterLocal, _cfg.scatterDuration)
                    .SetEase(_cfg.scatterEase);
            }

            _signalBus.Fire(new ScrollToItemRequestSignal { Config = entry.item, Duration = _cfg.scrollDuration });
        }

        private void FlyTo(Vector3 worldTarget = default)
        {
            Vector3 localTarget = transform.InverseTransformPoint(worldTarget);
            int flyCompleted = 0;

            for (int i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                float peakY = particle.transform.localPosition.y + _cfg.jumpHeight;

                var seq = DOTween.Sequence().SetDelay(i * _cfg.flyStagger);
                seq.Append(particle.transform.DOLocalMoveY(peakY, _cfg.jumpDuration).SetEase(Ease.OutQuad));
                seq.Append(particle.transform.DOLocalMove(localTarget, _cfg.flyDuration).SetEase(_cfg.flyEase));
                seq.OnComplete(() =>
                {
                    flyCompleted++;
                    if (flyCompleted < _particles.Length) return;
                    foreach (var p in _particles)
                        p.transform.localPosition = Vector3.zero;
                    _signalBus.Fire<RewardFlyCompleteSignal>();
                    gameObject.SetActive(false);
                });
            }
        }
    }
}