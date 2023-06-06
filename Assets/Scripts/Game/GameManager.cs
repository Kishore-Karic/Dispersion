using UnityEngine;

namespace Dispersion.Game
{
    public class GameManager : MonoBehaviour
    {
        public int weapon { get; private set; }
        public int totalPoints { get; private set; }

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SelectWeapon(int index)
        {
            weapon = index;
        }

        public void SetTotalPoints(int value)
        {
            totalPoints = value;
        }
    }
}