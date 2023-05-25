using Dispersion.Interface;
using UnityEngine;

namespace Dispersion.Weapons.RangedWeapons
{
    public class MultiShotWeapon : RangedWeapons
    {
        [SerializeField] private Camera cam;
        [SerializeField] private float rayXAxis, rayYAxis;

        public override void Use(Photon.Realtime.Player killer)
        {
            Shoot(killer);
        }

        private void Shoot(Photon.Realtime.Player killer)
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(rayXAxis, rayYAxis));
            ray.origin = cam.transform.position;
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                hit.collider.gameObject.GetComponent<IDamagable>()?.TakeDamage(weaponInfo.damage, killer);
            }
        }
    }
}