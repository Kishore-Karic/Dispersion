namespace Dispersion.Weapons.RangedWeapons
{
    public abstract class RangedWeapons : WeaponController
    {
        public abstract override void Use(Photon.Realtime.Player killer);
    }
}