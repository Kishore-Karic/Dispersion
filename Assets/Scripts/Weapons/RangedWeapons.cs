using Dispersion.Enum;
using Dispersion.Interface;
using Dispersion.Sound;
using UnityEngine;

namespace Dispersion.Weapons.RangedWeapons
{
    public class RangedWeapons : WeaponController
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float rayXAxis, rayYAxis;

        private float waitTime;
        private float defaultTime;
        private bool attackAvailable;

        private void Update()
        {
            waitTime += Time.deltaTime;
            if (waitTime > nextAttackTime)
            {
                attackAvailable = true;
            }
        }

        public override void Use(Photon.Realtime.Player killer)
        {
            if (attackAvailable)
            {
                attackAvailable = false;
                waitTime = defaultTime;
                Shoot(killer);
            }
        }

        private void Shoot(Photon.Realtime.Player killer)
        {
            animator.SetTrigger("Attack");
            SoundManager.Instance.PlayEffects(SoundType);

            Ray ray = cam.ViewportPointToRay(new Vector3(rayXAxis, rayYAxis));
            ray.origin = cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(weaponInfo.damage, killer);
            }
        }
    }
}