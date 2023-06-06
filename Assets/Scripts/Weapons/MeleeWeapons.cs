using Dispersion.Enum;
using Dispersion.Interface;
using Dispersion.Sound;
using UnityEngine;

namespace Dispersion.Weapons.MeleeWeapons
{
    public class MeleeWeapons : WeaponController
    {
        [SerializeField] private Transform attackPoint;
        [SerializeField] private float attackRange;

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

            Collider[] hit = Physics.OverlapSphere(attackPoint.position, attackRange);

            foreach (Collider Object in hit)
            {
                Object.gameObject.GetComponent<IDamagable>()?.TakeDamage(weaponInfo.damage, killer);
            }
        }
    }
}