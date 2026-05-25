using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class RewardItemView : MonoBehaviour
    {
        private const float FullRotation = 360f;

        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _amountText;

        [SerializeField] private Image[] _particles;

        private SO_RewardAnimationConfig.ItemSpreadBurst _cfg;

        public void Init(ItemData item, SO_RewardAnimationConfig.ItemSpreadBurst cfg)
        {
            _cfg = cfg;
            _icon.sprite = item.Config.icon;
            _nameText.text = item.Config.itemName;
            _amountText.text = $"x{item.Amount}";

            foreach (var p in _particles)
                p.sprite = item.Config.icon;
        }

        public void PlaySpreadEffect()
        {
            float angleStep = FullRotation / _particles.Length;
            for (int i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                var t = particle.transform;
                t.localPosition = Vector3.zero;
                particle.gameObject.SetActive(true);

                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 target = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _cfg.spreadRadius;

                t.DOKill();
                var seq = DOTween.Sequence();
                seq.Append(t.DOLocalMove(target, _cfg.spreadDuration).SetEase(Ease.OutQuad));
                seq.Append(t.DOLocalMove(Vector3.zero, _cfg.gatherDuration).SetEase(Ease.InQuad));
                seq.OnComplete(() => particle.gameObject.SetActive(false));
            }
        }

        private void OnDestroy()
        {
            foreach (var p in _particles) p.transform.DOKill();
        }
    }
}