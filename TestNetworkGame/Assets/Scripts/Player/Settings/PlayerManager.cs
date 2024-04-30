using Photon.Pun;
using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField] ShootBullet shootBullet;
        [Tooltip("Префаб игрового объекта пользовательского интерфейса игрока")]
        [SerializeField] private GameObject PlayerUiPrefab;
        [Tooltip("Экземпляр локального проигрывателя. Используйте это, чтобы узнать, представлен ли локальный проигрыватель в сцене")]
        public static GameObject LocalPLayerInstance;
        [Tooltip("Текущее здоровье игрока")]
        public float health = 0.5f;

        public void Awake()
        {
            if (photonView.IsMine)
            {
                PlayerManager.LocalPLayerInstance = this.gameObject;
            }
            // мы помечаем как "не уничтожать при загрузке", чтобы экземпляр сохранял синхронизацию уровней, что обеспечивает бесперебойную работу при загрузке уровней.
            DontDestroyOnLoad(this.gameObject);
        }

        public void Start()
        {
            CameraWork _cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }

                else
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> Компонент работы с камерой в playerPrefab.", this);
                }
            }

            if (PlayerUiPrefab != null)
            {
                GameObject _uiGO = Instantiate(this.PlayerUiPrefab);
                _uiGO.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }

            else Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab ссылка на сборку плеера.", this);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Update()
        {
            if (photonView.IsMine)
            {
                if (health <= 0f)
                {
                    health = 1f;
                    transform.position = new Vector3(0f, 5f, 0f);
                }
            }
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnlevelWasLoaded(scene.buildIndex);
        }

        private void CalledOnlevelWasLoaded(int level)
        {
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGO = Instantiate(this.PlayerUiPrefab);
            _uiGO.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(shootBullet.isFire);
                stream.SendNext(health);
            }

            else
            {
                shootBullet.isFire = (bool)stream.ReceiveNext();
                this.health = (float)stream.ReceiveNext();
            }
        }

        public void Damage(float damage)
        {
            this.health -= damage;
        }

        public void Healing(float health)
        {
            if (this.health >= 1f) return;

            this.health += health;
        }
    }
}
