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
        private SO_GameConfig _gameConfig;

        private readonly List<StageItemView> _stageItems = new();

        private float StageWidth => _stagePrefab.GetComponent<RectTransform>().rect.width;
        private float Spacing => _stagesContent.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>().spacing;

        [Inject]
        public void Construct(IZoneManager zoneManager, SO_GameConfig gameConfig)
        {
            _zoneManager = zoneManager;
            _gameConfig = gameConfig;
        }

        private IEnumerator Start()
        {
            yield return InstantiateStages();
            _zoneManager.OnZoneChanged += OnZoneChanged;
            yield return new WaitForEndOfFrame();
            SlideToZone(_zoneManager.CurrentZone);
            PunchFrame();
        }

        private void OnDestroy()
        {
            _zoneManager.OnZoneChanged -= OnZoneChanged;
        }

        private IEnumerator InstantiateStages()
        {
            for (int i = 1; i <= _gameConfig.goldZoneInterval; i++)
            {
                var go = Instantiate(_stagePrefab, _stagesContent);
                var view = go.GetComponent<StageItemView>();
                var zoneType = _zoneManager.GetZoneType(i);
                view.Setup(i, zoneType, _gameConfig.stageConfig);
                _stageItems.Add(view);
                if (i % _instantiatePerFrame == 0) yield return null;
            }
        }

        private void OnZoneChanged(int zone)
        {
            FadeOutPreviousStage(zone);
            SlideToZone(zone);
            PunchFrame();
        }

        private void FadeOutPreviousStage(int currentZone)
        {
            int prevIndex = currentZone - 2;
            if (prevIndex >= 0 && prevIndex < _stageItems.Count)
                _stageItems[prevIndex].FadeOutToPassed();
        }

        private void SlideToZone(int zone)
        {
            float targetX = -((zone - 1) * (StageWidth + Spacing)) - (StageWidth / 2f);
            _stagesContent.DOAnchorPosX(targetX, _slideDuration).SetEase(Ease.OutCubic);
        }

        private void PunchFrame()
        {
            _currentStageFrame.DOPunchScale(Vector3.one * _punchScale, _punchDuration, _punchVibrato, _punchElasticity);
        }
    }
}