using Runtime.Data.UnityObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelSlotView : MonoBehaviour
    {
        [SerializeField] private WheelCellView _cell;

        private const string SafeLabel = "SAFE";

        private Image _iconImage;
        private TextMeshProUGUI _amountText;

        private void OnValidate()
        {
            if (_cell == null)
                _cell = GetComponentInChildren<WheelCellView>();
        }

        private void Awake()
        {
            _iconImage = _cell.Icon;
            _amountText = _cell.AmountText;
        }

        public void Setup(RewardEntry entry, int currentZone, int totalZones)
        {
            _iconImage.sprite = entry.item.icon;

            if (entry.item.isBomb)
            {
                SetDisplay(Color.white, string.Empty);
                return;
            }

            int amount = CalculateAmount(entry, currentZone, totalZones);
            SetDisplay(Color.white, $"x{amount}");
        }

        public void SetSafe() => SetDisplay(Color.gray, SafeLabel);

        private void SetDisplay(Color color, string text)
        {
            _iconImage.color = color;
            _amountText.text = text;
        }

        private static int CalculateAmount(RewardEntry entry, int currentZone, int totalZones)
        {
            float t = (float)(currentZone - 1) / (totalZones - 1);
            return Mathf.RoundToInt(Mathf.Lerp(entry.minAmount, entry.maxAmount, t));
        }
    }
}