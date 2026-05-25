using Runtime.Interfaces;
using Runtime.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class ExitConfirmPopupView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private Button _collectButton;
        [SerializeField] private Button _goBackButton;

        private const string NoItemsMessage = "You don't have any items yet. Are you sure you want to leave?";

        private const string HasItemsMessage =
            "Want to exit and collect your rewards? The best ones are saved for the last!";

        private const string CollectButtonName = "ui_button_ensure_collect";
        private const string GoBackButtonName = "ui_button_ensure_goback";

        private void OnValidate()
        {
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (_collectButton == null && button.gameObject.name == CollectButtonName) _collectButton = button;
                if (_goBackButton == null && button.gameObject.name == GoBackButtonName) _goBackButton = button;
            }
        }

        private SignalBus _signalBus;
        private ISpinManager _spinManager;

        [Inject]
        public void Construct(SignalBus signalBus, ISpinManager spinManager)
        {
            _signalBus = signalBus;
            _spinManager = spinManager;
            _signalBus.Subscribe<ExitConfirmRequestSignal>(Show);
            _signalBus.Subscribe<GameRestartSignal>(Hide);
            _collectButton.onClick.AddListener(OnCollectClicked);
            _goBackButton.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<ExitConfirmRequestSignal>(Show);
                _signalBus.Unsubscribe<GameRestartSignal>(Hide);
            }

            _collectButton.onClick.RemoveListener(OnCollectClicked);
            _goBackButton.onClick.RemoveListener(Hide);
        }

        private void Show(ExitConfirmRequestSignal signal)
        {
            _messageText.text = signal.HasItems ? HasItemsMessage : NoItemsMessage;
            gameObject.SetActive(true);
        }

        private void Hide() => gameObject.SetActive(false);

        private void OnCollectClicked()
        {
            Hide();
            _spinManager.RequestRewards();
        }
    }
}