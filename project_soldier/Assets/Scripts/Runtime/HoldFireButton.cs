using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectSoldier
{
    public class HoldFireButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private int _activePointerId = int.MinValue;
        public bool IsHeld { get; private set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerId != int.MinValue)
            {
                return;
            }

            _activePointerId = eventData.pointerId;
            IsHeld = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            _activePointerId = int.MinValue;
            IsHeld = false;
        }
    }
}
