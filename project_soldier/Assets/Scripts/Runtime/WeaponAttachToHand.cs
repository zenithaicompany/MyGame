using UnityEngine;

namespace ProjectSoldier
{
    public class WeaponAttachToHand : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private HumanBodyBones attachBone = HumanBodyBones.RightHand;
        [SerializeField] private Vector3 localPositionOffset = new Vector3(0.02f, 0.02f, 0.08f);
        [SerializeField] private Vector3 localRotationOffset = new Vector3(0f, 90f, 90f);
        [SerializeField] private Vector3 localScale = new Vector3(0.65f, 0.65f, 0.65f);

        private GameObject _spawnedWeapon;

        private void Awake()
        {
            if (targetAnimator == null)
            {
                targetAnimator = GetComponent<Animator>();
            }

            if (targetAnimator == null || weaponPrefab == null)
            {
                return;
            }

            Transform hand = targetAnimator.GetBoneTransform(attachBone);
            if (hand == null)
            {
                return;
            }

            _spawnedWeapon = Instantiate(weaponPrefab, hand);
            _spawnedWeapon.name = "EquippedWeapon";
            _spawnedWeapon.transform.localPosition = localPositionOffset;
            _spawnedWeapon.transform.localRotation = Quaternion.Euler(localRotationOffset);
            _spawnedWeapon.transform.localScale = localScale;
        }
    }
}
