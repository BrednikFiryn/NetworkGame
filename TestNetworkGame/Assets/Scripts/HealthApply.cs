using Photon.Pun;
using UnityEngine;


namespace Com.BrednikCompany.TestNetworkGame
{
    public class HealthApply : MonoBehaviourPunCallbacks
    {
        [Tooltip("значение на которое будет увеличено здоровье")]
        [SerializeField] private float healthApply = 0.1f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
              other.GetComponent<PlayerManager>().Healing(healthApply);
              PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
