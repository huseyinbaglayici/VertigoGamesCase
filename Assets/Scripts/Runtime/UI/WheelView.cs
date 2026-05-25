using System;
using System.Collections;
using DG.Tweening;
using Runtime.Core;
using Runtime.Data.UnityObjects;
using Runtime.Enums;
using Runtime.Interfaces;
using Runtime.Signals;
using Runtime.Utility;
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

        private const float IndicatorAngle = 90f;
        private const float MinSpinDistance = 10f;
        private const float FullRotation = 360f;

        private const float SpinButtonPunchStrength = 0.1f;
        private const float SpinButtonPunchDuration = 0.2f;
        private const int SpinButtonPunchVibrato = 1;
        private const float SpinButtonPunchElasticity = 0.3f;

        private ISpinManager _spinManager;
        private IZoneManager _zoneManager;
        private RewardCalculator _rewardCalculator;
        private SO_WheelAnimationConfig _animCfg;
        private SceneTransitionView _transition;
        private SignalBus _signalBus;
        private IAudioService _audioService;
        private bool _isSpinning;
        private bool _gameCompleted;
        private Coroutine _hapticCoroutine;

        #endregion

        #region Lifecycle

        [Inject]
        public void Construct(ISpinManager spinManager, IZoneManager zoneManager, RewardCalculator rewardCalculator,
            SO_WheelAnimationConfig animConfig, SceneTransitionView transition, SignalBus signalBus,
            IAudioService audioService)
        {
            _spinManager = spinManager;
            _zoneManager = zoneManager;
            _rewardCalculator = rewardCalculator;
            _animCfg = animConfig;
            _transition = transition;
            _signalBus = signalBus;
            _audioService = audioService;
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
            _signalBus.Subscribe<GoldZoneAcknowledgedSignal>(HandleGoldZoneAcknowledged);
            yield return null;
            RefreshSlotData();
            _transition.OnOpened += PlayIntro;
            if (_transition.HasOpened)
            {
                _transition.OnOpened -= PlayIntro;
                PlayIntro();
            }
        }

        private void LateUpdate()
        {
            foreach (var slot in _slotViews)
                slot.LockRotation();
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
                _signalBus.Unsubscribe<GoldZoneAcknowledgedSignal>(HandleGoldZoneAcknowledged);
            }

            StopHaptic();
            transform.DOKill();
            _wheelContent.DOKill();
        }

        #endregion

        #region Spin

        private void OnSpinClicked()
        {
            if (_isSpinning) return;
            HapticFeedback.Play(HapticFeedback.HapticType.Light);
            SetSpinning(true);
            transform.DOKill();
            transform.DOPunchScale(Vector3.one * SpinButtonPunchStrength, SpinButtonPunchDuration,
                SpinButtonPunchVibrato, SpinButtonPunchElasticity);
            _spinManager.Spin();
        }

        private void HandleSpinDecision(int slotIndex, RewardEntry result)
        {
            _wheelContent.DOKill();
            StopHaptic();

            bool isBomb = result.item.IsBomb;
            _hapticCoroutine = StartCoroutine(SpinHapticCoroutine());

            _wheelContent.DORotate(new Vector3(0f, 0f, -ComputeCwDistance(slotIndex)), _animCfg.spinDuration,
                    RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(_animCfg.spinEase)
                .OnComplete(() => OnSpinComplete(slotIndex, result, isBomb));
        }

        private void OnSpinComplete(int slotIndex, RewardEntry result, bool isBomb)
        {
            StopHaptic();
            HapticFeedback.Play(isBomb ? HapticFeedback.HapticType.Heavy : HapticFeedback.HapticType.Medium);

            if (isBomb)
            {
                _audioService.PlayBombSfx();
                _slotViews[slotIndex].SetSafe();
            }
            else
            {
                bool isGold = _zoneManager.GetZoneType(_zoneManager.CurrentZone) == ZoneType.Gold;
                if (isGold) _audioService.PlayGoldRewardSfx();
                else _audioService.PlayRewardSfx();
            }

            _spinManager.CommitSpinResult();
            if (isBomb || _gameCompleted) return;

            _signalBus.Fire(new RewardReadyToFlySignal
            {
                SlotTransform = _slotViews[slotIndex].transform,
                Entry = result
            });
        }

        private void StopHaptic()
        {
            if (_hapticCoroutine == null) return;
            StopCoroutine(_hapticCoroutine);
            _hapticCoroutine = null;
        }

        private IEnumerator SpinHapticCoroutine()
        {
            float slotStep = FullRotation / _slotViews.Length;
            float accumulated = 0f;
            float nextThreshold = slotStep;
            float prevAngle = _wheelContent.eulerAngles.z;

            while (true)
            {
                yield return null;
                float currentAngle = _wheelContent.eulerAngles.z;
                float delta = Mathf.DeltaAngle(currentAngle, prevAngle); // positive = CW
                if (delta > 0f) accumulated += delta;
                prevAngle = currentAngle;

                while (accumulated >= nextThreshold)
                {
                    HapticFeedback.Play(HapticFeedback.HapticType.Light);
                    _audioService.PlaySpinTick();
                    nextThreshold += slotStep;
                }
            }
        }

        private float ComputeCwDistance(int slotIndex)
        {
            float slotAngle = Mathf.Atan2(
                _slotViews[slotIndex].transform.localPosition.y,
                _slotViews[slotIndex].transform.localPosition.x) * Mathf.Rad2Deg;

            float targetZ = ((IndicatorAngle - slotAngle) % FullRotation + FullRotation) % FullRotation;
            float cwDistance = _wheelContent.eulerAngles.z - targetZ;
            if (cwDistance < MinSpinDistance) cwDistance += FullRotation;
            return cwDistance + _animCfg.spinExtraRotations * FullRotation;
        }

        #endregion

        #region Animation

        private void PlayIntro()
        {
            _transition.OnOpened -= PlayIntro;
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, _animCfg.introDuration)
                .SetEase(_animCfg.introEase)
                .OnComplete(StartIdleAnimation);
        }

        private void GoIdle()
        {
            _wheelContent.DOKill();
            _wheelContent.localRotation = Quaternion.identity;
            RefreshSlotData();
            SetSpinning(false);
            StartIdleAnimation();

            if (!_gameCompleted && _zoneManager.GetZoneType(_zoneManager.CurrentZone) == ZoneType.Gold)
            {
                _spinButton.interactable = false;
                _signalBus.Fire<GoldZoneEnteredSignal>();
            }
        }

        private void HandleGoldZoneAcknowledged()
        {
            if (!_isSpinning) _spinButton.interactable = true;
        }

        private void StartIdleAnimation()
        {
            if (_isSpinning) return;
            _wheelContent.DORotate(new Vector3(0f, 0f, -FullRotation), _animCfg.idleRotationDuration,
                    RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(_animCfg.idleEase)
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
            int slotCount = Mathf.Min(_slotViews.Length, rewardSet.rewards.Length);
            for (int i = 0; i < slotCount; i++)
            {
                var entry = rewardSet.rewards[i];
                int amount = entry.item.IsBomb
                    ? 0
                    : _rewardCalculator.Calculate(entry.minAmount, entry.maxAmount, _zoneManager.CurrentZone);
                _slotViews[i].Setup(entry, amount);
            }
        }

        #endregion
    }
}