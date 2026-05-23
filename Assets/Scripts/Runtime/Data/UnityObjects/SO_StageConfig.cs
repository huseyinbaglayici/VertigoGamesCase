using UnityEngine;

namespace Runtime.Data.UnityObjects
{
    [CreateAssetMenu(menuName = "Config/StageConfig")]
    public class SO_StageConfig : ScriptableObject
    {
        public string silverAreaLabel = "Safe Area";
        public string goldAreaLabel = "Super Area";
        public Color normalColor;
        public Color silverColor;
        public Color goldColor;
    }
}