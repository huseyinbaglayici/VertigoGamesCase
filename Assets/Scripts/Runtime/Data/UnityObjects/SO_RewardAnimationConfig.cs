using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(fileName = "RewardAnimationConfig", menuName = "Game/Reward Animation Config")]
    public class SO_RewardAnimationConfig : ScriptableObjectInstaller
    {
        public ItemPopIn          itemPopIn;
        public ParticleScatterFly particleScatterFly;
        public ItemSpreadBurst    itemSpreadBurst;

        public override void InstallBindings()
        {
            Container.BindInstance(this).AsSingle();
        }

        [Serializable]
        public class ItemPopIn
        {
            public float duration = 0.3f;
            public float interval = 0.08f;
            public Ease  ease     = Ease.OutBack;
        }

        [Serializable]
        public class ParticleScatterFly
        {
            [Header("Scatter")]
            public float spreadRadius    = 30f;
            public float scatterDuration = 0.2f;
            public Ease  scatterEase     = Ease.OutBack;

            [Header("Jump")]
            public float jumpHeight   = 60f;
            public float jumpDuration = 0.15f;

            [Header("Scroll")]
            public float scrollDuration = 0.3f;

            [Header("Fly")]
            public float flyDuration = 0.45f;
            public float flyStagger  = 0.05f;
            public Ease  flyEase     = Ease.InQuad;
        }

        [Serializable]
        public class ItemSpreadBurst
        {
            public float spreadRadius   = 24f;
            public float spreadDuration = 0.15f;
            public float gatherDuration = 0.12f;
        }
    }
}
