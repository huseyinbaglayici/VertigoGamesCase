using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "WheelAnimationConfig", menuName = "Game/Wheel Animation Config")]
    public class SO_WheelAnimationConfig : ScriptableObjectInstaller
    {
        public float          idleRotationDuration = 40f;
        public Ease           idleEase             = Ease.Linear;
        public float          introDuration        = 0.5f;
        public Ease           introEase            = Ease.OutBack;
        public float spinDuration       = 4f;
        public int   spinExtraRotations = 2;
        public Ease  spinEase           = Ease.InOutQuad;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }
    }
}
