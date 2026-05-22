using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Transform _content;
        [SerializeField] private RewardItemView _itemPrefab;
        [SerializeField] private Button _collectButton;

        private IInventoryManager _inventoryManager;
        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;

        private void OnValidate()
        {
            if (_collectButton != null) return;
            var buttons = GetComponentsInChildren<Button>(true);
            if (buttons.Length > 0) _collectButton = buttons[0];
        }

        [Inject]
        public void Construct(IInventoryManager inventoryManager, ISpinManager spinManager, IZoneManager zoneManager)
        {
            _inventoryManager = inventoryManager;
            _spinManager = spinManager;
            _zoneManager = zoneManager;

            _spinManager.OnRewardsRequested += Show;
            _zoneManager.OnGameCompleted += Show;
            _collectButton.onClick.AddListener(OnCollectClicked);
        }

        private void OnDestroy()
        {
            if (_spinManager != null) _spinManager.OnRewardsRequested -= Show;
            if (_zoneManager != null) _zoneManager.OnGameCompleted -= Show;
            _collectButton.onClick.RemoveListener(OnCollectClicked);
        }

        private void Show()
        {
            foreach (Transform child in _content)
                Destroy(child.gameObject);

            foreach (var item in _inventoryManager.GetItems())
            {
                var view = Instantiate(_itemPrefab, _content);
                view.Init(item);
            }

            gameObject.SetActive(true);
        }

        private void OnCollectClicked() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}