using UnityEngine;

namespace Dispersion.Weapons
{
    public abstract class WeaponController : MonoBehaviour
    {
        [field: SerializeField] public WeaponInfo weaponInfo { get; private set; }
        [SerializeField] private GameObject weaponGameObject;

        public abstract void Use();
    }
}