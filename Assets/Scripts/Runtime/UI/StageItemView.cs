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

        private static readonly Color PassedColor = new Color(1f, 1f, 1f, 0.3f);

        private ZoneType _zoneType;
        private SO_StageConfig _config;
        private Tween _fadeTween;

        public void Setup(int zoneNumber, ZoneType zoneType, SO_StageConfig config)
        {
            _zoneType = zoneType;
            _config = config;
            stageText.text = zoneNumber.ToString();
            stageText.color = GetColorForZoneType(zoneType, config);
        }

        public void FadeOutToPassed()
        {
            _fadeTween?.Kill();
            Color from = stageText.color;
            _fadeTween = DOVirtual.Color(from, PassedColor, 0.4f, c => stageText.color = c).SetEase(Ease.InQuad);
        }

        public void Restore()
        {
            _fadeTween?.Kill();
            stageText.color = GetColorForZoneType(_zoneType, _config);
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