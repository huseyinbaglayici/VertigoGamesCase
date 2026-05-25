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
        public float          spinDuration         = 4f;
        public int            spinExtraRotations   = 2;
        public AnimationCurve spinCurve;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }

        private void Reset()
        {
            spinCurve = new AnimationCurve(
                new Keyframe(0f,   0f,    0f,   0.04f),
                new Keyframe(0.4f, 0.55f, 1.8f, 1.8f),
                new Keyframe(1f,   1f,    0f,   0f)
            );
        }
    }
}
