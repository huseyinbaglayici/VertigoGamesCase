using System.Collections.Generic;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class InventoryView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Button _exitButton;

        private IInventoryManager _inventoryManager;

        private void OnValidate()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            if (buttons.Length == 1) _exitButton = buttons[0];
        }
        private ISpinManager _spinManager;
        private readonly Dictionary<SO_ItemConfig, InventoryItemView> _itemViews = new();

        [Inject]
        public void Construct(IInventoryManager inventoryManager, ISpinManager spinManager)
        {
            _inventoryManager = inventoryManager;
            _spinManager = spinManager;
        }

        private void Start()
        {
            _inventoryManager.OnItemAdded += HandleItemAdded;
            _spinManager.OnSpinDecision += OnSpinStarted;
            _spinManager.OnSpinCompleted += OnSpinEnded;
            _spinManager.OnGameResumed += OnSpinResumed;
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            if (_inventoryManager != null)
                _inventoryManager.OnItemAdded -= HandleItemAdded;
            if (_spinManager != null)
            {
                _spinManager.OnSpinDecision -= OnSpinStarted;
                _spinManager.OnSpinCompleted -= OnSpinEnded;
                _spinManager.OnGameResumed -= OnSpinResumed;
            }
            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        private void OnSpinStarted(int _, RewardEntry __) => _exitButton.interactable = false;
        private void OnSpinEnded(RewardEntry _) => _exitButton.interactable = true;
        private void OnSpinResumed() => _exitButton.interactable = true;
        private void OnExitClicked() => _spinManager.RequestRewards();

        private void HandleItemAdded(ItemData item, bool isNew)
        {
            if (isNew)
            {
                var view = Instantiate(_itemPrefab, _content).GetComponent<InventoryItemView>();
                view.Init(item);
                _itemViews[item.Config] = view;
            }
            else
            {
                _itemViews[item.Config].AnimateToAmount(item.Amount);
            }
        }
    }
}
