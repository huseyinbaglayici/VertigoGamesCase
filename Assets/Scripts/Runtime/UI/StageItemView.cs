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

        private const float PassedAlpha = 0.3f;

        private Tween _fadeTween;
        private Tween _activateTween;

        public void Setup(int zoneNumber, ZoneType zoneType, SO_StageConfig config)
        {
            stageText.text = zoneNumber.ToString();

            if (zoneType == ZoneType.Silver) { backgroundImage.sprite = config.silverIcon; backgroundImage.enabled = true; }
            else if (zoneType == ZoneType.Gold) { backgroundImage.sprite = config.goldIcon; backgroundImage.enabled = true; }
            else backgroundImage.enabled = false;
        }

        public void Activate()
        {
            _activateTween?.Kill();
            _activateTween = stageText.transform.DOPunchScale(Vector3.one * 0.35f, 0.4f, 5, 0.5f);
        }

        public void FadeOutToPassed()
        {
            _fadeTween?.Kill();
            Color c = stageText.color;
            c.a = PassedAlpha;
            _fadeTween = DOVirtual.Color(stageText.color, c, 0.4f, x => stageText.color = x).SetEase(Ease.InQuad);
        }

        public void Restore()
        {
            _fadeTween?.Kill();
            Color c = stageText.color;
            c.a = 1f;
            stageText.color = c;
        }

        private void OnDestroy()
        {
            _fadeTween?.Kill();
            _activateTween?.Kill();
        }


    }
}
