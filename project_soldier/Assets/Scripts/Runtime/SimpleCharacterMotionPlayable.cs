using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace ProjectSoldier
{
    public class SimpleCharacterMotionPlayable : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerM1Controller playerController;
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip moveClip;
        [SerializeField] private float moveThreshold = 0.1f;
        [SerializeField] private float referenceMoveSpeed = 5f;
        [SerializeField] private float minMovePlaybackSpeed = 0.85f;
        [SerializeField] private float maxMovePlaybackSpeed = 1.25f;

        private PlayableGraph _graph;
        private AnimationMixerPlayable _mixer;
        private AnimationClipPlayable _movePlayable;
        private bool _isInitialized;

        private void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerM1Controller>();
            }

            if (animator == null || idleClip == null || moveClip == null)
            {
                enabled = false;
                return;
            }

            animator.applyRootMotion = false;
            _graph = PlayableGraph.Create("SimpleCharacterMotionPlayable");
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

            AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "Animation", animator);
            _mixer = AnimationMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_mixer);

            AnimationClipPlayable idlePlayable = AnimationClipPlayable.Create(_graph, idleClip);
            _movePlayable = AnimationClipPlayable.Create(_graph, moveClip);
            idlePlayable.SetApplyFootIK(false);
            _movePlayable.SetApplyFootIK(false);

            _graph.Connect(idlePlayable, 0, _mixer, 0);
            _graph.Connect(_movePlayable, 0, _mixer, 1);
            _mixer.SetInputWeight(0, 1f);
            _mixer.SetInputWeight(1, 0f);

            _graph.Play();
            _isInitialized = true;
        }

        private void Update()
        {
            if (!_isInitialized)
            {
                return;
            }

            float speed = playerController != null ? playerController.CurrentPlanarSpeed : 0f;
            float moveWeight = speed > moveThreshold ? 1f : 0f;
            _mixer.SetInputWeight(0, 1f - moveWeight);
            _mixer.SetInputWeight(1, moveWeight);

            float normalized = Mathf.Clamp01(speed / Mathf.Max(0.01f, referenceMoveSpeed));
            float playback = Mathf.Lerp(minMovePlaybackSpeed, maxMovePlaybackSpeed, normalized);
            _movePlayable.SetSpeed(playback);
        }

        private void OnDisable()
        {
            if (_isInitialized && _graph.IsValid())
            {
                _graph.Destroy();
            }
            _isInitialized = false;
        }
    }
}
