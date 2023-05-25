using Dispersion.Game;
using Dispersion.Interface;
using Dispersion.Weapons;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dispersion.Players
{
    public class PlayerController : MonoBehaviour, IDamagable
    {
        [SerializeField] private float mouseSensitivity, movementSpeed, jumpForce, smoothTime;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float minVerticalRotation, maxVerticalRotation;
        [SerializeField] private GameObject cameraHolder, healthBarUI, nameUI;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private List<WeaponController> weaponList;
        [SerializeField] private int zero, rotationAngle;
        [SerializeField] private float playerMaxHealth;
        [SerializeField] private Image healthBarImage;
        [SerializeField] private TextMeshProUGUI nameText;

        private float verticalLookRotation;
        private bool isGrounded;
        private Vector3 smoothMoveVelocity, moveAmount;
        private int weaponIndex;
        private float maxHealth, currentHealth;
        private PlayerManager playerManager;
        private Camera cam;

        private void Start()
        {
            playerManager = PhotonView.Find((int)_photonView.InstantiationData[zero]).GetComponent<PlayerManager>();
            maxHealth = playerMaxHealth;
            currentHealth = maxHealth;
            nameText.text = _photonView.Owner.NickName;
            weaponIndex = GameManager.Instance.weapon;

            if (PhotonNetwork.IsMasterClient)
            {
                _photonView.RPC(nameof(RPC_Weapon), RpcTarget.All, weaponIndex);
            }

            if (!_photonView.IsMine)
            {
                Destroy(cameraHolder);
                Destroy(rigidBody);
                Destroy(healthBarUI);
            }
            if (_photonView.IsMine)
            {
                nameUI.SetActive(false);
            }
        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                Look();
                Move();
                Jump();

                if(Input.GetMouseButtonDown(zero))
                {
                    weaponList[weaponIndex].Use(_photonView.Owner);
                }
            }
            else
            {
                if(cam == null)
                {
                    cam = FindObjectOfType<Camera>();
                }

                nameUI.transform.LookAt(cam.transform);
                nameUI.transform.Rotate(Vector3.up * rotationAngle);
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

        public void TakeDamage(float damage, Photon.Realtime.Player killer)
        {
            _photonView.RPC(nameof(RPC_TakeDamage), _photonView.Owner, damage, killer);
        }

        [PunRPC]
        private void RPC_Weapon(int weapon)
        {
            foreach(WeaponController go in weaponList)
            {
                go.gameObject.SetActive(false);
            }
            weaponIndex = weapon;
            weaponList[weaponIndex].gameObject.SetActive(true);
        }

        [PunRPC]
        private void RPC_TakeDamage(float damage, Photon.Realtime.Player killer)
        {
            currentHealth -= damage;
            healthBarImage.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= zero)
            {
                Die(killer);
            }
        }

        private void Die(Photon.Realtime.Player killer)
        {
            GameManager.Instance.UpdateGameStats(_photonView.Owner, killer);
            playerManager.Die();
        }
    }
}