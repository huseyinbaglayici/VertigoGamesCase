using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(menuName = "Config/StageConfig")]
    public class SO_StageConfig : ScriptableObject
    {
        public string silverAreaLabel = "Silver Area";
        public string goldAreaLabel = "Super Area";
        public Sprite silverIcon;
        public Sprite goldIcon;
    }
}