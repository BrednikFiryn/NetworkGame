using Photon.Pun;
using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class Bullet : MonoBehaviourPunCallbacks
    {
        [SerializeField] private float damage;

        private void OnCollisionEnter(Collision collision)
        {
                if (collision.gameObject.CompareTag("Wall"))
                {
                    Debug.Log("Попадание в стену");
                    Destroy(gameObject);     
                }

                else if (collision.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Попадание в игрока");
                    var playerManager = collision.gameObject.GetComponent<PlayerManager>();
                    playerManager.Damage(damage);
                    Destroy(gameObject);
                }
        }
    }
}
