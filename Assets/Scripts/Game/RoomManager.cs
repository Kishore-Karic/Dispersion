using Dispersion.GenericSingleton;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dispersion.Game
{
    public class RoomManager : GenericSingleton<RoomManager>
    {
        [SerializeField] private int one;
        [SerializeField] private string playerManagerString;

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == one)
            {
                PhotonNetwork.Instantiate(playerManagerString, Vector3.zero, Quaternion.identity);
            }
        }
    }
}