using System.Collections;
using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class WheelView : MonoBehaviour
    {
        #region References & Configuration

        [SerializeField] private RectTransform _wheelContent;
        [SerializeField] private Image _wheelBaseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private Button _spinButton;
        [SerializeField] private WheelSlotView[] _slotViews;

        private const float IdleRotationDuration = 40f;
        private const float IntroDuration = 0.5f;
        private const float SpinDuration = 3f;
        private const int SpinFullRotations = 5;
        private const float IndicatorAngle = 90f;

        // Non-zero initial slope keeps wheel above idle speed at spin start — prevents perceived pause.
        private static readonly AnimationCurve SpinCurve = new AnimationCurve(
            new Keyframe(0f,    0f,   0f,   0.1f),
            new Keyframe(0.35f, 0.5f, 3.0f, 3.0f),
            new Keyframe(1f,    1f,   0f,   0f)
        );

        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private SO_GameConfig _gameConfig;
        private SceneTransitionView _transition;
        private SignalBus _signalBus;
        private bool _isSpinning;
        private bool _gameCompleted;

        #endregion

        #region Lifecycle

        [Inject]
        public void Construct(ISpinManager spinManager, IZoneManager zoneManager, SO_GameConfig gameConfig,
            SceneTransitionView transition, SignalBus signalBus)
        {
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _gameConfig = gameConfig;
            _transition = transition;
            _signalBus = signalBus;
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
            _signalBus.Subscribe<GameRestartSignal>(HandleReset);
            _signalBus.Subscribe<RewardFlyCompleteSignal>(GoIdle);
            yield return null;
            RefreshSlotData();
            _transition.OnOpened += PlayIntro;
            if (_transition.HasOpened)
            {
                _transition.OnOpened -= PlayIntro;
                PlayIntro();
            }
        }

        private void OnDestroy()
        {
            _spinButton.onClick.RemoveListener(OnSpinClicked);
            if (_spinManager != null)
            {
                _spinManager.OnSpinDecision -= HandleSpinDecision;
                _spinManager.OnGameResumed -= HandleGameResumed;
            }
            if (_zoneManager != null) _zoneManager.OnGameCompleted -= HandleGameCompleted;
            if (_transition != null) _transition.OnOpened -= PlayIntro;
            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<GameRestartSignal>(HandleReset);
                _signalBus.Unsubscribe<RewardFlyCompleteSignal>(GoIdle);
            }
            transform.DOKill();
            _wheelContent.DOKill();
        }

        #endregion

        #region Spin

        private void OnSpinClicked()
        {
            if (_isSpinning) return;
            SetSpinning(true);
            _spinManager.Spin();
        }

        private void HandleSpinDecision(int slotIndex, RewardEntry result)
        {
            _wheelContent.DOKill();

            bool isBomb = result.item.isBomb;

            _wheelContent.DORotate(new Vector3(0f, 0f, -ComputeCwDistance(slotIndex)), SpinDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(SpinCurve)
                .OnComplete(() =>
                {
                    if (isBomb) _slotViews[slotIndex].SetSafe();
                    _spinManager.CommitSpinResult();
                    if (isBomb || _gameCompleted) return;

                    _signalBus.Fire(new RewardReadyToFlySignal
                    {
                        SlotTransform = _slotViews[slotIndex].transform,
                        Entry = result
                    });
                });
        }

        private float ComputeCwDistance(int slotIndex)
        {
            float slotAngle = Mathf.Atan2(
                _slotViews[slotIndex].transform.localPosition.y,
                _slotViews[slotIndex].transform.localPosition.x) * Mathf.Rad2Deg;

            float targetZ = ((IndicatorAngle - slotAngle) % 360f + 360f) % 360f;
            float cwDistance = _wheelContent.eulerAngles.z - targetZ;
            if (cwDistance < 10f) cwDistance += 360f;
            return cwDistance + SpinFullRotations * 360f;
        }

        #endregion

        #region Animation

        private void PlayIntro()
        {
            _transition.OnOpened -= PlayIntro;
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, IntroDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(StartIdleAnimation);
        }

        private void GoIdle()
        {
            RefreshSlotData();
            SetSpinning(false);
            StartIdleAnimation();
        }

        private void StartIdleAnimation()
        {
            if (_isSpinning) return;
            _wheelContent.localEulerAngles = Vector3.zero;
            _wheelContent.DORotate(new Vector3(0f, 0f, -360f), IdleRotationDuration, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        #endregion

        #region State

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

        private void HandleReset()
        {
            _gameCompleted = false;
            _wheelContent.DOKill();
            GoIdle();
        }

        private void SetSpinning(bool spinning)
        {
            _isSpinning = spinning;
            _spinButton.interactable = !spinning;
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

        #endregion
    }
}
