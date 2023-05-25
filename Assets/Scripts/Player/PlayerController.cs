using Dispersion.Game;
using Dispersion.Interface;
using Dispersion.Weapons;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dispersion.Players
{
    public class PlayerController : MonoBehaviour, IDamagable
    {
        [SerializeField] private float mouseSensitivity, movementSpeed, jumpForce, smoothTime;
        [SerializeField] private Rigidbody rigidBody;
        [SerializeField] private float minVerticalRotation, maxVerticalRotation;
        [SerializeField] private GameObject cameraHolder, ui;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private List<WeaponController> weaponList;
        [SerializeField] private int zero;
        [SerializeField] private float playerMaxHealth;
        [SerializeField] private string rpcTakeDamageString, rpcWeaponString;
        [SerializeField] private Image healthBarImage;

        private float verticalLookRotation;
        private bool isGrounded;
        private Vector3 smoothMoveVelocity, moveAmount;
        private int weaponIndex;
        private float maxHealth, currentHealth;
        private PlayerManager playerManager;

        private void Start()
        {
            playerManager = PhotonView.Find((int)_photonView.InstantiationData[zero]).GetComponent<PlayerManager>();

            maxHealth = playerMaxHealth;
            currentHealth = maxHealth;
            weaponIndex = GameManager.Instance.weapon;
            if (PhotonNetwork.IsMasterClient)
            {
                _photonView.RPC(rpcWeaponString, RpcTarget.All, weaponIndex);
            }

            if (!_photonView.IsMine)
            {
                Destroy(cameraHolder);
                Destroy(rigidBody);
                Destroy(ui);
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
                    weaponList[weaponIndex].Use();
                }
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

        public void TakeDamage(float damage)
        {
            _photonView.RPC(rpcTakeDamageString, RpcTarget.All, damage);
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
        private void RPC_TakeDamage(float damage)
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            currentHealth -= damage;
            healthBarImage.fillAmount = currentHealth / maxHealth;

            if (currentHealth <= zero)
            {
                Die();
            }
        }

        private void Die()
        {
            playerManager.Die();
        }
    }
}