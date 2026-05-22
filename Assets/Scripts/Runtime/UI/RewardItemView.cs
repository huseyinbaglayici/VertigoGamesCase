using Runtime.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _amountText;

        public void Init(ItemData item)
        {
            _icon.sprite = item.Config.icon;
            _amountText.text = $"x{item.Amount}";
        }
    }
}