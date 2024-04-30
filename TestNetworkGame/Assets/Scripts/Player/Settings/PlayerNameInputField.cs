using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.BrednikCompany.TestNetworkGame
{
    /// <summary>
    /// ���� ��� ����� ����� ������. ���� ������������ ������ ���� ���, ��� �������� ��� ������� � ����.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        // ��������� ���� PlayerPref, ����� �������� ��������
        const string playerNamePrefKey = "playerName";

        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();

            if( _inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// ������ ��� ������ � ��������� ��� � ���������� PlayerPrefs ��� ������� �������.
        /// </summary>
        /// <param name="value"></param>
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("��� ������ �������� ������� ��� �������������");
                return;
            }

            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}
