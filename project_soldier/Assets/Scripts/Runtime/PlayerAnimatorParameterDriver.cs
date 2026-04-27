using UnityEngine;

namespace ProjectSoldier
{
    [DisallowMultipleComponent]
    public class PlayerAnimatorParameterDriver : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerM1Controller playerController;
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private float referenceMoveSpeed = 5f;
        [SerializeField] private float speedDampTime = 0.08f;

        private int _speedHash;

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

            _speedHash = Animator.StringToHash(speedParameter);
        }

        private void Update()
        {
            if (animator == null)
            {
                return;
            }

            float speed = playerController != null ? playerController.CurrentPlanarSpeed : 0f;
            float normalizedSpeed = Mathf.Clamp01(speed / Mathf.Max(0.01f, referenceMoveSpeed));
            animator.SetFloat(_speedHash, normalizedSpeed, speedDampTime, Time.deltaTime);
        }
    }
}
