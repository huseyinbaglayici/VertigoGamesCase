using DG.Tweening;
using Runtime.Data.UnityObjects;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace Runtime.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private SO_GameConfig gameConfig;
        [SerializeField] private SO_AudioConfig audioConfig;

        public override void InstallBindings()
        {
            DOTween.Init();
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameRestartSignal>();
            Container.DeclareSignal<RewardReadyToFlySignal>();
            Container.DeclareSignal<RewardFlyCompleteSignal>();
            Container.BindInstance(gameConfig).AsSingle();
            Container.BindInstance(audioConfig).AsSingle();
            CoreInstaller.Install(Container);
        }
    }
}
