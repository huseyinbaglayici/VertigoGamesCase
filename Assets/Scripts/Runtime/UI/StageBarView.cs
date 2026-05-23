using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using UnityEngine;
using Zenject;

namespace Runtime.UI
{
    public class StageBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform _stagesContent;
        [SerializeField] private GameObject _stagePrefab;
        [SerializeField] private float _slideDuration = 0.3f;
        [SerializeField] private float _punchScale = 0.35f;
        [SerializeField] private float _punchDuration = 0.4f;
        [SerializeField] private int _punchVibrato = 5;
        [SerializeField] private float _punchElasticity = 0.5f;
        [SerializeField] private int _instantiatePerFrame = 5;
        [SerializeField] private RectTransform _currentStageFrame;

        private IZoneManager _zoneManager;
        private ISpinManager _spinManager;
        private SO_GameConfig _gameConfig;
        private SceneTransitionView _transition;

        private readonly List<StageItemView> _stageItems = new();

        [Inject]
        public void Construct(IZoneManager zoneManager, ISpinManager spinManager, SO_GameConfig gameConfig, SceneTransitionView transition)
        {
            _zoneManager = zoneManager;
            _spinManager = spinManager;
            _gameConfig = gameConfig;
            _transition = transition;
        }

        private void Start()
        {
            _zoneManager.OnZoneChanged += OnZoneChanged;
            _spinManager.OnGameResumed += OnGameResumed;
            _transition.OnOpened += PlayIntro;
            StartCoroutine(InstantiateStages());
        }

        private void OnDestroy()
        {
            if (_zoneManager != null)
                _zoneManager.OnZoneChanged -= OnZoneChanged;
            if (_spinManager != null)
                _spinManager.OnGameResumed -= OnGameResumed;
            if (_transition != null)
                _transition.OnOpened -= PlayIntro;
        }

        private void PlayIntro()
        {
            _transition.OnOpened -= PlayIntro;
            StartCoroutine(IntroSlide());
        }

        private IEnumerator IntroSlide()
        {
            yield return new WaitForEndOfFrame();
            SlideToZone(_zoneManager.CurrentZone);
            _stageItems[_zoneManager.CurrentZone - 1].Activate();
            PunchFrame();
        }

        private IEnumerator InstantiateStages()
        {
            for (int i = 1; i <= _gameConfig.goldZoneInterval; i++)
            {
                var go = Instantiate(_stagePrefab, _stagesContent);
                var view = go.GetComponent<StageItemView>();
                view.Setup(i, _zoneManager.GetZoneType(i), _gameConfig.stageConfig);
                _stageItems.Add(view);
                if (i % _instantiatePerFrame == 0) yield return null;
            }
        }

        private void OnZoneChanged(int zone)
        {
            FadeOutPreviousStage(zone);
            SlideToZone(zone);
            _stageItems[zone - 1].Activate();
            PunchFrame();
        }

        private void OnGameResumed()
        {
            int currentIndex = _zoneManager.CurrentZone - 1;
            if (currentIndex >= 0 && currentIndex < _stageItems.Count)
                _stageItems[currentIndex].Restore();
        }

        private void FadeOutPreviousStage(int currentZone)
        {
            int prevIndex = currentZone - 2;
            if (prevIndex >= 0 && prevIndex < _stageItems.Count)
                _stageItems[prevIndex].FadeOutToPassed();
        }

        private void SlideToZone(int zone)
        {
            float targetX = -_stageItems[zone - 1].transform.localPosition.x;
            _stagesContent.DOAnchorPosX(targetX, _slideDuration).SetEase(Ease.OutCubic);
        }

        private void PunchFrame()
        {
            _currentStageFrame.DOPunchScale(new Vector3(_punchScale, _punchScale * 0.5f, 0f), _punchDuration, _punchVibrato, _punchElasticity);
        }
    }
}
