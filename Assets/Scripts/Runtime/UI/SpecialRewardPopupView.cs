using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using Runtime.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Runtime.UI
{
    public class SpecialRewardPopupView : MonoBehaviour
    {
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private Button _collectButton;
        [SerializeField] private Transform _effectImage;


        private const string ContinueButtonName = "ui_button_reward_collect";

        private const float BreathTargetScale = 1.06f;
        private const float BreathDuration    = 1.2f;
        private const float FlipYMaxAngle     = 25f;
        private const float FlipYDuration     = 2.5f;
        private const float FlipXMaxAngle     = 8f;
        private const float FlipXDuration     = 1.8f;

        private void OnValidate()
        {
            if (_collectButton != null) return;
            foreach (var button in GetComponentsInChildren<Button>(true))
            {
                if (button.gameObject.name == ContinueButtonName)
                {
                    _collectButton = button;
                    break;
                }
            }
        }

        private SignalBus _signalBus;
        private SO_GameConfig _gameConfig;
        private IAudioService _audioService;

        [Inject]
        public void Construct(SignalBus signalBus, SO_GameConfig gameConfig, IAudioService audioService)
        {
            _signalBus = signalBus;
            _gameConfig = gameConfig;
            _audioService = audioService;
            _signalBus.Subscribe<GoldZoneEnteredSignal>(Show);
            _signalBus.Subscribe<GameRestartSignal>(Hide);

            if (_collectButton == null)
                foreach (var button in GetComponentsInChildren<Button>(true))
                    if (button.gameObject.name == ContinueButtonName)
                    {
                        _collectButton = button;
                        break;
                    }

            _collectButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnDestroy()
        {
            if (_signalBus != null)
            {
                _signalBus.Unsubscribe<GoldZoneEnteredSignal>(Show);
                _signalBus.Unsubscribe<GameRestartSignal>(Hide);
            }

            _collectButton.onClick.RemoveListener(OnContinueClicked);
            if (_effectImage != null) _effectImage.DOKill();
            if (_itemIcon != null) _itemIcon.transform.DOKill();
        }

        private void Show()
        {
            var item = _gameConfig.goldWheel?.specialItem;
            if (item == null)
            {
                _signalBus.Fire<GoldZoneAcknowledgedSignal>();
                return;
            }

            if (_itemIcon != null) _itemIcon.sprite = item.icon;
            if (_itemNameText != null) _itemNameText.text = item.itemName;

            gameObject.SetActive(true);
            PlayBreathAnimation();
            _audioService.PlaySpecialItemOpenSfx();
        }

        private void PlayBreathAnimation()
        {
            if (_effectImage != null)
            {
                _effectImage.DOKill();
                _effectImage.localScale = Vector3.one;
                _effectImage.DOScale(BreathTargetScale, BreathDuration)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            }

            if (_itemIcon != null)
            {
                var iconT = _itemIcon.transform;
                iconT.DOKill();
                iconT.localRotation = Quaternion.identity;

                float xAngle = 0f, yAngle = 0f;
                DOTween.To(() => yAngle, v =>
                    {
                        yAngle = v;
                        iconT.localRotation = Quaternion.Euler(xAngle, yAngle, 0f);
                    }, FlipYMaxAngle, FlipYDuration)
                    .From(-FlipYMaxAngle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                DOTween.To(() => xAngle, v =>
                    {
                        xAngle = v;
                        iconT.localRotation = Quaternion.Euler(xAngle, yAngle, 0f);
                    }, FlipXMaxAngle, FlipXDuration)
                    .From(-FlipXMaxAngle).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void Hide()
        {
            if (_effectImage != null) _effectImage.DOKill();
            if (_itemIcon != null) _itemIcon.transform.DOKill();
            gameObject.SetActive(false);
        }

        private void OnContinueClicked()
        {
            _audioService.PlayCollectSfx();
            Hide();
            _signalBus.Fire<GoldZoneAcknowledgedSignal>();
        }
    }
}