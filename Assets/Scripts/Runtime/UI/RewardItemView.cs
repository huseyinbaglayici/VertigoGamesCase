using DG.Tweening;
using Runtime.Data.ValueObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class RewardItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _amountText;

        [Header("Spread Effect")]
        [SerializeField] private Image[] _particles;
        [SerializeField] private float _spreadRadius   = 24f;
        [SerializeField] private float _spreadDuration = 0.15f;
        [SerializeField] private float _gatherDuration = 0.12f;

        public void Init(ItemData item)
        {
            _icon.sprite = item.Config.icon;
            _nameText.text = item.Config.itemName;
            _amountText.text = $"x{item.Amount}";

            foreach (var p in _particles)
                p.sprite = item.Config.icon;
        }

        public void PlaySpreadEffect()
        {
            float angleStep = 360f / _particles.Length;
            for (int i = 0; i < _particles.Length; i++)
            {
                var particle = _particles[i];
                var t = particle.transform;
                t.localPosition = Vector3.zero;
                particle.gameObject.SetActive(true);

                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 target = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * _spreadRadius;

                t.DOKill();
                var seq = DOTween.Sequence();
                seq.Append(t.DOLocalMove(target, _spreadDuration).SetEase(Ease.OutQuad));
                seq.Append(t.DOLocalMove(Vector3.zero, _gatherDuration).SetEase(Ease.InQuad));
                seq.OnComplete(() => particle.gameObject.SetActive(false));
            }
        }

        private void OnDestroy()
        {
            foreach (var p in _particles) p.transform.DOKill();
        }
    }
}