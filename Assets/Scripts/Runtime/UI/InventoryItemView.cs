using System;
using DG.Tweening;
using Runtime.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class InventoryItemView : MonoBehaviour
    {
        [SerializeField] private float _countDuration = 0.5f;

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;
        private const float SpawnDuration = 0.35f;

        public event Action OnBounced;

        private int _displayedAmount;
        private int _targetAmount;
        private Tween _countTween;
        private Tween _spawnTween;

        public float SpawnRemainingTime =>
            _spawnTween != null && _spawnTween.IsActive()
                ? Mathf.Max(0f, SpawnDuration - (float)_spawnTween.Elapsed())
                : 0f;

        public void Init(ItemData item)
        {
            _icon.sprite = item.Config.icon;
            _displayedAmount = 0;
            _targetAmount = item.Amount;
            _amountText.text = "0";
        }

        public void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            _spawnTween = transform.DOScale(Vector3.one, SpawnDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() => AnimateToAmount(_targetAmount));
        }

        public void AnimateToAmount(int targetAmount)
        {
            OnBounced?.Invoke();
            _countTween?.Kill();
            _countTween = DOTween.To(
                () => _displayedAmount,
                x =>
                {
                    _displayedAmount = x;
                    _amountText.text = $"{x}";
                },
                targetAmount,
                _countDuration
            ).SetEase(Ease.OutQuad);
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f);
        }

        public Vector3 GetIconWorldCenter() =>
            _icon.rectTransform.TransformPoint(_icon.rectTransform.rect.center);


        private void OnDestroy()
        {
            _countTween?.Kill();
            transform.DOKill();
        }
    }
}