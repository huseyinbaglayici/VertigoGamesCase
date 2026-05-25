using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class WheelCellView : MonoBehaviour
    {
        [FormerlySerializedAs("Icon")] [SerializeField]
        private Image _icon;

        [FormerlySerializedAs("AmountText")] [SerializeField]
        private TextMeshProUGUI _amountText;

        public Image Icon => _icon;
        public TextMeshProUGUI AmountText => _amountText;
    }
}