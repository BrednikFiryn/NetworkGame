using UnityEngine;

using Photon.Pun;
using System.Collections;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class ShootBullet : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float _bulletSpeed = 100f;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float delay = 5f;

        public bool isFire;

        void Update()
        {
            if (photonView.IsMine) Shooting();
        }

        private void Shooting()
        {
            if (Input.GetButtonDown("Fire1"))
            {  
                if (!isFire)
                {
                    isFire = true;
                    photonView.RPC("SpawnAndMoveBullet", RpcTarget.All);
                }
            }

            isFire = false;
        }

        [PunRPC]
        public void SpawnAndMoveBullet()
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = bullet.transform.forward * _bulletSpeed;
        }
    }
}
