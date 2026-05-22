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

        private Image _icon;
        private TextMeshProUGUI _amountText;
        private int _displayedAmount;
        private Tween _countTween;

        private void Awake()
        {
            _icon = GetComponentInChildren<Image>();
            _amountText = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Init(ItemData item)
        {
            _icon.sprite = item.Config.icon;
            _displayedAmount = item.Amount;
            _amountText.text = $"x{_displayedAmount}";
        }

        public void AnimateToAmount(int targetAmount)
        {
            _countTween?.Kill();
            _countTween = DOTween.To(
                () => _displayedAmount,
                x => { _displayedAmount = x; _amountText.text = $"x{x}"; },
                targetAmount,
                _countDuration
            ).SetEase(Ease.OutQuad);
        }

        private void OnDestroy() => _countTween?.Kill();
    }
}
