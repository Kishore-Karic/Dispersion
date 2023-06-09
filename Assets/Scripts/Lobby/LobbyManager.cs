using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;
using Dispersion.Game;

namespace Dispersion.Lobby
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameObject loadingLayer, joinLayer, menuLayer, createRoomLayer, joinRoomLayer, roomLayer;
        [SerializeField] private Button connectButton, createRoomButton, createAndJoinRoomButton, joinRoomButton, quitButton, backButton, leaveRoomButton, startGameButton;
        [SerializeField] private TMP_InputField roomNameText, playerNameText;
        [SerializeField] private TextMeshProUGUI roomName, weaponsText, totalPointText;
        [SerializeField] private GameObject roomInfoPrefab, roomInfoParent, playerListItem, playerParent, masterLayer;
        [SerializeField] private string slashString, weaponEndText, totalPointsString;
        [SerializeField] private int maxPlayerPerRoom, zero, one, minNameTextValue, maxNameTextValue;
        [SerializeField] private List<string> weaponNameList;
        [SerializeField] private Slider totalPointSlider;

        public static LobbyManager Instance { get; private set; }
        private int totalPoint;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            connectButton.onClick.AddListener(ConnectToLobby);
            createRoomButton.onClick.AddListener(ShowCreateRoomLayer);
            createAndJoinRoomButton.onClick.AddListener(CreateAndJoinRoom);
            joinRoomButton.onClick.AddListener(ShowRoomList);
            backButton.onClick.AddListener(GoBackToMainLobby);
            leaveRoomButton.onClick.AddListener(LeaveRoom);
            startGameButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);
        }

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                loadingLayer.SetActive(false);
                joinLayer.SetActive(false);
                joinRoomLayer.SetActive(false);
                backButton.gameObject.SetActive(false);

                if (PhotonNetwork.CurrentRoom == null)
                {
                    menuLayer.SetActive(true);
                    roomLayer.SetActive(false);
                }
                else
                {
                    menuLayer.SetActive(false);
                    roomLayer.SetActive(true);

                    roomName.text = PhotonNetwork.CurrentRoom.Name;
                    Player[] players = PhotonNetwork.PlayerList;
                    for (int i = zero; i < players.Length; i++)
                    {
                        Instantiate(playerListItem, playerParent.transform).GetComponent<PlayerListItem>().SetUpPlayer(players[i]);
                    }

                    startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
                    masterLayer.SetActive(PhotonNetwork.IsMasterClient);
                }
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.AutomaticallySyncScene = true;
            }
        }

        public override void OnConnectedToMaster()
        {
            loadingLayer.SetActive(false);
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
            masterLayer.SetActive(PhotonNetwork.IsMasterClient);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach(Transform transform in roomInfoParent.transform)
            {
                Destroy(transform.gameObject);
            }

            for(int i = zero; i < roomList.Count; i++)
            {
                if (roomList[i].RemovedFromList)
                {
                    continue;
                }
                Instantiate(roomInfoPrefab, roomInfoParent.transform).GetComponent<RoomInfoButton>().SetUpRoom(roomList[i]);
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
            joinRoomLayer.SetActive(false);
            menuLayer.SetActive(true);
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
            joinLayer.SetActive(false);
            menuLayer.SetActive(true);
        }

        private void CreateAndJoinRoom()
        {
            if (roomNameText.text.Length > zero)
            {
                loadingLayer.SetActive(true);
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayerPerRoom;
                
                PhotonNetwork.CreateRoom(roomNameText.text, roomOptions);
                createRoomLayer.SetActive(false);
            }
        }

        public void JoinRoomButtonClicked(RoomInfo _info)
        {
            loadingLayer.SetActive(true);
            PhotonNetwork.JoinRoom(_info.Name);
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        private void StartGame()
        {
            GameManager.Instance.SetTotalPoints(totalPoint);
            PhotonNetwork.LoadLevel(one);
        }

        public void WeaponSelected(int index)
        {
            GameManager.Instance.SelectWeapon(index);
            weaponsText.text = weaponNameList[index] + weaponEndText;
        }

        public void SetTotalPoint()
        {
            totalPoint = (int)totalPointSlider.value;
            totalPointText.text = totalPointsString + totalPoint;
        }

        private void GoBackToMainLobby()
        {
            createRoomLayer.SetActive(false);
            joinRoomLayer.SetActive(false);
            roomLayer.SetActive(false);
            menuLayer.SetActive(true);
            backButton.gameObject.SetActive(false);
        }

        private void ShowRoomList()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            menuLayer.SetActive(false);
            joinRoomLayer.SetActive(true);
            backButton.gameObject.SetActive(true);
        }

        private void ShowCreateRoomLayer()
        {
            menuLayer.SetActive(false);
            createRoomLayer.SetActive(true);
            backButton.gameObject.SetActive(true);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}