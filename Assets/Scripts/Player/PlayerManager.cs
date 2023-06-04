using Dispersion.Game;
using Photon.Pun;
using UnityEngine;

namespace Dispersion.Players
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private string playerControllerString;
        [SerializeField] private int zero;

        public GameObject player;

        private void Start()
        {
            if (_photonView.IsMine)
            {
                CreateController();
            }
        }

        public Transform GetSpawnPoint()
        {
            return SpawnManager.Instance.GetRandomSpawnPoint();
        }

        private void CreateController()
        {
            Transform spawnPoint = GetSpawnPoint();

            player = PhotonNetwork.Instantiate(playerControllerString, spawnPoint.position, spawnPoint.rotation, (byte)zero, new object[] { _photonView.ViewID });
        }

        public void DieAndSpawn()
        {
            player.SetActive(false);
            Transform spawnPoint = GetSpawnPoint();
            player.transform.position = spawnPoint.position;
            player.transform.rotation = spawnPoint.rotation;
            player.SetActive(true);
        }
    }
}