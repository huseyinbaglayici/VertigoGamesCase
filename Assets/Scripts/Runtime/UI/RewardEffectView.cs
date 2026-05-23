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

        [Header("Spread")] [SerializeField] private float _spreadRadius = 30f;

        [Header("Scroll")] [SerializeField] private float _scrollDuration = 0.3f;

        [Header("Fly")] [SerializeField] private float _flyDuration = 0.45f;
        [SerializeField] private float _flyStagger = 0.05f;
        [SerializeField] private Ease _flyEase = Ease.InQuad;

        private InventoryView _inventoryView;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(InventoryView inventoryView, SignalBus signalBus)
        {
            _inventoryView = inventoryView;
            _signalBus = signalBus;
            _signalBus.Subscribe<RewardReadyToFlySignal>(OnRewardReadyToFly);
        }

        private void OnDestroy()
        {
            _signalBus?.Unsubscribe<RewardReadyToFlySignal>(OnRewardReadyToFly);
            foreach (var p in _particles) p.transform.DOKill();
        }

        private void OnRewardReadyToFly(RewardReadyToFlySignal signal) => Play(signal.SlotTransform, signal.Entry);

        private void Play(Transform slotTransform, RewardEntry entry)
        {
            gameObject.SetActive(true);

            float angleStep = 360f / _particles.Length;

            for (int i = 0; i < _particles.Length; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 worldPos = slotTransform.position +
                                   (Vector3)(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * _spreadRadius);

                _particles[i].transform.DOKill();
                _particles[i].sprite = entry.item.icon;
                _particles[i].transform.localPosition = transform.InverseTransformPoint(worldPos);
            }

            _inventoryView.ScrollToItem(entry.item, _scrollDuration, FlyTo);
        }

        private void FlyTo(Vector3 worldTarget)
        {
            Vector3 localTarget = transform.InverseTransformPoint(worldTarget);
            int flyCompleted = 0;

            for (int i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                float delay = i * _flyStagger;

                particle.transform.DOLocalMove(localTarget, _flyDuration)
                    .SetDelay(delay)
                    .SetEase(_flyEase)
                    .OnComplete(() =>
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