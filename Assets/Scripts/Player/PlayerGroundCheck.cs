using UnityEngine;

namespace Dispersion.Players
{
    public class PlayerGroundCheck : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject == playerController.gameObject)
            {
                return;
            }

            playerController.SetGroundedState(true);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
            {
                return;
            }

            playerController.SetGroundedState(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == playerController.gameObject)
            {
                return;
            }

            playerController.SetGroundedState(false);
        }
    }
}