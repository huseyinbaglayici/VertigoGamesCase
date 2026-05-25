using Runtime.Core;
using Zenject;

namespace Runtime.Installers
{
    public class CoreInstaller : Installer<CoreInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInstance(new System.Random()).AsSingle();
            Container.Bind<RewardCalculator>().AsSingle();
            Container.BindInterfacesAndSelfTo<ZoneManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<SpinManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InventoryManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<CurrencyManager>().AsSingle();
        }
    }
}