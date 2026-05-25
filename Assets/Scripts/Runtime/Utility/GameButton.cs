using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.Utility
{
    public class GameButton : Button
    {
        private const float ClickCooldown = 0.3f;

        private bool _clicked;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (_clicked) return;
            _clicked = true;
            base.OnPointerClick(eventData);
            Invoke(nameof(ResetClick), ClickCooldown);
        }

        private void ResetClick() => _clicked = false;
    }
}