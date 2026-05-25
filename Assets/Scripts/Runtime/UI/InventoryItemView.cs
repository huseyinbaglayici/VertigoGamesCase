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
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        [Header("Spawn")] [SerializeField] private float _spawnDuration = 0.35f;
        [SerializeField] private Ease _spawnEase = Ease.OutBack;

        [Header("Count")] [SerializeField] private float _countDuration = 0.5f;
        [SerializeField] private Ease _countEase = Ease.OutQuad;

        [Header("Bounce")] [SerializeField] private float _bounceStrength = 0.2f;
        [SerializeField] private float _bounceDuration = 0.3f;
        [SerializeField] private int _bounceVibrato = 5;
        [SerializeField] private float _bounceElastic = 0.5f;

        public event Action OnBounced;

        private int _displayedAmount;
        private int _targetAmount;
        private Tween _countTween;
        private Tween _spawnTween;

        public float SpawnRemainingTime =>
            _spawnTween != null && _spawnTween.IsActive()
                ? Mathf.Max(0f, _spawnDuration - _spawnTween.Elapsed())
                : 0f;

        public void Init(ItemData item)
        {
            _icon.sprite = item.Config.icon;
            _displayedAmount = 0;
            _targetAmount = item.Amount;
            _amountText.text = _displayedAmount.ToString();
        }

        public void PlaySpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            _spawnTween = transform.DOScale(Vector3.one, _spawnDuration)
                .SetEase(_spawnEase)
                .OnComplete(() => AnimateToAmount(_targetAmount));
        }

        public void AnimateToAmount(int targetAmount)
        {
            OnBounced?.Invoke();
            _countTween?.Kill();
            _countTween = DOTween.To(() => _displayedAmount, SetDisplayedAmount, targetAmount, _countDuration)
                .SetEase(_countEase);
            transform.DOPunchScale(Vector3.one * _bounceStrength, _bounceDuration, _bounceVibrato, _bounceElastic);
        }

        private void SetDisplayedAmount(int value)
        {
            _displayedAmount = value;
            _amountText.text = value.ToString();
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