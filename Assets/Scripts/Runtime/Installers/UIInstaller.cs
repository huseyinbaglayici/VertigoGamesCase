using Runtime.UI;
using UnityEngine;
using Zenject;

namespace Runtime.Installers
{
    public class UIInstaller : MonoInstaller
    {
        [SerializeField] private SceneTransitionView _sceneTransitionView;
        [SerializeField] private WheelView _wheelView;
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private BombPanelView _bombPanelView;
        [SerializeField] private RewardView _rewardView;
        [SerializeField] private StageBarView _stageBarView;
        [SerializeField] private RewardEffectView _rewardEffectView;

        public override void InstallBindings()
        {
            Container.BindInstance(_sceneTransitionView).AsSingle();
            Container.BindInstance(_wheelView).AsSingle();
            Container.BindInstance(_inventoryView).AsSingle();
            Container.BindInstance(_bombPanelView).AsSingle();
            Container.BindInstance(_rewardView).AsSingle();
            Container.BindInstance(_stageBarView).AsSingle();
            Container.BindInstance(_rewardEffectView).AsSingle().NonLazy();
        }
    }
}
