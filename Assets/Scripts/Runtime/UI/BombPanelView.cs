using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class BombPanelView : MonoBehaviour
    {
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private RectTransform _inventorySlot;
        [SerializeField] private RectTransform _inventoryRoot;

        private ISpinManager _spinManager;
        private Transform _inventoryOriginalParent;
        private int _inventoryOriginalSiblingIndex;

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
        public void Construct(ISpinManager spinManager)
        {
            _spinManager = spinManager;
            _spinManager.OnBombHit += Show;
            _continueButton.onClick.AddListener(OnContinueClicked);
            _exitButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnDestroy()
        {
            if (_spinManager != null)
                _spinManager.OnBombHit -= Show;
            _continueButton.onClick.RemoveListener(OnContinueClicked);
            _exitButton.onClick.RemoveListener(OnRestartClicked);
        }

        private void Show()
        {
            if (_inventoryRoot != null && _inventorySlot != null)
            {
                _inventoryOriginalParent = _inventoryRoot.parent;
                _inventoryOriginalSiblingIndex = _inventoryRoot.GetSiblingIndex();
                _inventoryRoot.SetParent(_inventorySlot, false);
            }
            gameObject.SetActive(true);
        }

        private void Hide()
        {
            if (_inventoryRoot != null && _inventoryOriginalParent != null)
            {
                _inventoryRoot.SetParent(_inventoryOriginalParent, false);
                _inventoryRoot.SetSiblingIndex(_inventoryOriginalSiblingIndex);
            }
            gameObject.SetActive(false);
        }

        private void OnContinueClicked()
        {
            Hide();
            _spinManager.Continue();
        }

        private void OnRestartClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}