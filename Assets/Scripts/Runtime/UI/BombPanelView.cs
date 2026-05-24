using DG.Tweening;
using Runtime.Audio;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class BombPanelView : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private RectTransform _content;

        private TextMeshProUGUI _continueButtonText;

        [Header("Show Animation")]
        [SerializeField] private float _showDuration = 0.4f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        private ISpinManager _spinManager;
        private ICurrencyManager _currencyManager;
        private AudioService _audioService;
        private SceneTransitionView _transition;
        private SignalBus _signalBus;
        private int _continueCost;

        private const string ContinueButtonName = "ui_button_bomb_continue";
        private const string ExitButtonName = "ui_button_bomb_exit";

        private void OnValidate()
        {
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (button.gameObject.name == ContinueButtonName)
                {
                    _continueButton = button;
                    _continueButtonText = button.GetComponentInChildren<TextMeshProUGUI>();
                }
                if (button.gameObject.name == ExitButtonName) _exitButton = button;
            }
        }

        [Inject]
        public void Construct(ISpinManager spinManager, ICurrencyManager currencyManager,
            SO_GameConfig gameConfig, SceneTransitionView transition, SignalBus signalBus,
            AudioService audioService)
        {
            _spinManager = spinManager;
            _currencyManager = currencyManager;
            _audioService = audioService;
            _transition = transition;
            _signalBus = signalBus;
            _continueCost = gameConfig.continueCost;
            _spinManager.OnBombHit += Show;
            _currencyManager.OnCurrencyChanged += RefreshContinueButton;
            _signalBus.Subscribe<GameRestartSignal>(HandleReset);
            _continueButton.onClick.AddListener(OnContinueClicked);
            _exitButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnDestroy()
        {
            if (_spinManager != null) _spinManager.OnBombHit -= Show;
            if (_currencyManager != null) _currencyManager.OnCurrencyChanged -= RefreshContinueButton;
            if (_signalBus != null) _signalBus.Unsubscribe<GameRestartSignal>(HandleReset);
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _exitButton.onClick.RemoveListener(OnRestartClicked);
            _content.DOKill();
        }

        private void Show()
        {
            if (_continueButtonText != null)
                _continueButtonText.text = $"Continue ({_continueCost:N0})";
            RefreshContinueButton(_currencyManager.Currency);
            gameObject.SetActive(true);
            _content.localScale = Vector3.zero;
            _content.DOScale(Vector3.one, _showDuration).SetEase(_showEase);
        }

        private void Hide()
        {
            _content.DOScale(Vector3.zero, _showDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _audioService.StopBombSfx();
                    gameObject.SetActive(false);
                    _spinManager.Continue();
                });
        }

        private void HandleReset()
        {
            _content.DOKill();
            _content.localScale = Vector3.one;
            gameObject.SetActive(false);
        }

        private void RefreshContinueButton(int currency)
        {
            _continueButton.interactable = currency >= _continueCost;
        }

        private void OnContinueClicked()
        {
            if (!_currencyManager.TrySpend(_continueCost)) return;
            Hide();
        }

        private void OnRestartClicked()
        {
            _audioService.StopBombSfx();
            _transition.FadeAndReset(() => _signalBus.Fire<GameRestartSignal>());
        }
    }
}
