using DG.Tweening;
using Runtime.Core;
using Runtime.Data.UnityObjects;
using Runtime.Interfaces;
using Runtime.UI;
using UnityEngine;
using Zenject;

namespace Runtime.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SO_GameConfig gameConfig;

        public override void InstallBindings()
        {
            DOTween.Init();
            Container.BindInstance(gameConfig).AsSingle();
            Container.BindInstance(new System.Random()).AsSingle();
            Container.Bind<IZoneManager>().To<ZoneManager>().AsSingle();
            Container.Bind<IInventoryManager>().To<InventoryManager>().AsSingle();
            Container.Bind<ISpinManager>().To<SpinManager>().AsSingle();
            Container.Bind<RewardCalculator>().AsSingle();
            Container.Bind<SceneTransitionView>().FromComponentInHierarchy().AsSingle();
        }
    }
}