using Dispersion.Enum;
using Dispersion.Players;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dispersion.Game
{
    public class RoomManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int zero, one, killScoreValue;
        [SerializeField] private PlayerManager playerManager;

        private Dictionary<Player, Dictionary<ScoreType, int>> playersScore;
        public static RoomManager Instance { get; private set; }

        private bool isEndScreen;
        private int winningPoints;

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

            isEndScreen = false;
        }

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
                PhotonNetwork.Instantiate(playerManager.name, Vector3.zero, Quaternion.identity);

                playersScore = new Dictionary<Player, Dictionary<ScoreType, int>>();

                if(PhotonNetwork.PlayerList.Length == one)
                {
                    LevelManager.Instance.LimitedPlayers();
                }
            }
        }

        public void SetTotalPoints(int totalPoints)
        {
            winningPoints = totalPoints;
        }

        public void UpdateGameStats(Player player, Player killer, PlayerController playerController)
        {
            if (!playersScore.ContainsKey(player))
            {
                playersScore[player] = new Dictionary<ScoreType, int>();
            }

            if (!playersScore[player].ContainsKey(ScoreType.kills))
            {
                playersScore[player].Add(ScoreType.kills, zero);
            }

            if (!playersScore[player].ContainsKey(ScoreType.deaths))
            {
                playersScore[player].Add(ScoreType.deaths, zero);
            }

            if (!playersScore[player].ContainsKey(ScoreType.totalScore))
            {
                playersScore[player].Add(ScoreType.totalScore, zero);
            }

            int deathScore = zero;
            if (playersScore[player].TryGetValue(ScoreType.deaths, out deathScore))
            {
                deathScore++;
                playersScore[player][ScoreType.deaths] = deathScore;
            }

            playersScore[player][ScoreType.totalScore] = playersScore[player][ScoreType.kills] * killScoreValue - playersScore[player][ScoreType.deaths];

            if (!playersScore.ContainsKey(killer))
            {
                playersScore[killer] = new Dictionary<ScoreType, int>();
            }

            if (!playersScore[killer].ContainsKey(ScoreType.kills))
            {
                playersScore[killer].Add(ScoreType.kills, zero);
            }

            if (!playersScore[killer].ContainsKey(ScoreType.deaths))
            {
                playersScore[killer].Add(ScoreType.deaths, zero);
            }

            if (!playersScore[killer].ContainsKey(ScoreType.totalScore))
            {
                playersScore[killer].Add(ScoreType.totalScore, zero);
            }

            int killScore = zero;
            if (playersScore[killer].TryGetValue(ScoreType.kills, out killScore))
            {
                killScore++;
                playersScore[killer][ScoreType.kills] = killScore;
            }

            playersScore[killer][ScoreType.totalScore] = playersScore[killer][ScoreType.kills] * killScoreValue - playersScore[killer][ScoreType.deaths];

            if (killScore == winningPoints)
            {
                isEndScreen = true;
                playerController.GameEnd();
                SortPlayersByKills(killer);
            }
        }

        public bool IsGameEnd(Player killer)
        {
            if (!playersScore.ContainsKey(killer))
            {
                playersScore[killer] = new Dictionary<ScoreType, int>();
            }

            if (!playersScore[killer].ContainsKey(ScoreType.kills))
            {
                playersScore[killer].Add(ScoreType.kills, zero);
            }

            int score = zero;
            playersScore[killer].TryGetValue(ScoreType.kills, out score);
            return (score == winningPoints);
        }

        private void SortPlayersByKills(Player winner)
        {
            Player[] playersList = playersScore.Keys.OrderByDescending(n => playersScore[n][ScoreType.totalScore]).ToArray();
            DisplayScoreBoard(winner, playersList);
        }

        private void DisplayScoreBoard(Player winner, Player[] playersList)
        {
            LevelManager.Instance.DisplayOutro(winner.NickName);
            LevelManager.Instance.ScoreBoard(playersList, playersScore);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            if (otherPlayer != PhotonNetwork.LocalPlayer && PhotonNetwork.PlayerList.Length == one && !isEndScreen)
            {
                LevelManager.Instance.LimitedPlayers();
            }
        }
    }
}