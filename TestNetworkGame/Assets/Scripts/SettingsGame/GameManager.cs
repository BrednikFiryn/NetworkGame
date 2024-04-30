using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections.Generic;

using Photon.Pun;
using Photon.Realtime;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        static public GameManager instance;

        public List<Transform> spawnPoints;
        [Tooltip("Сборная конструкция, используемая для представления игрока")]
        [SerializeField] private GameObject playerPrefab;
        private Vector3 targetPosition;
        private int id;
        private Quaternion rotation;

        private void Start()
        {
            instance = this;

            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(0);
                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> ссылка на сборку игрока. Пожалуйста, настройте ее в GameObject 'Game Manager'", this);
            }

            else
            {
                if (PhotonNetwork.InRoom && PlayerManager.LocalPLayerInstance == null)
                {
                    targetPosition = new Vector3(0f, 0f, 0f);
                    id = PhotonNetwork.LocalPlayer.ActorNumber;
                    rotation = Quaternion.LookRotation(targetPosition - spawnPoints[id - 1].position);

                    Debug.LogFormat("Мы создаем экземпляр локального игрока из {0}", SceneManagerHelper.ActiveSceneName);
                    // мы в комнате. создайте персонажа для местного игрока. он синхронизируется с помощью PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[id - 1].position, rotation, 0);
                }

                else
                {
                    Debug.LogFormat("Игнорирование загрузки сцены для {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
        }

        public override void OnJoinedRoom()
        {
            // Примечание: возможно, что это единообразное поведение не создается (или не активно) при включении в игру OnJoinedRoom
            // из-за этого метод Start() также проверяет, был ли локальный персонаж создан в сети!
            if (PlayerManager.LocalPLayerInstance == null)
            {
                Debug.LogFormat("Мы создаем экземпляр локального игрока из {0}", SceneManagerHelper.ActiveSceneName);
                // мы в комнате. создайте персонажа для местного игрока.
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[id - 1].position, rotation, 0);

            }
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // не видно, являетесь ли вы игроком, подключающимся к сети.

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // вызывается перед OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // видно, когда другие отключаются

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // вызывается перед OnPlayerLeftRoom

                LoadArena();
            }
        }

        /// <summary>
        /// Вызывается, когда местный игрок покидает комнату. Нам нужно загрузить сцену запуска.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        public void QuitApplication()
        {
            Application.Quit();
        }

        private void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Пытаемся загрузить уровень, но мы не являемся основным клиентом");
                return;
            }

            Debug.LogFormat("PhotonNetwork : Уровень загрузки : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("Комната для " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
}
