using System.Collections.Generic;
using UnityEngine;

namespace Dispersion.Game
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> spawnPoints;
        [SerializeField] private int zero;

        public static SpawnManager Instance { get; private set; }

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

        public Transform GetRandomSpawnPoint()
        {
            return spawnPoints[Random.Range(zero, spawnPoints.Count)].transform;
        }
    }
}