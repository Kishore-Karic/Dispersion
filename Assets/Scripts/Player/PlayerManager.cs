using Photon.Pun;
using UnityEngine;

namespace Dispersion.Players
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private string playerControllerString;
        [SerializeField] private int zero;

        private GameObject player;

        private void Start()
        {
            if (_photonView.IsMine)
            {
                CreateController();
            }
        }

        private void CreateController()
        {
            player = PhotonNetwork.Instantiate(playerControllerString, Vector3.zero, Quaternion.identity, (byte)zero, new object[] { _photonView.ViewID });
        }

        public void Die()
        {
            PhotonNetwork.Destroy(player);
            CreateController();
        }
    }
}