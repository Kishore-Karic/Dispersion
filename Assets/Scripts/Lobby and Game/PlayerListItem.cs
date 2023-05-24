using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

namespace Dispersion.Lobby
{
    public class PlayerListItem : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI playerNameText;

        private Player player;

        public void SetUpPlayer(Player _player)
        {
            player = _player;
            playerNameText.text = _player.NickName;
        }

        public void SetUpName(string _name)
        {
            playerNameText.text = _name;
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if(player == otherPlayer)
            {
                Destroy(gameObject);
            }
        }

        public override void OnLeftRoom()
        {
            Destroy(gameObject);
        }
    }
}