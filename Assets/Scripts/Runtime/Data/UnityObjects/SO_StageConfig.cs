using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(menuName = "Config/StageConfig")]
    public class SO_StageConfig : ScriptableObject
    {
        public Color normalColor;
        public Color silverColor;
        public Color goldColor;
    }
}