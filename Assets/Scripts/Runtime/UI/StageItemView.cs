using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Enums;
using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public class StageItemView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stageText;

        private const float PassedAlpha = 0.3f;

        private ZoneType _zoneType;
        private SO_StageConfig _config;
        private Tween _fadeTween;
        private Tween _activateTween;

        public void Setup(int zoneNumber, ZoneType zoneType, SO_StageConfig config)
        {
            _zoneType = zoneType;
            _config = config;
            stageText.text = zoneNumber.ToString();
            stageText.color = GetColorForZoneType(zoneType, config);
        }

        public void Activate()
        {
            _activateTween?.Kill();
            _activateTween = stageText.transform.DOPunchScale(Vector3.one * 0.35f, 0.4f, 5, 0.5f);
        }

        public void FadeOutToPassed()
        {
            _fadeTween?.Kill();
            Color passedColor = GetColorForZoneType(_zoneType, _config);
            passedColor.a = PassedAlpha;
            _fadeTween = DOVirtual.Color(stageText.color, passedColor, 0.4f, c => stageText.color = c).SetEase(Ease.InQuad);
        }

        public void Restore()
        {
            _fadeTween?.Kill();
            stageText.color = GetColorForZoneType(_zoneType, _config);
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _activateTween?.Kill();
        }

        private Color GetColorForZoneType(ZoneType zoneType, SO_StageConfig config)
        {
            return zoneType switch
            {
                ZoneType.Gold => config.goldColor,
                ZoneType.Silver => config.silverColor,
                _ => config.normalColor
            };
        }
    }
}
