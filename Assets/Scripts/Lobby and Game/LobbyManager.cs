using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;

namespace Dispersion.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject loadingLayer, joinLayer, menuLayer, createRoomLayer, joinRoomLayer, roomLayer;
        [SerializeField] private Button connectButton, createRoomButton, createAndJoinRoomButton, joinRoomButton, quitButton, backButton, leaveRoomButton, startGameButton;
        [SerializeField] private TMP_InputField roomNameText, playerNameText;
        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private GameObject roomInfoPrefab, roomInfoParentGameobject, playerListItem, playerParent;
        [SerializeField] private string slashString;
        [SerializeField] private int maxPlayerPerRoom, zero, one, minNameTextValue, maxNameTextValue;

        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListGameobjects;

        private void Awake()
        {
            connectButton.onClick.AddListener(ConnectToLobby);
            createRoomButton.onClick.AddListener(ShowCreateRoomLayer);
            createAndJoinRoomButton.onClick.AddListener(CreateAndJoinRoom);
            joinRoomButton.onClick.AddListener(ShowRoomList);
            backButton.onClick.AddListener(GoBackToMainLobby);
            leaveRoomButton.onClick.AddListener(LeaveRoom);
            startGameButton.onClick.AddListener(StartGame);

            cachedRoomList = new Dictionary<string, RoomInfo>();
            roomListGameobjects = new Dictionary<string, GameObject>();

            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnConnectedToMaster()
        {
            loadingLayer.SetActive(false);
        }

        public override void OnJoinedLobby()
        {
            joinLayer.SetActive(false);
            menuLayer.SetActive(true);
        }

        public override void OnCreatedRoom()
        {
            if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                PhotonNetwork.JoinRoom(roomNameText.text);
            }
        }

        public override void OnJoinedRoom()
        {
            roomName.text = PhotonNetwork.CurrentRoom.Name;
            playerNameText.gameObject.SetActive(false);
            roomLayer.SetActive(true);
            leaveRoomButton.gameObject.SetActive(true);
            joinRoomLayer.SetActive(false);
            backButton.gameObject.SetActive(false);
            
            Player[] players = PhotonNetwork.PlayerList;
            for(int i = zero; i < players.Length - one; i++)
            {
                Instantiate(playerListItem, playerParent.transform).GetComponent<PlayerListItem>().SetUpPlayer(players[i]);
            }
            
            Instantiate(playerListItem, playerParent.transform).GetComponent<PlayerListItem>().SetUpPlayer(PhotonNetwork.LocalPlayer);
            loadingLayer.SetActive(false);

            startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            foreach (RoomInfo room in roomList)
            {
                if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
                {
                    if (cachedRoomList.ContainsKey(room.Name))
                    {
                        cachedRoomList.Remove(room.Name);
                    }
                }
                else
                {
                    if (cachedRoomList.ContainsKey(room.Name))
                    {
                        cachedRoomList[room.Name] = room;
                    }
                    else
                    {
                        cachedRoomList.Add(room.Name, room);

                    }
                }
            }

            foreach (RoomInfo room in cachedRoomList.Values)
            {
                GameObject roomListEntryGameobject = Instantiate(roomInfoPrefab);
                roomListEntryGameobject.transform.SetParent(roomInfoParentGameobject.transform);
                roomListEntryGameobject.transform.localScale = Vector3.one;
                RoomInfoButton joinRoom = roomListEntryGameobject.GetComponent<RoomInfoButton>();

                joinRoom.roomNameText.text = room.Name;
                joinRoom.playersAvailableText.text = room.PlayerCount + slashString + room.MaxPlayers;
                joinRoom.joinButton.onClick.AddListener(() => JoinRoomButtonClicked(room.Name));

                roomListGameobjects.Add(room.Name, roomListEntryGameobject);
            }
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Instantiate(playerListItem, playerParent.transform).GetComponent<PlayerListItem>().SetUpPlayer(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer.IsMasterClient)
            {
                PhotonNetwork.LeaveRoom();
            }
        }

        public override void OnLeftRoom()
        {
            roomLayer.SetActive(false);
            menuLayer.SetActive(true);
            playerNameText.gameObject.SetActive(true);
        }

        private void ClearRoomListView()
        {
            foreach (var roomListGameobject in roomListGameobjects.Values)
            {
                Destroy(roomListGameobject);
            }

            roomListGameobjects.Clear();
        }

        private void ConnectToLobby()
        {
            if (playerNameText.text.Length > zero)
            {
                PhotonNetwork.LocalPlayer.NickName = playerNameText.text;
            }
            else
            {
                PhotonNetwork.LocalPlayer.NickName = Random.Range(minNameTextValue, maxNameTextValue).ToString();
            }
            PhotonNetwork.JoinLobby();
        }

        private void CreateAndJoinRoom()
        {
            loadingLayer.SetActive(true);
            if (roomNameText.text.Length > zero)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayerPerRoom;

                PhotonNetwork.CreateRoom(roomNameText.text, roomOptions);
                createRoomLayer.SetActive(false);
            }
        }

        private void JoinRoomButtonClicked(string _roomName)
        {
            loadingLayer.SetActive(true);
            PhotonNetwork.JoinRoom(_roomName);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void StartGame()
        {
            PhotonNetwork.LoadLevel(one);
        }

        private void GoBackToMainLobby()
        {
            createRoomLayer.SetActive(false);
            joinRoomLayer.SetActive(false);
            menuLayer.SetActive(true);
            backButton.gameObject.SetActive(false);
        }

        private void ShowRoomList()
        {
            menuLayer.SetActive(false);
            joinRoomLayer.SetActive(true);
            backButton.gameObject.SetActive(true);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        private void ShowCreateRoomLayer()
        {
            menuLayer.SetActive(false);
            createRoomLayer.SetActive(true);
            backButton.gameObject.SetActive(true);
        }
    }
}