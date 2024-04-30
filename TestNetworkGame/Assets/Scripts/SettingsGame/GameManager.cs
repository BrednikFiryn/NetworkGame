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
        [Tooltip("������� �����������, ������������ ��� ������������� ������")]
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
                Debug.LogError("<Color=Red><a>Missing</a></Color> ������ �� ������ ������. ����������, ��������� �� � GameObject 'Game Manager'", this);
            }

            else
            {
                if (PhotonNetwork.InRoom && PlayerManager.LocalPLayerInstance == null)
                {
                    targetPosition = new Vector3(0f, 0f, 0f);
                    id = PhotonNetwork.LocalPlayer.ActorNumber;
                    rotation = Quaternion.LookRotation(targetPosition - spawnPoints[id - 1].position);

                    Debug.LogFormat("�� ������� ��������� ���������� ������ �� {0}", SceneManagerHelper.ActiveSceneName);
                    // �� � �������. �������� ��������� ��� �������� ������. �� ���������������� � ������� PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[id - 1].position, rotation, 0);
                }

                else
                {
                    Debug.LogFormat("������������� �������� ����� ��� {0}", SceneManagerHelper.ActiveSceneName);
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
            // ����������: ��������, ��� ��� ������������� ��������� �� ��������� (��� �� �������) ��� ��������� � ���� OnJoinedRoom
            // ��-�� ����� ����� Start() ����� ���������, ��� �� ��������� �������� ������ � ����!
            if (PlayerManager.LocalPLayerInstance == null)
            {
                Debug.LogFormat("�� ������� ��������� ���������� ������ �� {0}", SceneManagerHelper.ActiveSceneName);
                // �� � �������. �������� ��������� ��� �������� ������.
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPoints[id - 1].position, rotation, 0);

            }
        }

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // �� �����, ��������� �� �� �������, �������������� � ����.

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // ���������� ����� OnPlayerLeftRoom

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // �����, ����� ������ �����������

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // ���������� ����� OnPlayerLeftRoom

                LoadArena();
            }
        }

        /// <summary>
        /// ����������, ����� ������� ����� �������� �������. ��� ����� ��������� ����� �������.
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
                Debug.LogError("PhotonNetwork : �������� ��������� �������, �� �� �� �������� �������� ��������");
                return;
            }

            Debug.LogFormat("PhotonNetwork : ������� �������� : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

            PhotonNetwork.LoadLevel("������� ��� " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
    }
}
