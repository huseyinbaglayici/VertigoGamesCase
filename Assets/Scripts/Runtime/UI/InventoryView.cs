using System;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class InventoryView : MonoBehaviour
    {
        #region Fields & References

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Transform _content;
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField] private Button _exitButton;

        private const string ExitButtonName      = "ui_button_inventory_exit";
        private const float  ViewportCenterRatio = 0.5f;

        private IInventoryManager _inventoryManager;
        private ISpinManager _spinManager;
        private IAudioService _audioService;
        private SignalBus _signalBus;
        private readonly Dictionary<SO_ItemConfig, InventoryItemView> _itemViews = new();

        #endregion

        #region Lifecycle

        private void OnValidate()
        {
            if (_exitButton != null) return;
            foreach (var button in GetComponentsInChildren<Button>(true))
                if (button.gameObject.name == ExitButtonName)
                {
                    _exitButton = button;
                    break;
                }
        }

        [Inject]
        public void Construct(IInventoryManager inventoryManager, ISpinManager spinManager,
            IAudioService audioService, SignalBus signalBus)
        {
            _inventoryManager = inventoryManager;
            _spinManager = spinManager;
            _audioService = audioService;
            _signalBus = signalBus;
        }

        private void Start()
        {
            _inventoryManager.OnItemAdded += HandleItemAdded;
            _spinManager.OnSpinDecision += OnSpinStarted;
            _spinManager.OnSpinCompleted += OnSpinEnded;
            _spinManager.OnGameResumed += OnSpinResumed;
            _signalBus.Subscribe<GameRestartSignal>(HandleReset);
            _signalBus.Subscribe<ScrollToItemRequestSignal>(OnScrollRequest);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnDestroy()
        {
            if (_inventoryManager != null) _inventoryManager.OnItemAdded -= HandleItemAdded;
            if (_spinManager != null)
            {
                _spinManager.OnSpinDecision -= OnSpinStarted;
                _spinManager.OnSpinCompleted -= OnSpinEnded;
                _spinManager.OnGameResumed -= OnSpinResumed;
            }

            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<GameRestartSignal>(HandleReset);
                _signalBus.Unsubscribe<ScrollToItemRequestSignal>(OnScrollRequest);
            }

            _exitButton.onClick.RemoveListener(OnExitClicked);
        }

        #endregion

        #region Item Management

        private void OnSpinStarted(int _, RewardEntry __) => _exitButton.interactable = false;
        private void OnSpinEnded(RewardEntry _)           => _exitButton.interactable = true;
        private void OnSpinResumed()                      => _exitButton.interactable = true;

        private void OnExitClicked()
        {
            bool hasItems = _inventoryManager.GetItems().Count > 0;
            _signalBus.Fire(new ExitConfirmRequestSignal { HasItems = hasItems });
        }

        private void HandleItemAdded(ItemData item, bool isNew)
        {
            if (isNew)
            {
                var view = Instantiate(_itemPrefab, _content).GetComponent<InventoryItemView>();
                view.Init(item);
                view.OnBounced += _audioService.PlayTakeRewardSfx;
                view.PlaySpawnAnimation();
                _itemViews[item.Config] = view;
            }
            else
            {
                _itemViews[item.Config].AnimateToAmount(item.Amount);
            }
        }

        private void HandleReset()
        {
            foreach (Transform child in _content) Destroy(child.gameObject);
            _itemViews.Clear();
            _exitButton.interactable = true;
        }

        #endregion

        #region Scroll

        private void OnScrollRequest(ScrollToItemRequestSignal signal)
        {
            ScrollToItem(signal.Config, signal.Duration,
                pos => _signalBus.Fire(new ScrollToItemCompleteSignal { WorldPosition = pos }));
        }

        private void ScrollToItem(SO_ItemConfig config, float duration, Action<Vector3> onComplete)
        {
            if (!_itemViews.TryGetValue(config, out var view))
            {
                onComplete?.Invoke(Vector3.zero);
                return;
            }

            Canvas.ForceUpdateCanvases();
            var itemRect = (RectTransform)view.transform;

            void Deliver()
            {
                Canvas.ForceUpdateCanvases();
                LayoutRebuilder.ForceRebuildLayoutImmediate(_scrollRect.content);

                float wait = view.SpawnRemainingTime;
                if (wait > 0f)
                    DOVirtual.DelayedCall(wait, () => onComplete?.Invoke(view.GetIconWorldCenter()));
                else
                    onComplete?.Invoke(view.GetIconWorldCenter());
            }

            if (IsItemInViewport(itemRect))
            {
                Deliver();
                return;
            }

            _scrollRect.DOVerticalNormalizedPos(GetTargetNormalizedPos(itemRect), duration)
                .SetEase(Ease.OutCubic)
                .OnComplete(Deliver);
        }

        private bool IsItemInViewport(RectTransform item)
        {
            var vc = new Vector3[4];
            var ic = new Vector3[4];
            _scrollRect.viewport.GetWorldCorners(vc);
            item.GetWorldCorners(ic);
            return ic[0].y >= vc[0].y && ic[1].y <= vc[1].y;
        }

        private float GetTargetNormalizedPos(RectTransform item)
        {
            var contentRect = (RectTransform)_content;
            float scrollable = contentRect.rect.height - _scrollRect.viewport.rect.height;
            if (scrollable <= 0f) return 1f;

            float itemLocalY = contentRect.InverseTransformPoint(item.position).y;
            float offset = Mathf.Clamp(-itemLocalY - _scrollRect.viewport.rect.height * ViewportCenterRatio, 0f, scrollable);
            return 1f - offset / scrollable;
        }

        #endregion
    }
}
