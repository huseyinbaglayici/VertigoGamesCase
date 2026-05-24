using Runtime.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;

namespace Runtime.UI
{
    public class CurrencyHudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private ICurrencyManager _currencyManager;

        [Inject]
        public void Construct(ICurrencyManager currencyManager)
        {
            _currencyManager = currencyManager;
            _currencyManager.OnCurrencyChanged += Refresh;
            Refresh(_currencyManager.Currency);
        }

        private void OnDestroy()
        {
            if (_currencyManager != null) _currencyManager.OnCurrencyChanged -= Refresh;
        }

        private void Refresh(int currency) => _currencyText.text = currency.ToString("N0");
    }
}
