using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class StageItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stageText;
        [SerializeField] private Image backgroundImage;

        private const float ActivateStrength = 0.35f;
        private const float ActivateDuration = 0.4f;
        private const int ActivateVibrato = 5;
        private const float ActivateElastic = 0.5f;
        private const Ease FadeEase = Ease.InQuad;
        private const float PassedAlpha = 0.3f;

        [SerializeField] private float _fadeDuration = 0.4f;

        private Tween _fadeTween;
        private Tween _activateTween;

        public void Setup(int zoneNumber, ZoneType zoneType, SO_StageConfig config)
        {
            stageText.text = zoneNumber.ToString();

            if (zoneType == ZoneType.Silver)
            {
                backgroundImage.sprite = config.silverIcon;
                backgroundImage.enabled = true;
            }
            else if (zoneType == ZoneType.Gold)
            {
                backgroundImage.sprite = config.goldIcon;
                backgroundImage.enabled = true;
            }
            else backgroundImage.enabled = false;
        }

        public void Activate()
        {
            _activateTween?.Kill();
            _activateTween = stageText.transform.DOPunchScale(
                Vector3.one * ActivateStrength, ActivateDuration, ActivateVibrato, ActivateElastic);
        }

        public void FadeOutToPassed() => FadeTo(PassedAlpha, _fadeDuration);
        public void Restore() => FadeTo(1f);

        private void FadeTo(float targetAlpha, float duration = 0f)
        {
            _fadeTween?.Kill();
            Color target = stageText.color;
            target.a = targetAlpha;
            if (duration > 0f)
                _fadeTween = DOVirtual.Color(stageText.color, target, duration, x => stageText.color = x)
                    .SetEase(FadeEase);
            else
                stageText.color = target;
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _activateTween?.Kill();
        }
    }
}