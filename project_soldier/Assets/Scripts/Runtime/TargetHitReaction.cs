using System.Collections;
using UnityEngine;

namespace ProjectSoldier
{
    [RequireComponent(typeof(Renderer))]
    public class TargetHitReaction : MonoBehaviour
    {
        [SerializeField] private Color hitColor = Color.red;
        [SerializeField] private float hitDuration = 0.15f;

        private Renderer _cachedRenderer;
        private Color _baseColor;
        private Coroutine _reactionRoutine;

        private void Awake()
        {
            _cachedRenderer = GetComponent<Renderer>();
            _baseColor = _cachedRenderer.material.color;
        }

        public void PlayReaction()
        {
            if (_reactionRoutine != null)
            {
                StopCoroutine(_reactionRoutine);
            }

            _reactionRoutine = StartCoroutine(PlayReactionRoutine());
        }

        private IEnumerator PlayReactionRoutine()
        {
            _cachedRenderer.material.color = hitColor;
            yield return new WaitForSeconds(hitDuration);
            _cachedRenderer.material.color = _baseColor;
            _reactionRoutine = null;
        }
    }
}
