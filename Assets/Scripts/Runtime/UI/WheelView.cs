using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class WheelView : MonoBehaviour
    {
        [SerializeField] private RectTransform _wheelContent;
        [SerializeField] private Button _spinButton;
        [SerializeField] private WheelSlotView[] _slotViews;
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private float _idleRotationDuration = 20f;
        [SerializeField] private float _idleBreatheDuration = 2f;
        [SerializeField] private float _idleBreatheScale = 1.02f;

        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private SO_GameConfig _gameConfig;

        [Inject]
        public void Construct(ISpinManager spinManager, IZoneManager zoneManager, SO_GameConfig gameConfig)
        {
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _gameConfig = gameConfig;
        }

        private void OnValidate()
        {
            if (_spinButton == null)
                _spinButton = GetComponentInChildren<Button>();
            if (_slotViews == null || _slotViews.Length == 0)
                _slotViews = GetComponentsInChildren<WheelSlotView>();
        }

        private void Start()
        {
            _spinButton.onClick.AddListener(OnSpinClicked);
            SetupSlots();
            StartIdleAnimation();
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(OnSpinClicked);
            _wheelContent.DOKill();
        }

        private void OnSpinClicked()
        {
            _spinManager.Spin();
        }

        private void SetupSlots()
        {
            var config = _zoneManager.GetCurrentWheelConfig();
            var rewardSet = _zoneManager.GetCurrentRewardSet(config);

            for (int i = 0; i < _slotViews.Length; i++)
            {
                _slotViews[i].Init(_cellPrefab);
                _slotViews[i].Setup(rewardSet.rewards[i], _zoneManager.CurrentZone, _gameConfig.goldZoneInterval);
            }
        }

        private void StartIdleAnimation()
        {
            _wheelContent.DORotate(new Vector3(0, 0, -360), _idleRotationDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);

            _wheelContent.DOScale(Vector3.one * _idleBreatheScale, _idleBreatheDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}