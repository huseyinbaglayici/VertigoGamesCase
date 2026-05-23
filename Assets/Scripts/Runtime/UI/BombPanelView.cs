using DG.Tweening;
using Runtime.Interfaces;
using Runtime.Signals;
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

        [Header("Show Animation")]
        [SerializeField] private float _showDuration = 0.4f;
        [SerializeField] private Ease _showEase = Ease.OutBack;

        private ISpinManager _spinManager;
        private SceneTransitionView _transition;
        private SignalBus _signalBus;

        private const string ContinueButtonName = "ui_button_bomb_continue";
        private const string ExitButtonName = "ui_button_bomb_exit";

        private void OnValidate()
        {
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (button.gameObject.name == ContinueButtonName) _continueButton = button;
                if (button.gameObject.name == ExitButtonName) _exitButton = button;
            }
        }

        [Inject]
        public void Construct(ISpinManager spinManager, SceneTransitionView transition, SignalBus signalBus)
        {
            _spinManager = spinManager;
            _transition = transition;
            _signalBus = signalBus;
            _spinManager.OnBombHit += Show;
            _signalBus.Subscribe<GameRestartSignal>(HandleReset);
            _continueButton.onClick.AddListener(OnContinueClicked);
            _exitButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnDestroy()
        {
            if (_spinManager != null) _spinManager.OnBombHit -= Show;
            if (_signalBus != null) _signalBus.Unsubscribe<GameRestartSignal>(HandleReset);
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _exitButton.onClick.RemoveListener(OnRestartClicked);
            _content.DOKill();
        }

        private void Show()
        {
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

        private void OnContinueClicked() => Hide();

        private void OnRestartClicked() => _transition.FadeAndReset(() => _signalBus.Fire<GameRestartSignal>());
    }
}
