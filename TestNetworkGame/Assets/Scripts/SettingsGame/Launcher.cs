using Photon.Realtime;
using Photon.Pun;

using UnityEngine;

namespace Com.BrednikCompany.TestNetworkGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [Tooltip("������������ ���������� ������� � �������. ����� ������� ���������, ����� ������ �� ����� �������������� � ���, � ������� ����� ������� ����� �������")]
        [SerializeField] private byte maxPlayersPerRoom = 4;

        [Tooltip("������ ����������������� ����������, ����������� ������������ ������� ���, ������������ � ������")]
        [SerializeField] private GameObject controlPanel;

        [Tooltip("UILabel, ������������� ������������ � ���, ��� ����������� �����������")]
        [SerializeField] private GameObject progressLabel;

        [Tooltip("Button, ��������� �����, ����� ������������� ���� �������������")]
        [SerializeField] private GameObject playButton;

        /// <summary>
        /// ������� �� ������� ���������. ��������� ���������� �������� ����������� � �������� �� ���������� �������� ������� �� Photon,
        /// ��� ����� ����������� ���, ����� ��������� ��������� ��������� ��� ��������� ��������� ������ �� Photon.
        /// ������ ��� ������������ ��� ��������� ������ On Connected To Master().
        /// </summary>
        private bool _isConnecting;


        /// <summary>
        /// ����� ������ ����� �������. ������������ ��������� ����� ����� �� ������ ���� (��� ��������� ������� ������ ���������).
        /// </summary>
        private string _gameVersion = "1";

        private void Awake()
        {
            //��� �����������, ��� �� ������ ������������ PhotonNetwork.LoadLevel() �� ������� ������� � ��� ������� � ����� ������� ������������� �������������� ���� �������
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        /// <summary>
        ///  ��������� ������� �����������.
        ///  ���� �� ��� ����������, �� �������� �������������� � ��������� �������
        ///  ���� ��� �� ����������, ���������� ���� ��������� ���������� � Photon Cloud Network
        /// </summary>
        public void Connect()
        {
            _isConnecting = true;
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // �� ���������, ���������� �� ��� ���, ��������������, ���� ��, � ��������� ������ �� ���������� ����������� � �������.
            if (PhotonNetwork.IsConnected)
            {
                // � ������ ������ ��� ���������� ���������� �������������� � ��������� �������. ���� ��� �� �������, �� ������� ����������� � OnJoinRandomFailed() � �������� �����.
                PhotonNetwork.JoinRandomRoom();
            }

            else
            {
                // �� ������ � ������ ������� ������������ � ������� Photon Online.
                _isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = _gameVersion;
            }
        }

        public void OnButtonPlay()
        {
            Debug.LogFormat("�� ��������� 'Room for {0}' ", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.PlayerCount);
        }

        public override void OnConnectedToMaster()
        {
            if (_isConnecting)
            {
                Debug.Log("OnConnectedToMaster() ��� ������ PUN");
                // � ������ ������ ��� ���������� ���������� �������������� � ��������� �������. ���� ��� �� �������, �� ������� ����������� � OnJoinRandomFailed() � �������� �����.
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("��� ����������� ������� Random Failed() ���� ������� PUN. ��������� ������� ���, ������� �� ������� ��. �����: PhotonNetwork.CreateRoom");
            // ��� �� ������� �������������� � ��������� �������, ��������, �� �� ���������� ��� ��� ��� ���������. �� ������������, �� ������� ����� �������.
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            playButton.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarningFormat("OnDisconnected() ���� ������� PUN �� ��� ��������� {0}", cause);
            _isConnecting = false;
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() ������ PUN. ������ ���� ������ ��������� � �������.");
            playButton.SetActive(true);
        }
    }
}
