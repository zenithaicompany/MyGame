using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectSoldier
{
    public class LookDragInputArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private float touchSensitivity = 0.15f;

        private int _activePointerId = int.MinValue;
        private Vector2 _lastScreenPosition;
        private Vector2 _frameDelta;

        public Vector2 ConsumeDelta()
        {
            Vector2 delta = _frameDelta;
            _frameDelta = Vector2.zero;
            return delta;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_activePointerId != int.MinValue)
            {
                return;
            }

            _activePointerId = eventData.pointerId;
            _lastScreenPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            Vector2 current = eventData.position;
            Vector2 delta = current - _lastScreenPosition;
            _lastScreenPosition = current;
            _frameDelta += delta * touchSensitivity;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            _activePointerId = int.MinValue;
            _frameDelta = Vector2.zero;
        }
    }
}
