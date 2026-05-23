using Runtime.Data.UnityObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelSlotView : MonoBehaviour
    {
        [SerializeField] private WheelCellView _cell;

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
            int amount = Mathf.RoundToInt(Mathf.Lerp(entry.minAmount, entry.maxAmount,
                (float)(currentZone - 1) / (totalZones - 1)));
            _amountText.text = $"x{amount}";
        }
    }
}