using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("Максимальное количество игроков в комнате. Когда комната заполнена, новые игроки не могут присоединиться к ней, и поэтому будет создана новая комната")]
        [SerializeField] private byte maxPlayersPerRoom = 4;

        [Tooltip("Панель пользовательского интерфейса, позволяющая пользователю вводить имя, подключаться и играть")]
        [SerializeField] private GameObject controlPanel;

        [Tooltip("UILabel, информирующий пользователя о том, что подключение выполняется")]
        [SerializeField] private GameObject progressLabel;

        [Tooltip("Button, загружает сцену, после присоединения всех пользователей")]
        [SerializeField] private GameObject playButton;

        /// <summary>
        /// Следите за текущим процессом. Поскольку соединение является асинхронным и основано на нескольких обратных вызовах от Photon,
        /// нам нужно отслеживать это, чтобы правильно настроить поведение при получении обратного вызова от Photon.
        /// Обычно это используется для обратного вызова On Connected To Master().
        /// </summary>
        private bool _isConnecting;


        /// <summary>
        /// Номер версии этого клиента. Пользователи разделены между собой по версии игры (что позволяет вносить важные изменения).
        /// </summary>
        private string _gameVersion = "1";

        private void Awake()
        {
            //это гарантирует, что мы сможем использовать PhotonNetwork.LoadLevel() на главном клиенте и все клиенты в одной комнате автоматически синхронизируют свой уровень
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        /// <summary>
        ///  Запускаем процесс подключения.
        ///  Если мы уже подключены, мы пытаемся присоединиться к случайной комнате
        ///  если еще не подключены, подключаем этот экземпляр приложения к Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            _isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // мы проверяем, подключены мы или нет, присоединяемся, если да, в противном случае мы инициируем подключение к серверу.
            if (PhotonNetwork.IsConnected)
            {
                // В данный момент нам необходимо попытаться присоединиться к случайной комнате. Если это не удастся, мы получим уведомление в OnJoinRandomFailed() и создадим новую.
                PhotonNetwork.JoinRandomRoom();
            }

            else
            {
                // Мы должны в первую очередь подключиться к серверу Photon Online.
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = _gameVersion;
            }
        }

        public void OnButtonPlay()
        {
            Debug.LogFormat("Мы загружаем 'Room for {0}' ", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnConnectedToMaster()
        {
            if (_isConnecting)
            {
                Debug.Log("OnConnectedToMaster() был вызван PUN");
                // В данный момент нам необходимо попытаться присоединиться к случайной комнате. Если это не удастся, мы получим уведомление в OnJoinRandomFailed() и создадим новую.
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("При подключении функция Random Failed() была вызвана PUN. Случайной комнаты нет, поэтому мы создаем ее. Вызов: PhotonNetwork.CreateRoom");
            // Нам не удалось присоединиться к случайной комнате, возможно, ее не существует или все они заполнены. Не беспокойтесь, мы создаем новую комнату.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            playButton.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("OnDisconnected() была вызвана PUN не без оснований {0}", cause);
            _isConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() вызван PUN. Сейчас этот клиент находится в комнате.");
            playButton.SetActive(true);
        }
    }
}
