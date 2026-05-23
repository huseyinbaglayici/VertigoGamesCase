using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime.UI
{
    public class SceneTransitionView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.3f;

        public event Action OnOpened;
        public bool HasOpened { get; private set; }

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
        }

        private void Start()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.DOFade(0f, _fadeDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => { HasOpened = true; OnOpened?.Invoke(); });
        }

        public void FadeAndLoad()
        {
            _canvasGroup.DOFade(1f, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
        }

        private void OnDestroy() => _canvasGroup.DOKill();
    }
}
