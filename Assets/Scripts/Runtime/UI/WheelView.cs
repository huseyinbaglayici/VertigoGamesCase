using System.Collections;
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
        [SerializeField] private Image _wheelBaseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private Button _spinButton;
        [SerializeField] private WheelSlotView[] _slotViews;

        [Header("Idle")] [SerializeField] private float _idleRotationDuration = 20f;
        [SerializeField] private float _idleBreatheDuration = 2f;
        [SerializeField] private float _idleBreatheScale = 1.02f;

        [Header("Intro")] [SerializeField] private float _introPunchScale = 0.3f;
        [SerializeField] private float _introPunchDuration = 0.5f;
        [SerializeField] private int _introPunchVibrato = 5;
        [SerializeField] private float _introPunchElasticity = 0.5f;

        [Header("Spin")] [SerializeField] private float _spinDuration = 3f;
        [SerializeField] private int _spinFullRotations = 5;

        [SerializeField] private Ease _spinEase = Ease.InOutCubic;

        // Indicator position in standard math degrees (CCW from +X). 90 = top (+Y).
        [SerializeField] private float _indicatorAngle = 90f;

        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private SO_GameConfig _gameConfig;
        private SceneTransitionView _transition;
        private bool _isSpinning;
        private bool _gameCompleted;

        [Inject]
        public void Construct(ISpinManager spinManager, IZoneManager zoneManager, SO_GameConfig gameConfig, SceneTransitionView transition)
        {
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _gameConfig = gameConfig;
            _transition = transition;
        }

        private void OnValidate()
        {
            if (_spinButton == null)
                _spinButton = GetComponentInChildren<Button>();
            if (_slotViews == null || _slotViews.Length == 0)
                _slotViews = GetComponentsInChildren<WheelSlotView>();
        }

        private IEnumerator Start()
        {
            _spinButton.onClick.AddListener(OnSpinClicked);
            _spinManager.OnSpinDecision += HandleSpinDecision;
            _spinManager.OnGameResumed += HandleGameResumed;
            _zoneManager.OnGameCompleted += HandleGameCompleted;
            yield return null;
            RefreshSlotData();
            _transition.OnOpened += PlayIntro;
            if (_transition.HasOpened)
            {
                _transition.OnOpened -= PlayIntro;
                PlayIntro();
            }
        }

        private void PlayIntro()
        {
            _transition.OnOpened -= PlayIntro;
            _wheelContent.DOPunchScale(Vector3.one * _introPunchScale, _introPunchDuration, _introPunchVibrato,
                    _introPunchElasticity)
                .OnComplete(StartIdleAnimation);

            _spinButton.transform.localScale = Vector3.zero;
            _spinButton.transform.DOScale(Vector3.one, _introPunchDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(_introPunchDuration * 0.3f);
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(OnSpinClicked);
            if (_spinManager != null)
            {
                _spinManager.OnSpinDecision -= HandleSpinDecision;
                _spinManager.OnGameResumed -= HandleGameResumed;
            }
            if (_zoneManager != null)
                _zoneManager.OnGameCompleted -= HandleGameCompleted;
            if (_transition != null)
                _transition.OnOpened -= PlayIntro;
            _wheelContent.DOKill();
        }

        private void OnSpinClicked()
        {
            if (_isSpinning) return;
            SetSpinning(true);
            _spinManager.Spin();
        }

        private void HandleSpinDecision(int slotIndex, RewardEntry result)
        {
            _wheelContent.DOKill();
            _wheelContent.localScale = Vector3.one;

            float slotAngle = GetSlotAngleDeg(slotIndex);
            float currentZ = _wheelContent.eulerAngles.z;

            // targetZ: the wheel Z that brings slotAngle to _indicatorAngle
            float targetZ = ((_indicatorAngle - slotAngle) % 360f + 360f) % 360f;

            // Clockwise (decreasing Z) distance from currentZ to targetZ
            float cwDistance = currentZ - targetZ;
            if (cwDistance < 10f) cwDistance += 360f;
            cwDistance += _spinFullRotations * 360f;

            bool isBomb = result.item.isBomb;

            _wheelContent.DORotate(new Vector3(0f, 0f, -cwDistance), _spinDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(_spinEase)
                .OnComplete(() =>
                {
                    _spinManager.CommitSpinResult();
                    if (isBomb || _gameCompleted) return;
                    RefreshSlotData();
                    SetSpinning(false);
                    StartIdleAnimation();
                });
        }

        private void HandleGameResumed()
        {
            SetSpinning(false);
            StartIdleAnimation();
        }

        private void HandleGameCompleted()
        {
            _gameCompleted = true;
            _wheelContent.DOKill();
            SetSpinning(true);
        }

        private void SetSpinning(bool spinning)
        {
            _isSpinning = spinning;
            _spinButton.interactable = !spinning;
        }

        private float GetSlotAngleDeg(int slotIndex)
        {
            Vector2 localPos = _slotViews[slotIndex].transform.localPosition;
            return Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
        }

        private void RefreshSlotData()
        {
            var config = _zoneManager.GetCurrentWheelConfig();
            var rewardSet = _zoneManager.GetCurrentRewardSet(config);

            _wheelBaseImage.sprite = config.baseSprite;
            _indicatorImage.sprite = config.indicatorSprite;

            for (int i = 0; i < _slotViews.Length; i++)
                _slotViews[i].Setup(rewardSet.rewards[i], _zoneManager.CurrentZone, _gameConfig.goldZoneInterval);
        }

        private void StartIdleAnimation()
        {
            _wheelContent.DORotate(new Vector3(0f, 0f, -360f), _idleRotationDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);

            _wheelContent.DOScale(Vector3.one * _idleBreatheScale, _idleBreatheDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}