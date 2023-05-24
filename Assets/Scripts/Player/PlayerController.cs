using Photon.Pun;
using UnityEngine;

namespace Dispersion.Players
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float mouseSensitivity, movementSpeed, jumpForce, smoothTime;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float minVerticalRotation, maxVerticalRotation;
        [SerializeField] private GameObject cameraHolder;
        [SerializeField] private PhotonView _photonView;

        private float verticalLookRotation;
        private bool isGrounded;
        private Vector3 smoothMoveVelocity, moveAmount;

        private void Start()
        {
            if (!_photonView.IsMine)
            {
                Destroy(cameraHolder);
                Destroy(rigidBody);
            }
        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                Look();
                Move();
                Jump();
            }
        }

        private void FixedUpdate()
        {
            if (_photonView.IsMine)
            {
                rigidBody.MovePosition(rigidBody.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
            }
        }

        private void Look()
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

            verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, minVerticalRotation, maxVerticalRotation);

            cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
        }

        private void Move()
        {
            Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

            moveAmount = Vector3.SmoothDamp(moveAmount, moveDirection * movementSpeed, ref smoothMoveVelocity, smoothTime);
        }

        private void Jump()
        {
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rigidBody.AddForce(transform.up * jumpForce);
            }
        }

        public void SetGroundedState(bool _value)
        {
            isGrounded = _value;
        }
    }
}