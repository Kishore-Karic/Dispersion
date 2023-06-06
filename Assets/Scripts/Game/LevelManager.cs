using Dispersion.Enum;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dispersion.Game
{
    public class LevelManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI winningText;
        [SerializeField] private string wonString;
        [SerializeField] private Camera cam;
        [SerializeField] private int one;
        [SerializeField] private GameObject endCanvas, limitedPlayerCanvas, scoreBoardPrefab, scoreBoardParent, masterLayer, clientLayer, pauseLayer;
        
        public static LevelManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void DisplayOutro(string playerName)
        {
            endCanvas.SetActive(true);
            winningText.text = playerName + wonString;
            cam.gameObject.SetActive(true);

            if (PhotonNetwork.IsMasterClient)
            {
                masterLayer.SetActive(true);
            }
            else
            {
                clientLayer.SetActive(true);
            }
        }

        public void ScoreBoard(Player[] playersList, Dictionary<Player, Dictionary<ScoreType, int>> playersScore)
        {
            int rank = one;
            foreach(Player player in playersList)
            {
                ScoreBoardItem scoreBoardItem = Instantiate(scoreBoardPrefab, scoreBoardParent.transform).GetComponent<ScoreBoardItem>();

                scoreBoardItem.rankText.text = rank.ToString();
                scoreBoardItem.playerNameText.text = player.NickName;
                scoreBoardItem.killsText.text = playersScore[player][ScoreType.kills].ToString();
                scoreBoardItem.deathsText.text = playersScore[player][ScoreType.deaths].ToString();

                rank++;
            }
        }

        public void BackToRoom()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - one);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - one);
        }

        public void StayHere()
        {
            limitedPlayerCanvas.SetActive(false);
        }

        public void LimitedPlayers()
        {
            limitedPlayerCanvas.SetActive(true);
        }

        public void Pause()
        {
            pauseLayer.SetActive(true);
        }

        public void Resume()
        {
            pauseLayer.SetActive(false);
        }
    }
}