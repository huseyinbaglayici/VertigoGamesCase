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

        public void Setup(int zoneNumber, ZoneType zoneType, SO_StageConfig config)
        {
            stageText.text = zoneNumber.ToString();
            stageText.color = GetColorForZoneType(zoneType, config);
        }

        public void FadeOutToPassed()
        {
            Color from = stageText.color;
            DOVirtual.Color(from, PassedColor, 0.4f, c => stageText.color = c).SetEase(Ease.InQuad);
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