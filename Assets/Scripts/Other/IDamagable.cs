namespace Dispersion.Interface
{
    public interface IDamagable
    {
        public void TakeDamage(float damage, Photon.Realtime.Player killer);
    }
}