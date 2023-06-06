using Dispersion.Enum;
using UnityEngine;

namespace Dispersion.Weapons
{
    public abstract class WeaponController : MonoBehaviour
    {
        [field: SerializeField] public WeaponInfo weaponInfo { get; private set; }
        [SerializeField] private GameObject weaponGameObject;
        [SerializeField] protected Animator animator;
        [SerializeField] protected float nextAttackTime;

        public Sounds SoundType;

        public abstract void Use(Photon.Realtime.Player killer);
    }
}