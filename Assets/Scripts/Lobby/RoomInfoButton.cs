using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dispersion.Lobby
{
    public class RoomInfoButton : MonoBehaviour
    {
        public TextMeshProUGUI roomNameText;
        public TextMeshProUGUI playersAvailableText;
        public Button joinButton;
        public string slashString;
        public RoomInfo info;

        public void SetUpRoom(RoomInfo _info)
        {
            info = _info;
            roomNameText.text = _info.Name;
            playersAvailableText.text = _info.PlayerCount + slashString + _info.MaxPlayers;
        }

        public void OnClick()
        {
            LobbyManager.Instance.JoinRoomButtonClicked(info);
        }
    }
}