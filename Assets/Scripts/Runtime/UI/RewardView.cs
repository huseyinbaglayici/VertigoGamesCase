using DG.Tweening;
using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private RewardItemView _itemPrefab;
        [SerializeField] private Button _collectButton;
        [SerializeField] private float _itemAnimDuration = 0.3f;
        [SerializeField] private float _itemAnimInterval = 0.08f;

        private IInventoryManager _inventoryManager;
        private ICurrencyManager _currencyManager;
        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private SceneTransitionView _transition;
        private SignalBus _signalBus;

        private const string CollectButtonName = "ui_button_reward_exit";

        private void OnValidate()
        {
            if (_collectButton != null) return;
            foreach (var button in GetComponentsInChildren<Button>(true))
                if (button.gameObject.name == CollectButtonName)
                { _collectButton = button; break; }
        }

        [Inject]
        public void Construct(IInventoryManager inventoryManager, ICurrencyManager currencyManager,
            ISpinManager spinManager, IZoneManager zoneManager,
            SceneTransitionView transition, SignalBus signalBus)
        {
            _inventoryManager = inventoryManager;
            _currencyManager = currencyManager;
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _transition = transition;
            _signalBus = signalBus;
            _spinManager.OnRewardsRequested += Show;
            _zoneManager.OnGameCompleted += Show;
            _signalBus.Subscribe<GameRestartSignal>(HandleReset);
            _collectButton.onClick.AddListener(OnCollectClicked);
        }

        private void OnDestroy()
        {
            if (_spinManager != null) _spinManager.OnRewardsRequested -= Show;
            if (_zoneManager != null) _zoneManager.OnGameCompleted -= Show;
            if (_signalBus != null) _signalBus.Unsubscribe<GameRestartSignal>(HandleReset);
            _collectButton.onClick.RemoveListener(OnCollectClicked);
        }

        private void Show()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            gameObject.SetActive(true);

            int index = 0;
            foreach (var item in _inventoryManager.GetItems())
            {
                var view = Instantiate(_itemPrefab, _content);
                view.Init(item);
                view.transform.localScale = Vector3.zero;
                view.transform.DOScale(Vector3.one, _itemAnimDuration)
                    .SetDelay(index * _itemAnimInterval)
                    .SetEase(Ease.OutBack);
                index++;
            }
        }

        private void HandleReset()
        {
            foreach (Transform child in _content) Destroy(child.gameObject);
            gameObject.SetActive(false);
        }

        private void OnCollectClicked()
        {
            int total = 0;
            foreach (var item in _inventoryManager.GetItems())
                if (item.Config.isCurrency) total += item.Amount;
            if (total > 0) _currencyManager.Collect(total);

            _transition.FadeAndReset(() => _signalBus.Fire<GameRestartSignal>());
        }
    }
}
