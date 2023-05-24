using Photon.Pun;

namespace Dispersion.GenericSingleton
{
    public class GenericSingleton<T> : MonoBehaviourPunCallbacks where T : GenericSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = (T)this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}