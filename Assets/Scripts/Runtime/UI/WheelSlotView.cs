using Runtime.Data.UnityObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelSlotView : MonoBehaviour
    {
        private Image _iconImage;
        private TextMeshProUGUI _amountText;

        public void Init(GameObject cellPrefab)
        {
            var cell = Instantiate(cellPrefab, transform);
            _iconImage = cell.GetComponentInChildren<Image>();
            _amountText = cell.GetComponentInChildren<TextMeshProUGUI>();
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