using Photon.Pun;
using UnityEngine;

namespace Dispersion.Players
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PhotonView _phototnView;
        [SerializeField] private string playerControllerString;

        private void Start()
        {
            if (_phototnView.IsMine)
            {
                CreateController();
            }
        }

        private void CreateController()
        {
            PhotonNetwork.Instantiate(playerControllerString, Vector3.zero, Quaternion.identity);
        }
    }
}