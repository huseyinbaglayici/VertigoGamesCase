using DG.Tweening;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace Runtime.Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            DOTween.Init();
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameRestartSignal>();
            Container.DeclareSignal<RewardReadyToFlySignal>();
            Container.DeclareSignal<RewardFlyCompleteSignal>();
            Container.DeclareSignal<ScrollToItemRequestSignal>();
            Container.DeclareSignal<ScrollToItemCompleteSignal>();
            Container.DeclareSignal<GoldZoneEnteredSignal>();
            Container.DeclareSignal<GoldZoneAcknowledgedSignal>();
            CoreInstaller.Install(Container);
        }
    }
}