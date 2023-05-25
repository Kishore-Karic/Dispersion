using Dispersion.GenericSingleton;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dispersion.Game
{
    public class GameManager : GenericSingleton<GameManager>
    {
        [field: SerializeField] public int winningPoint { get; private set; }
        [SerializeField] private int zero, one;

        public int weapon { get; private set; }
        private Dictionary<Player, int> playersScore;
        private Dictionary<Player, int> playersDeath;

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if(scene.buildIndex == one)
            {
                playersScore = new Dictionary<Player, int>();
                playersDeath = new Dictionary<Player, int>();
            }
        }

        public void SelectWeapon(int index)
        {
            weapon = index;
        }

        public void UpdateGameStats(Player player, Player killer)
        {
            if (!playersScore.ContainsKey(killer))
            {
                playersScore.Add(killer, zero);
            }
            if (!playersDeath.ContainsKey(player))
            {
                playersDeath.Add(player, zero);
            }

            int score = zero;
            if (playersScore.TryGetValue(killer, out score))
            {
                score++;
                playersScore[killer] = score;
            }
            score = zero;
            if(playersDeath.TryGetValue(player, out score))
            {
                score++;
                playersDeath[player] = score;
            }

            int tempScore;
            playersScore.TryGetValue(killer, out tempScore);
            Debug.Log(killer.NickName + " " + tempScore + " kill");
            playersDeath.TryGetValue(player, out tempScore);
            Debug.Log(player.NickName + " " + tempScore + " death");
        }
    }
}